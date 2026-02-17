using Godot;

namespace Quasar.scenes.world
{
    public partial class WorldCell(Vector2I atlasCoord, Color modulate, int alternateTile = 0)
    {
        public Vector2I AtlasCoord { get; set; } = atlasCoord;

        public Color Modulate { get; set; } = modulate;

        public int AlternateTile { get; set; } = alternateTile;
    }
}