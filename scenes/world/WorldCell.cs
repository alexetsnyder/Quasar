using Godot;
using Quasar.data.enums;

namespace Quasar.scenes.world
{
    public partial class WorldCell(TileType tileType, Vector2I atlasCoords, Color color)
    {
        public TileType TileType { get; set; } = tileType;

        public Vector2I AtlasCoords { get; set; } = atlasCoords;

        public Color Color { get; set; } = color;
    }
}