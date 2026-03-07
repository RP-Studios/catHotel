using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using CatHotel.Grid;

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

        [Header("Rest Activities")]
        [SerializeField, Range(0f, 1f)] private float _sleepChance = 0.12f;
        [SerializeField, Range(0f, 1f)] private float _eatChance   = 0.15f;
        [SerializeField, Range(0f, 1f)] private float _drinkChance = 0.12f;
        [SerializeField, Range(0f, 1f)] private float _cleanChance = 0.12f;
        [SerializeField, Range(0f, 1f)] private float _playChance  = 0.12f;
        [SerializeField, Range(0f, 1f)] private float _fightChance = 0.15f;
        [SerializeField] private float _sleepTimeMin = 3f;
        [SerializeField] private float _sleepTimeMax = 5f;
        [SerializeField] private float _eatTimeMin   = 3f;
        [SerializeField] private float _eatTimeMax   = 5f;
        [SerializeField] private float _drinkTimeMin = 3f;
        [SerializeField] private float _drinkTimeMax = 5f;
        [SerializeField] private float _cleanTimeMin = 3f;
        [SerializeField] private float _cleanTimeMax = 5f;
        [SerializeField] private float _playTimeMin  = 3f;
        [SerializeField] private float _playTimeMax  = 5f;

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
        private Vector2Int _gridPos;
        private Sequence _moveSequence;
        private Tween _pendingAction;
        private CatDirection _currentDir;
        private bool _isWalking;
        private bool _isFighting;
        private string _chosenRestState;

        public Vector2Int GridPos => _gridPos;

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
            transform.position = CellToWorld(startCell);

            _currentDir = CatDirection.Front;
            EnterRest();
        }

        private void OnDestroy()
        {
            _moveSequence?.Kill();
            _pendingAction?.Kill();
        }

        // --- Emotes (one-shot, triggered externally) ---

        /// <summary>Play happy animation once (2s), then return to idle.</summary>
        public void PlayHappy() => PlayEmote("Happy", 2f);

        /// <summary>Play unhappy animation once (1s), then return to idle.</summary>
        public void PlayUnhappy() => PlayEmote("Unhappy", 1f);

        /// <summary>Play petting animation (2s) with hand overlay, then return to idle.</summary>
        public void PlayPetting(RuntimeAnimatorController handController)
        {
            if (_isFighting) return;

            _moveSequence?.Kill();
            _pendingAction?.Kill();
            _isWalking = false;
            _chosenRestState = null;

            // Face front for petting
            _currentDir = CatDirection.Front;
            PlayAnimState("Pet_Front");

            // Spawn hand overlay above cat
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

        private void PlayEmote(string prefix, float duration)
        {
            if (_isFighting) return;
            // Interrupt whatever the cat is doing
            _moveSequence?.Kill();
            _pendingAction?.Kill();
            _isWalking = false;
            _chosenRestState = null;

            string state = DirectionalState(prefix, _currentDir);
            PlayAnimState(state);

            _pendingAction = DOVirtual.DelayedCall(duration, () =>
            {
                _chosenRestState = null;
                EnterIdle();
            });
        }

        // --- Combat ---

        public bool IsFighting => _isFighting;
        public SpriteRenderer SpriteRenderer => _sr;

        private bool CanFight()
        {
            if (_animator == null || _isFighting) return false;
            return _animator.HasState(0, Animator.StringToHash("Fight_In_Left"));
        }

        private bool TryInitiateFight()
        {
            if (_spawner == null || !CanFight()) return false;
            if (Random.value >= _fightChance) return false;

            // Check left and right neighbors
            var leftPos = new Vector2Int(_gridPos.x - 1, _gridPos.y);
            var rightPos = new Vector2Int(_gridPos.x + 1, _gridPos.y);

            var neighbor = _spawner.GetCatAt(leftPos, this);
            if (neighbor == null || !neighbor.CanFight())
                neighbor = _spawner.GetCatAt(rightPos, this);
            if (neighbor == null || !neighbor.CanFight())
                return false;

            // Determine who is left and who is right
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
        }

        private void RunFightSequence(CatEntity left, CatEntity right)
        {
            left.EnterFight();
            right.EnterFight();

            var seq = DOTween.Sequence();
            GameObject cloudGo = null;

            // Phase 1: Fight In (2s)
            seq.AppendCallback(() =>
            {
                left._currentDir = CatDirection.Right;
                right._currentDir = CatDirection.Left;
                left.PlayAnimState("Fight_In_Right");
                right.PlayAnimState("Fight_In_Left");
            });
            seq.AppendInterval(2f);

            // Phase 2: Hide cats, show cloud (10s)
            seq.AppendCallback(() =>
            {
                left._sr.enabled = false;
                right._sr.enabled = false;

                Vector3 midpoint = (left.transform.position + right.transform.position) / 2f;
                cloudGo = new GameObject("FightCloud");
                cloudGo.transform.position = midpoint;

                var cloudSr = cloudGo.AddComponent<SpriteRenderer>();
                cloudSr.sortingOrder = 15;

                var cloudAnim = cloudGo.AddComponent<Animator>();
                cloudAnim.runtimeAnimatorController = _spawner.FightCloudController;
            });
            seq.AppendInterval(10f);

            // Phase 3: Destroy cloud, show cats, Fight Out (1s)
            seq.AppendCallback(() =>
            {
                if (cloudGo != null) Object.Destroy(cloudGo);

                left._sr.enabled = true;
                right._sr.enabled = true;

                left.PlayAnimState("Fight_Out_Right");
                right.PlayAnimState("Fight_Out_Left");
            });
            seq.AppendInterval(1f);

            // Phase 4: Return to normal
            seq.AppendCallback(() =>
            {
                left._isFighting = false;
                right._isFighting = false;
                left.EnterRest();
                right.EnterRest();
            });
        }

        // --- Rest: idle / sleep / eat / drink / clean ---

        private void EnterRest()
        {
            _isWalking = false;
            _chosenRestState = null;

            if (TryInitiateFight()) return;

            float roll = Random.value;
            float cursor = 0f;

            cursor += _sleepChance;
            if (roll < cursor)
            {
                StartRestActivity("Sleep", _sleepTimeMin, _sleepTimeMax);
                return;
            }

            cursor += _eatChance;
            if (roll < cursor)
            {
                StartRestActivity("Eat", _eatTimeMin, _eatTimeMax);
                return;
            }

            cursor += _drinkChance;
            if (roll < cursor)
            {
                StartRestActivity("Drink", _drinkTimeMin, _drinkTimeMax);
                return;
            }

            cursor += _cleanChance;
            if (roll < cursor)
            {
                StartRestActivity("Clean", _cleanTimeMin, _cleanTimeMax);
                return;
            }

            cursor += _playChance;
            if (roll < cursor)
            {
                StartRestActivity("Play", _playTimeMin, _playTimeMax);
                return;
            }

            EnterIdle();
        }

        private void StartRestActivity(string prefix, float timeMin, float timeMax)
        {
            _chosenRestState = DirectionalState(prefix, _currentDir);
            PlayAnimState(_chosenRestState);

            float duration = Random.Range(timeMin, timeMax);
            _pendingAction = DOVirtual.DelayedCall(duration, () =>
            {
                _chosenRestState = null;
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
            _pendingAction = DOVirtual.DelayedCall(idleTime, Wander);
        }

        // --- Targeted Movement (BFS pathfinding) ---

        /// <summary>
        /// Walk to a specific target cell using BFS pathfinding.
        /// On arrival, enter normal rest/wander behavior.
        /// </summary>
        public void WalkToTarget(Vector2Int target)
        {
            if (_isFighting) return;
            _moveSequence?.Kill();
            _pendingAction?.Kill();
            _isWalking = false;
            _chosenRestState = null;

            var path = _grid.FindPath(_gridPos, target);
            if (path == null || path.Count < 2)
            {
                EnterRest();
                return;
            }

            _isWalking = true;
            _moveSequence = DOTween.Sequence();

            // Skip index 0 (current position)
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
                EnterRest();
            });
        }

        // --- Random Movement ---

        private void Wander()
        {
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
            PlayAnimState($"Walk_{dir}");
        }

        // --- Helpers ---

        /// <summary>Returns "{prefix}_Front/Left/Right". Back → Front fallback.</summary>
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
                // No anim for this state → show static sprite
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
            var path = new List<Vector2Int>(steps);
            Vector2Int current = _gridPos;

            for (int i = 0; i < steps; i++)
            {
                var neighbors = _grid.GetFloorNeighbors(current.x, current.y);

                if (path.Count >= 1)
                    neighbors.Remove(path.Count >= 2 ? path[^2] : _gridPos);

                if (neighbors.Count == 0) break;

                Vector2Int next = neighbors[Random.Range(0, neighbors.Count)];
                path.Add(next);
                current = next;
            }

            return path;
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
