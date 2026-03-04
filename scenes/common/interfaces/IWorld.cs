using Godot;
using System.Collections.Generic;

namespace Quasar.scenes.common.interfaces
{
    public interface IWorld
    {
        public int Rows { get; }

        public int Cols { get; }

        public bool IsInBounds(Vector2I coords);

        public bool IsSolid(Vector2I coords);

        public bool IsImpassable(Vector2I coords);

        public bool IsWater(Vector2I coords);

        public bool IsMineable(Vector2I coords);

        public bool IsTree(Vector2I coords);

        public bool IsGatherable(Vector2I coords);

        public bool HasItemsToHaul(Vector2I coords);

        public List<Vector2I> GetAllPoints();

        public List<Vector2> GetAdjacentTiles(Vector2 localPos, bool includeDiagonals = false);
    }
}