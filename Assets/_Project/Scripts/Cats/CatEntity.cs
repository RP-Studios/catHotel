using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using CatHotel.Grid;

namespace CatHotel.Cats
{
    public enum CatDirection { Front, Back, Right, Left }

    public class CatEntity : MonoBehaviour
    {
        [Header("Sprites")]
        [SerializeField] private Sprite _frontSprite;
        [SerializeField] private Sprite _rightSprite;
        [SerializeField] private Sprite _backSprite;

        [Header("Movement")]
        [SerializeField] private float _cellMoveTime   = 0.7f;
        [SerializeField] private float _idleTimeMin    = 1f;
        [SerializeField] private float _idleTimeMax    = 3f;
        [SerializeField] private int   _wanderStepsMin = 2;
        [SerializeField] private int   _wanderStepsMax = 6;

        [Header("Sleep")]
        [SerializeField, Range(0f, 1f)] private float _sleepChance = 0.3f;
        [SerializeField] private float _sleepTimeMin = 3f;
        [SerializeField] private float _sleepTimeMax = 5f;

        private static readonly string[][] IdleStates =
        {
            new[] { "Idle3_Front", "Idle2_Front" }, // Front
            new[] { "Idle3_Back" },                  // Back
            new[] { "Idle3_Right", "Idle2_Right" },  // Right
            new[] { "Idle3_Left", "Idle2_Left" },    // Left
        };

        private SpriteRenderer _sr;
        private Animator _animator;
        private GridData _grid;
        private Vector2Int _gridPos;
        private Sequence _moveSequence;
        private CatDirection _currentDir;
        private bool _isWalking;
        private string _chosenRestState;

        public Vector2Int GridPos => _gridPos;

        public void SetSprites(Sprite front, Sprite right, Sprite back)
        {
            _frontSprite = front;
            _rightSprite = right;
            _backSprite = back;
        }

        public void Init(GridData grid, Vector2Int startCell)
        {
            _grid = grid;
            _gridPos = startCell;
            _sr = GetComponent<SpriteRenderer>();
            _animator = GetComponent<Animator>();
            transform.position = CellToWorld(startCell);

            EnterRest();
        }

        private void OnDestroy()
        {
            _moveSequence?.Kill();
        }

        // --- Rest state: idle or sleep ---

        private void EnterRest()
        {
            _isWalking = false;
            _chosenRestState = null;

            bool willSleep = Random.value < _sleepChance;

            if (willSleep)
            {
                _chosenRestState = "Sleep_Front";
                _currentDir = CatDirection.Front;
                PlayAnimState(_chosenRestState);

                float sleepTime = Random.Range(_sleepTimeMin, _sleepTimeMax);
                DOVirtual.DelayedCall(sleepTime, () =>
                {
                    // Wake up → idle briefly then wander
                    _chosenRestState = null;
                    EnterIdle();
                });
            }
            else
            {
                EnterIdle();
            }
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
            DOVirtual.DelayedCall(idleTime, Wander);
        }

        // --- Movement ---

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

            string walkState = dir switch
            {
                CatDirection.Front => "Walk_Front",
                CatDirection.Back  => "Walk_Back",
                _ => null
            };

            if (walkState != null && _animator != null)
            {
                _sr.flipX = false;
                if (!_animator.enabled) _animator.enabled = true;
                _animator.Play(walkState, 0, 0f);
                return;
            }

            // No walk animation for this direction: static sprite
            if (_animator != null && _animator.enabled)
                _animator.enabled = false;
            _sr.flipX = (dir == CatDirection.Left);
            _sr.sprite = _rightSprite;
        }

        // --- Helpers ---

        private void PlayAnimState(string stateName)
        {
            if (_animator == null) return;
            _sr.flipX = false;
            if (!_animator.enabled) _animator.enabled = true;
            _animator.Play(stateName, 0, 0f);
        }

        private List<Vector2Int> BuildRandomPath(int steps)
        {
            var path = new List<Vector2Int>(steps);
            Vector2Int current = _gridPos;

            for (int i = 0; i < steps; i++)
            {
                var neighbors = _grid.GetFloorNeighbors(current.x, current.y);

                // Avoid immediate backtrack
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
            // Prioritize vertical if equal
            if (Mathf.Abs(delta.y) >= Mathf.Abs(delta.x))
                return delta.y > 0 ? CatDirection.Back : CatDirection.Front;
            return delta.x > 0 ? CatDirection.Right : CatDirection.Left;
        }

        private static Vector3 CellToWorld(Vector2Int cell)
        {
            // Cell center: grid cells have their origin at bottom-left,
            // so center is at +0.5
            return new Vector3(cell.x + 0.5f, cell.y + 0.5f, 0f);
        }
    }
}
