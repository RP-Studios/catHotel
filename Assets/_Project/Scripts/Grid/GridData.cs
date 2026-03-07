using System.Collections.Generic;
using UnityEngine;

namespace CatHotel.Grid
{
    public class GridData
    {
        private static readonly Vector2Int[] Directions =
        {
            Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right
        };

        public const int Width  = 48;
        public const int Height = 32;

        private readonly CellType[] _cells;

        public GridData()
        {
            _cells = new CellType[Width * Height];
        }

        public bool InBounds(int x, int y)
            => x >= 0 && x < Width && y >= 0 && y < Height;

        public CellType GetCell(int x, int y)
        {
            if (!InBounds(x, y)) return CellType.Empty;
            return _cells[y * Width + x];
        }

        public void SetCell(int x, int y, CellType type)
        {
            if (!InBounds(x, y)) return;
            _cells[y * Width + x] = type;
        }

        public bool IsWalkable(int x, int y)
        {
            var c = GetCell(x, y);
            return c == CellType.Floor || c == CellType.Door;
        }

        public List<Vector2Int> GetFloorNeighbors(int x, int y)
        {
            var result = new List<Vector2Int>(4);
            foreach (var dir in Directions)
            {
                int nx = x + dir.x;
                int ny = y + dir.y;
                if (IsWalkable(nx, ny))
                    result.Add(new Vector2Int(nx, ny));
            }
            return result;
        }

        public List<Vector2Int> GetAllFloorCells()
        {
            var result = new List<Vector2Int>();
            for (int y = 0; y < Height; y++)
                for (int x = 0; x < Width; x++)
                    if (IsWalkable(x, y))
                        result.Add(new Vector2Int(x, y));
            return result;
        }

        /// <summary>
        /// BFS pathfinding from start to target. Returns path including start and target,
        /// or null if no path exists.
        /// </summary>
        public List<Vector2Int> FindPath(Vector2Int start, Vector2Int target)
        {
            if (!IsWalkable(start.x, start.y) || !IsWalkable(target.x, target.y))
                return null;
            if (start == target)
                return new List<Vector2Int> { start };

            var queue = new Queue<Vector2Int>();
            var cameFrom = new Dictionary<Vector2Int, Vector2Int>();
            queue.Enqueue(start);
            cameFrom[start] = start;

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                if (current == target)
                {
                    var path = new List<Vector2Int>();
                    var c = target;
                    while (c != start)
                    {
                        path.Add(c);
                        c = cameFrom[c];
                    }
                    path.Add(start);
                    path.Reverse();
                    return path;
                }

                foreach (var dir in Directions)
                {
                    var next = new Vector2Int(current.x + dir.x, current.y + dir.y);
                    if (IsWalkable(next.x, next.y) && !cameFrom.ContainsKey(next))
                    {
                        cameFrom[next] = current;
                        queue.Enqueue(next);
                    }
                }
            }
            return null;
        }

        public bool DoesInteriorOverlapFloor(RectInt rect)
        {
            for (int y = rect.yMin + 1; y < rect.yMax - 1; y++)
                for (int x = rect.xMin + 1; x < rect.xMax - 1; x++)
                    if (IsWalkable(x, y))
                        return true;
            return false;
        }
    }
}
