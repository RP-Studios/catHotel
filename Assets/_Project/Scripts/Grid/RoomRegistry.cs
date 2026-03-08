using System.Collections.Generic;
using UnityEngine;

namespace CatHotel.Grid
{
    public class RoomRegistry
    {
        private readonly GridData _grid;
        private readonly List<RoomData> _rooms = new();
        private int _nextId = 1;

        public IReadOnlyList<RoomData> Rooms => _rooms;

        /// <summary>
        /// Register a pre-built room (cells already set in GridData).
        /// </summary>
        public RoomData RegisterRoom(RectInt rect)
        {
            var room = new RoomData(_nextId++, rect);
            _rooms.Add(room);
            return room;
        }

        public RoomRegistry(GridData grid)
        {
            _grid = grid;
        }

        public bool CanCreateRoom(RectInt rect)
        {
            if (rect.width < 3 || rect.height < 3)
                return false;

            if (rect.xMin < 0 || rect.yMin < 0 ||
                rect.xMax > GridData.Width || rect.yMax > GridData.Height)
                return false;

            if (_grid.DoesInteriorOverlapFloor(rect))
                return false;

            // Extension-only: new room must share at least one wall with existing structure
            if (!SharesWallWithExisting(rect))
                return false;

            return true;
        }

        private bool SharesWallWithExisting(RectInt rect)
        {
            // Check if any perimeter cell of the new room overlaps existing structure
            // (Wall or Floor, since left/right edges are Floor not Wall)
            for (int x = rect.xMin; x < rect.xMax; x++)
            {
                if (_grid.GetCell(x, rect.yMin) != CellType.Empty) return true;
                if (_grid.GetCell(x, rect.yMax - 1) != CellType.Empty) return true;
            }
            for (int y = rect.yMin + 1; y < rect.yMax - 1; y++)
            {
                if (_grid.GetCell(rect.xMin, y) != CellType.Empty) return true;
                if (_grid.GetCell(rect.xMax - 1, y) != CellType.Empty) return true;
            }
            return false;
        }

        public RoomData TryCreateRoom(RectInt rect)
        {
            if (!CanCreateRoom(rect))
                return null;

            var room = new RoomData(_nextId++, rect);

            for (int y = rect.yMin; y < rect.yMax; y++)
            {
                for (int x = rect.xMin; x < rect.xMax; x++)
                {
                    bool isTopOrBottom = y == rect.yMin || y == rect.yMax - 1;

                    if (isTopOrBottom)
                    {
                        CellType existing = _grid.GetCell(x, y);
                        // Merge: existing structure becomes floor, new edge becomes wall
                        _grid.SetCell(x, y, existing != CellType.Empty
                            ? CellType.Floor
                            : CellType.Wall);
                    }
                    else
                    {
                        _grid.SetCell(x, y, CellType.Floor);
                    }
                }
            }

            _rooms.Add(room);
            return room;
        }
    }
}
