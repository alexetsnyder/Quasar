using Godot;
using Quasar.data.enums;
using Quasar.scenes.systems.building;
using System.Collections.Generic;

namespace Quasar.scenes.common.interfaces
{
    public interface IWorld
    {
        public int Rows { get; }

        public int Cols { get; }

        public int GetWorldCellId(Vector2 localPos);

        public TileType GetTileType(Vector2 localPos);

        public Color? GetTileColor(Vector2 localPos);

        public bool IsInBounds(Vector2I coords);

        public bool IsSolid(Vector2I coords);

        public bool IsImpassable(Vector2 localPos);

        public bool IsImpassable(Vector2I coords);

        public bool IsWater(Vector2I coords);

        public bool IsMineable(Vector2I coords);

        public bool IsTree(Vector2I coords);

        public bool IsGatherable(Vector2I coords);

        public bool HasItemsToHaul(Vector2I coords);

        public List<Vector2I> GetAllPoints();

        public List<Vector2> GetAdjacentTiles(Vector2 localPos, bool includeDiagonals = false);

        public void Mine(Vector2 localPos);

        public void Build(Vector2 localPos, Buildable buildable);

        public void Cut(Vector2 localPos);

        public void Till(Vector2 localPos);

        public void Gather(Vector2 localPos);

        public void Fish(Vector2 localPos);
    }
}