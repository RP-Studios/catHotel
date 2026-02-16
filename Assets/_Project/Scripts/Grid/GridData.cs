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

        public const int Width  = 24;
        public const int Height = 16;

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

        public List<Vector2Int> GetFloorNeighbors(int x, int y)
        {
            var result = new List<Vector2Int>(4);
            foreach (var dir in Directions)
            {
                int nx = x + dir.x;
                int ny = y + dir.y;
                if (GetCell(nx, ny) == CellType.Floor)
                    result.Add(new Vector2Int(nx, ny));
            }
            return result;
        }

        public List<Vector2Int> GetAllFloorCells()
        {
            var result = new List<Vector2Int>();
            for (int y = 0; y < Height; y++)
                for (int x = 0; x < Width; x++)
                    if (GetCell(x, y) == CellType.Floor)
                        result.Add(new Vector2Int(x, y));
            return result;
        }

        public bool DoesInteriorOverlapFloor(RectInt rect)
        {
            for (int y = rect.yMin + 1; y < rect.yMax - 1; y++)
                for (int x = rect.xMin + 1; x < rect.xMax - 1; x++)
                    if (GetCell(x, y) == CellType.Floor)
                        return true;
            return false;
        }
    }
}
