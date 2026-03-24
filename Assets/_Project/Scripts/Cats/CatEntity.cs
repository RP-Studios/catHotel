using System.Collections.Generic;
using Action = System.Action;
using UnityEngine;
using DG.Tweening;
using CatHotel.Grid;
using CatHotel.Core;
using CatHotel.Hotel;

namespace CatHotel.Cats
{
    public enum CatDirection { Front, Back, Right, Left }

    public class CatEntity : MonoBehaviour
    {
        [Header("Sprites (fallback)")]
        [SerializeField] private Sprite _frontSprite;
        [SerializeField] private Sprite _rightSprite;
        [SerializeField] private Sprite _backSprite;

        [Header("Movement")]
        [SerializeField] private float _cellMoveTime   = 0.7f;
        [SerializeField] private float _idleTimeMin    = 1f;
        [SerializeField] private float _idleTimeMax    = 3f;
        [SerializeField] private int   _wanderStepsMin = 2;
        [SerializeField] private int   _wanderStepsMax = 6;

        // Idle: multiple variants per direction, chosen randomly
        private static readonly string[][] IdleStates =
        {
            new[] { "Idle1_Front", "Idle2_Front", "Idle3_Front" }, // Front
            new[] { "Idle1_Back", "Idle3_Back" },                   // Back (no Idle2)
            new[] { "Idle1_Right", "Idle2_Right", "Idle3_Right" }, // Right
            new[] { "Idle1_Left", "Idle2_Left", "Idle3_Left" },    // Left
        };

        private SpriteRenderer _sr;
        private Animator _animator;
        private GridData _grid;
        private CatSpawner _spawner;
        private CatNeeds _needs;
        private CatBreedData _breed;
        private Vector2Int _gridPos;
        private Sequence _moveSequence;
        private Tween _pendingAction;
        private CatDirection _currentDir;
        private bool _isWalking;
        private bool _useSadWalk;
        private bool _isFighting;
        private float _fightCooldown;
        private string _chosenRestState;
        private BedSpot _claimedBed;
        private bool _isDeparting;

        // Object interaction
        private HotelObject _targetObject;
        private bool _isUsingObject;

        // Pooled path buffer to avoid GC allocation per wander
        private readonly List<Vector2Int> _pathBuffer = new(8);

        public Vector2Int GridPos => _gridPos;
        public bool IsFighting => _isFighting;
        public bool IsUsingObject => _isUsingObject;
        public SpriteRenderer SpriteRenderer => _sr;
        public CatBreedData Breed => _breed;

        /// <summary>Fired when the cat finishes using a service object.</summary>
        public event Action OnServiceUsed;

        /// <summary>
        /// Sync _gridPos with actual world position.
        /// Must be called whenever a walk sequence is killed mid-way,
        /// otherwise _gridPos stays at the start of the interrupted walk
        /// and all subsequent pathfinding uses a stale position.
        /// </summary>
        private void SyncGridPos()
        {
            Vector3 pos = transform.position;
            var newPos = new Vector2Int(
                Mathf.FloorToInt(pos.x),
                Mathf.FloorToInt(pos.y));
            _gridPos = newPos;
        }

        public void SetSprites(Sprite front, Sprite right, Sprite back)
        {
            _frontSprite = front;
            _rightSprite = right;
            _backSprite = back;
        }

        public void Init(GridData grid, Vector2Int startCell, CatSpawner spawner = null)
        {
            _grid = grid;
            _spawner = spawner;
            _gridPos = startCell;
            _sr = GetComponent<SpriteRenderer>();
            _animator = GetComponent<Animator>();
            _needs = GetComponent<CatNeeds>();
            transform.position = CellToWorld(startCell);

            _currentDir = CatDirection.Front;
            EnterRest();
        }

        private void LateUpdate()
        {
            // Sort by Y: lower Y = closer to camera = higher sortingOrder
            if (_sr != null)
                _sr.sortingOrder = Mathf.RoundToInt(-transform.position.y * 100f) + 10000 + (GetInstanceID() & 0xFF);

            if (_fightCooldown > 0f)
                _fightCooldown -= Time.deltaTime;
        }

        /// <summary>Set breed data for speed multiplier.</summary>
        public void SetBreed(CatBreedData breed)
        {
            _breed = breed;
            if (breed != null)
                _cellMoveTime = 0.7f / breed.speed;
        }

        private void OnDestroy()
        {
            _moveSequence?.Kill();
            _pendingAction?.Kill();
            ReleaseCurrentObject();
            if (_claimedBed != null && _spawner != null)
            {
                _spawner.ReleaseBed(_claimedBed);
                _claimedBed = null;
            }
        }

        // --- Emotes (one-shot, triggered externally) ---

        public void PlayHappy() => PlayEmote("Happy", 2f);
        public void PlayUnhappy() => PlayEmote("Unhappy", 1f);

        public void PlayPetting(RuntimeAnimatorController handController)
        {
            if (_isFighting) return;

            _moveSequence?.Kill();
            _pendingAction?.Kill();
            _isWalking = false;
            _chosenRestState = null;
            ReleaseCurrentObject();
            ReleaseBedIfClaimed();

            _currentDir = CatDirection.Front;
            PlayAnimState("Pet_Front");

            GameObject handGo = null;
            if (handController != null)
            {
                handGo = new GameObject("HandPet");
                handGo.transform.position = transform.position + new Vector3(0, 0.5f, 0);
                var handSr = handGo.AddComponent<SpriteRenderer>();
                handSr.sortingOrder = 15;
                var handAnim = handGo.AddComponent<Animator>();
                handAnim.runtimeAnimatorController = handController;
            }

            _pendingAction = DOVirtual.DelayedCall(2f, () =>
            {
                if (handGo != null) Object.Destroy(handGo);
                _chosenRestState = null;
                EnterIdle();
            });
        }

        private void ReleaseBedIfClaimed()
        {
            if (_claimedBed != null && _spawner != null)
            {
                _spawner.ReleaseBed(_claimedBed);
                _claimedBed = null;
            }
        }

        private void ReleaseCurrentObject()
        {
            if (_targetObject != null)
            {
                // Show toy objects again after cat is done playing
                if (_targetObject.Data.category == CatHotel.Core.ObjectCategory.Play)
                    SetObjectVisible(_targetObject, true);
                _targetObject.Release(GetInstanceID());
                _targetObject = null;
            }
            _isUsingObject = false;
        }

        private static void SetObjectVisible(HotelObject obj, bool visible)
        {
            var sr = obj.GetComponent<SpriteRenderer>();
            if (sr != null) sr.enabled = visible;
        }

        private void PlayEmote(string prefix, float duration)
        {
            if (_isFighting) return;
            _moveSequence?.Kill();
            _pendingAction?.Kill();
            _isWalking = false;
            _chosenRestState = null;
            ReleaseCurrentObject();
            ReleaseBedIfClaimed();

            string state = DirectionalState(prefix, _currentDir);
            PlayAnimState(state);

            _pendingAction = DOVirtual.DelayedCall(duration, () =>
            {
                _chosenRestState = null;
                EnterIdle();
            });
        }

        // --- Combat ---

        private bool CanFight()
        {
            if (_animator == null || _isFighting) return false;
            if (_fightCooldown > 0f) return false;
            return _animator.HasState(0, Animator.StringToHash("Fight_In_Left"));
        }

        private bool TryInitiateFight()
        {
            if (_spawner == null || !CanFight()) return false;

            // Only fight if happiness is low (GDD: < 50%)
            var happiness = GetComponent<CatHappiness>();
            if (happiness != null && happiness.Value >= 50f) return false;

            var leftPos = new Vector2Int(_gridPos.x - 1, _gridPos.y);
            var rightPos = new Vector2Int(_gridPos.x + 1, _gridPos.y);

            var neighbor = _spawner.GetCatAt(leftPos, this);
            if (neighbor == null || !neighbor.CanFight())
                neighbor = _spawner.GetCatAt(rightPos, this);
            if (neighbor == null || !neighbor.CanFight())
                return false;

            CatEntity leftCat = _gridPos.x < neighbor.GridPos.x ? this : neighbor;
            CatEntity rightCat = _gridPos.x < neighbor.GridPos.x ? neighbor : this;

            RunFightSequence(leftCat, rightCat);
            return true;
        }

        private void EnterFight()
        {
            _isFighting = true;
            _moveSequence?.Kill();
            _pendingAction?.Kill();
            _isWalking = false;
            _chosenRestState = null;
            ReleaseCurrentObject();
            ReleaseBedIfClaimed();
        }

        private void RunFightSequence(CatEntity left, CatEntity right)
        {
            left.EnterFight();
            right.EnterFight();

            // Apply happiness penalty (GDD: -25)
            var leftH = left.GetComponent<CatHappiness>();
            var rightH = right.GetComponent<CatHappiness>();
            leftH?.ApplyFightPenalty();
            rightH?.ApplyFightPenalty();

            var seq = DOTween.Sequence();
            GameObject cloudGo = null;

            seq.AppendCallback(() =>
            {
                left._currentDir = CatDirection.Right;
                right._currentDir = CatDirection.Left;
                left.PlayAnimState("Fight_In_Right");
                right.PlayAnimState("Fight_In_Left");
            });
            seq.AppendInterval(2f);

            seq.AppendCallback(() =>
            {
                left._sr.enabled = false;
                right._sr.enabled = false;

                Vector3 midpoint = (left.transform.position + right.transform.position) / 2f;
                cloudGo = new GameObject("FightCloud");
                cloudGo.transform.position = midpoint;

                var cloudSr = cloudGo.AddComponent<SpriteRenderer>();
                cloudSr.sortingOrder = 15;

                if (_spawner != null)
                {
                    var cloudAnim = cloudGo.AddComponent<Animator>();
                    cloudAnim.runtimeAnimatorController = _spawner.FightCloudController;
                }
            });
            seq.AppendInterval(3f); // GDD: 3 seconds

            seq.AppendCallback(() =>
            {
                if (cloudGo != null) Object.Destroy(cloudGo);
                left._sr.enabled = true;
                right._sr.enabled = true;
                left.PlayAnimState("Fight_Out_Right");
                right.PlayAnimState("Fight_Out_Left");
            });
            seq.AppendInterval(1f);

            seq.AppendCallback(() =>
            {
                left._isFighting = false;
                right._isFighting = false;
                left._fightCooldown = 30f;
                right._fightCooldown = 30f;
                left.EnterRest();
                right.EnterRest();
            });
        }

        // --- Need-Driven AI ---

        /// <summary>
        /// Main decision point. Checks needs and decides what to do next.
        /// Priority: Combat > Seek object for urgent need > Idle/Wander.
        /// </summary>
        private void EnterRest()
        {
            if (_isDeparting) return;
            SyncGridPos();
            _isWalking = false;
            _chosenRestState = null;
            ReleaseCurrentObject();

            if (TryInitiateFight()) return;
            if (_needs != null && TrySeekObject()) return;
            EnterIdle();
        }

        /// <summary>
        /// Find the most urgent need and walk to an object that satisfies it.
        /// </summary>
        private bool TrySeekObject()
        {
            var urgentNeed = _needs.GetMostUrgentNeed();
            if (urgentNeed == null) return false;

            var obj = ObjectRegistry.FindNearest(urgentNeed.Value, _gridPos);
            if (obj == null) return false;
            if (!obj.TryReserve(GetInstanceID())) return false;

            _targetObject = obj;
            _isUsingObject = false;

            WalkToTarget(obj.GridPos, () => StartUsingObject(urgentNeed.Value));
            return true;
        }

        /// <summary>
        /// Cat arrived at object. Play use animation and satisfy need over time.
        /// </summary>
        private void StartUsingObject(NeedType need)
        {
            if (_targetObject == null)
            {
                EnterIdle();
                return;
            }

            _isUsingObject = true;

            // Hide toy objects while cat is playing with them
            if (_targetObject.Data.category == CatHotel.Core.ObjectCategory.Play)
                SetObjectVisible(_targetObject, false);

            string animPrefix = need switch
            {
                NeedType.Hunger => "Eat",
                NeedType.Sleep => "Sleep",
                NeedType.Play => "Play",
                NeedType.Clean => "Clean",
                _ => "Eat"
            };

            _chosenRestState = DirectionalState(animPrefix, _currentDir);
            PlayAnimState(_chosenRestState);

            float duration = _targetObject.Data.useDuration;
            float ratePerSec = _targetObject.SatisfactionRate;
            float elapsed = 0f;

            Debug.Log($"[CatEntity] {gameObject.name} started using object for need={need}, duration={duration}s");
            _pendingAction = DOVirtual.Float(0f, duration, duration, t =>
            {
                float dt = t - elapsed;
                elapsed = t;
                if (_needs != null)
                    _needs.Satisfy(need, ratePerSec * dt);
            }).OnComplete(() =>
            {
                Debug.Log($"[CatEntity] {gameObject.name} finished using object, firing OnServiceUsed (listeners={OnServiceUsed?.GetInvocationList()?.Length ?? 0})");
                ReleaseCurrentObject();
                _chosenRestState = null;
                OnServiceUsed?.Invoke();
                EnterIdle();
            });
        }

        private void EnterIdle()
        {
            _isWalking = false;

            if (_chosenRestState == null)
            {
                var options = IdleStates[(int)_currentDir];
                _chosenRestState = options[Random.Range(0, options.Length)];
            }

            PlayAnimState(_chosenRestState);

            float idleTime = Random.Range(_idleTimeMin, _idleTimeMax);
            _pendingAction = DOVirtual.DelayedCall(idleTime, () =>
            {
                // After idle, check needs again before wandering
                if (_needs != null && TrySeekObject()) return;
                Wander();
            });
        }

        // --- Targeted Movement (BFS pathfinding) ---

        public void WalkToTarget(Vector2Int target)
        {
            WalkToTarget(target, null);
        }

        /// <summary>Force sad walk animation (used for unhappy departures).</summary>
        public void SetSadWalk() => _useSadWalk = true;

        /// <summary>Mark cat as departing — stops all AI behavior.</summary>
        public void SetDeparting()
        {
            _isDeparting = true;
            _pendingAction?.Kill();
            if (_isUsingObject && _targetObject != null)
            {
                _targetObject.Release(GetInstanceID());
                _targetObject = null;
                _isUsingObject = false;
            }
        }

        public void WalkToTarget(Vector2Int target, System.Action onArrival)
        {
            if (_isFighting) return;
            _moveSequence?.Kill();
            _pendingAction?.Kill();
            SyncGridPos();
            _isWalking = false;
            _chosenRestState = null;

            var path = _grid.FindPath(_gridPos, target);
            if (path == null || path.Count < 2)
            {
                if (onArrival != null)
                {
                    onArrival();
                    if (!_isWalking && !_isUsingObject && !_isFighting)
                        EnterRest();
                }
                else EnterRest();
                return;
            }

            _isWalking = true;
            _moveSequence = DOTween.Sequence();

            for (int i = 1; i < path.Count; i++)
            {
                Vector2Int cell = path[i];
                Vector2Int from = path[i - 1];
                Vector2Int delta = cell - from;

                CatDirection dir = DeltaToDirection(delta);
                _moveSequence.AppendCallback(() => SetWalkDirection(dir));

                Vector3 worldTarget = CellToWorld(cell);
                _moveSequence.Append(
                    transform.DOMove(worldTarget, _cellMoveTime)
                        .SetEase(Ease.Linear));
            }

            Vector2Int finalCell = path[^1];
            _moveSequence.OnComplete(() =>
            {
                _gridPos = finalCell;
                _isWalking = false;
                if (onArrival != null) onArrival();
                // If callback didn't start a new action, restart AI
                if (!_isWalking && !_isUsingObject && !_isFighting)
                    EnterRest();
            });
        }

        // --- Random Movement ---

        private void Wander()
        {
            SyncGridPos();
            int steps = Random.Range(_wanderStepsMin, _wanderStepsMax + 1);
            var path = BuildRandomPath(steps);

            if (path.Count == 0)
            {
                EnterRest();
                return;
            }

            _isWalking = true;
            _chosenRestState = null;
            _moveSequence = DOTween.Sequence();

            for (int i = 0; i < path.Count; i++)
            {
                Vector2Int target = path[i];
                Vector2Int from = i == 0 ? _gridPos : path[i - 1];
                Vector2Int delta = target - from;

                CatDirection dir = DeltaToDirection(delta);
                _moveSequence.AppendCallback(() => SetWalkDirection(dir));

                Vector3 worldTarget = CellToWorld(target);
                _moveSequence.Append(
                    transform.DOMove(worldTarget, _cellMoveTime)
                        .SetEase(Ease.Linear));
            }

            Vector2Int finalCell = path[^1];
            _moveSequence.OnComplete(() =>
            {
                _gridPos = finalCell;
                EnterRest();
            });
        }

        private void SetWalkDirection(CatDirection dir)
        {
            _currentDir = dir;
            if (_useSadWalk && dir == CatDirection.Right)
                PlayAnimState("SadWalk_Right");
            else
                PlayAnimState($"Walk_{dir}");
        }

        // --- Helpers ---

        private static string DirectionalState(string prefix, CatDirection dir)
        {
            return dir == CatDirection.Back
                ? $"{prefix}_Front"
                : $"{prefix}_{dir}";
        }

        private void PlayAnimState(string stateName)
        {
            if (_animator == null) return;
            _sr.flipX = false;

            int hash = Animator.StringToHash(stateName);
            if (_animator.HasState(0, hash))
            {
                if (!_animator.enabled) _animator.enabled = true;
                _animator.Play(hash, 0, 0f);
            }
            else
            {
                _animator.enabled = false;
                switch (_currentDir)
                {
                    case CatDirection.Back:
                        _sr.sprite = _backSprite != null ? _backSprite : _frontSprite;
                        break;
                    case CatDirection.Right:
                        _sr.sprite = _rightSprite != null ? _rightSprite : _frontSprite;
                        break;
                    case CatDirection.Left:
                        _sr.sprite = _rightSprite != null ? _rightSprite : _frontSprite;
                        _sr.flipX = true;
                        break;
                    default:
                        _sr.sprite = _frontSprite;
                        break;
                }
            }
        }

        private List<Vector2Int> BuildRandomPath(int steps)
        {
            _pathBuffer.Clear();
            Vector2Int current = _gridPos;

            for (int i = 0; i < steps; i++)
            {
                var neighbors = _grid.GetFloorNeighbors(current.x, current.y);

                if (_pathBuffer.Count >= 1)
                    neighbors.Remove(_pathBuffer.Count >= 2 ? _pathBuffer[^2] : _gridPos);

                if (neighbors.Count == 0) break;

                Vector2Int next = neighbors[Random.Range(0, neighbors.Count)];
                _pathBuffer.Add(next);
                current = next;
            }

            return _pathBuffer;
        }

        private static CatDirection DeltaToDirection(Vector2Int delta)
        {
            if (Mathf.Abs(delta.y) >= Mathf.Abs(delta.x))
                return delta.y > 0 ? CatDirection.Back : CatDirection.Front;
            return delta.x > 0 ? CatDirection.Right : CatDirection.Left;
        }

        private static Vector3 CellToWorld(Vector2Int cell)
        {
            return new Vector3(cell.x + 0.5f, cell.y + 0.5f, 0f);
        }
    }
}
