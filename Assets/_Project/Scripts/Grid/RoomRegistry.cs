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

            return true;
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
                    bool isPerimeter = x == rect.xMin || x == rect.xMax - 1 ||
                                       y == rect.yMin || y == rect.yMax - 1;

                    if (isPerimeter)
                    {
                        CellType existing = _grid.GetCell(x, y);
                        // Wall merge: shared wall becomes floor
                        _grid.SetCell(x, y, existing == CellType.Wall
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
