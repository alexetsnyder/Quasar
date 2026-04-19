using Godot;
using Catcophony.data.enums;
using Catcophony.scenes.systems.building;
using Catcophony.scenes.systems.items;
using System.Collections.Generic;

namespace Catcophony.scenes.common.interfaces
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

        public Vector2? SearchForNearest(Vector2 localPos, TileType tileType);

        public Vector2? TryGetWanderingPoint(int tries);

        public List<Vector2> GetAdjacentTiles(Vector2 localPos, bool includeDiagonals = false);

        public ItemMaterial Mine(Vector2 localPos);

        public void Build(Vector2 localPos, Buildable buildable);

        public ItemMaterial Cut(Vector2 localPos);

        public void Till(Vector2 localPos);

        public void Gather(Vector2 localPos);

        public void Fish(Vector2 localPos);
    }
}