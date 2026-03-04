using Godot;
using Quasar.data.enums;

namespace Quasar.scenes.work
{
    public partial class Buildable(TileType tileType, Vector2I atlasCoords, Color color) : Resource
    {
        public TileType TileType { get; set; } = tileType;

        public Vector2I AtlasCoords { get; set; } = atlasCoords;

        public Color Color { get; set; } = color;
    }
}

