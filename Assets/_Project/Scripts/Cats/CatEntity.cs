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
        [SerializeField] private float _cellMoveTime = 0.35f;
        [SerializeField] private float _idleTimeMin  = 1f;
        [SerializeField] private float _idleTimeMax   = 3f;
        [SerializeField] private int   _wanderStepsMin = 2;
        [SerializeField] private int   _wanderStepsMax = 6;

        private SpriteRenderer _sr;
        private GridData _grid;
        private Vector2Int _gridPos;
        private Sequence _moveSequence;

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
            transform.position = CellToWorld(startCell);

            SetDirection(CatDirection.Front);
            ScheduleNextWander();
        }

        private void OnDestroy()
        {
            _moveSequence?.Kill();
        }

        private void ScheduleNextWander()
        {
            float idleTime = Random.Range(_idleTimeMin, _idleTimeMax);
            DOVirtual.DelayedCall(idleTime, Wander);
        }

        private void Wander()
        {
            int steps = Random.Range(_wanderStepsMin, _wanderStepsMax + 1);
            var path = BuildRandomPath(steps);

            if (path.Count == 0)
            {
                ScheduleNextWander();
                return;
            }

            _moveSequence = DOTween.Sequence();

            for (int i = 0; i < path.Count; i++)
            {
                Vector2Int target = path[i];
                Vector2Int from = i == 0 ? _gridPos : path[i - 1];
                Vector2Int delta = target - from;

                // Change sprite direction at the start of each step
                CatDirection dir = DeltaToDirection(delta);
                _moveSequence.AppendCallback(() => SetDirection(dir));

                Vector3 worldTarget = CellToWorld(target);
                _moveSequence.Append(
                    transform.DOMove(worldTarget, _cellMoveTime)
                        .SetEase(Ease.Linear));
            }

            Vector2Int finalCell = path[^1];
            _moveSequence.OnComplete(() =>
            {
                _gridPos = finalCell;
                ScheduleNextWander();
            });
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

        private void SetDirection(CatDirection dir)
        {
            _sr.flipX = false;

            switch (dir)
            {
                case CatDirection.Front:
                    _sr.sprite = _frontSprite;
                    break;
                case CatDirection.Back:
                    _sr.sprite = _backSprite;
                    break;
                case CatDirection.Right:
                    _sr.sprite = _rightSprite;
                    break;
                case CatDirection.Left:
                    _sr.sprite = _rightSprite;
                    _sr.flipX = true;
                    break;
            }
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
