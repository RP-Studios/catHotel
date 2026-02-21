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
        [SerializeField, Range(0f, 1f)] private float _sleepChance = 0.15f;
        [SerializeField, Range(0f, 1f)] private float _eatChance   = 0.20f;
        [SerializeField, Range(0f, 1f)] private float _drinkChance = 0.15f;
        [SerializeField] private float _sleepTimeMin = 3f;
        [SerializeField] private float _sleepTimeMax = 5f;
        [SerializeField] private float _eatTimeMin   = 3f;
        [SerializeField] private float _eatTimeMax   = 5f;
        [SerializeField] private float _drinkTimeMin = 3f;
        [SerializeField] private float _drinkTimeMax = 5f;

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

            _currentDir = CatDirection.Front;
            EnterRest();
        }

        private void OnDestroy()
        {
            _moveSequence?.Kill();
        }

        // --- Rest: idle / sleep / eat / drink ---

        private void EnterRest()
        {
            _isWalking = false;
            _chosenRestState = null;

            float roll = Random.value;

            if (roll < _sleepChance)
            {
                StartRestActivity("Sleep", _currentDir, _sleepTimeMin, _sleepTimeMax);
            }
            else if (roll < _sleepChance + _eatChance)
            {
                StartRestActivity("Eat", _currentDir, _eatTimeMin, _eatTimeMax);
            }
            else if (roll < _sleepChance + _eatChance + _drinkChance)
            {
                StartRestActivity("Drink", _currentDir, _drinkTimeMin, _drinkTimeMax);
            }
            else
            {
                EnterIdle();
            }
        }

        private void StartRestActivity(string prefix, CatDirection dir, float timeMin, float timeMax)
        {
            // Sleep/Eat/Drink have front, left, right. Back → front fallback.
            _chosenRestState = dir == CatDirection.Back
                ? $"{prefix}_Front"
                : $"{prefix}_{dir}";

            PlayAnimState(_chosenRestState);

            float duration = Random.Range(timeMin, timeMax);
            DOVirtual.DelayedCall(duration, () =>
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
            PlayAnimState($"Walk_{dir}");
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
