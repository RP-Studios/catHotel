using UnityEngine;

namespace CatHotel.Grid
{
    public class RoomData
    {
        public int Id { get; }
        public RectInt Bounds { get; }

        public int InteriorWidth  => Bounds.width - 2;
        public int InteriorHeight => Bounds.height - 2;
        public int TotalCells     => Bounds.width * Bounds.height;
        public int InteriorCells  => InteriorWidth * InteriorHeight;

        public RoomData(int id, RectInt bounds)
        {
            Id = id;
            Bounds = bounds;
        }
    }
}
