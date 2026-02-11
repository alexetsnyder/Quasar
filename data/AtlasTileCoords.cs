using Godot;

namespace Quasar.data
{
    public static partial class AtlasTileCoords
    {
        public static Vector2I WATER { get => new Vector2I(7, 15); }

        public static Vector2I GRASS { get => new Vector2I(2, 2); }

        public static Vector2I TREE { get => new Vector2I(8, 1); }

        public static Vector2I MOUNTAIN { get =>  new Vector2I(15, 7); }

        public static Vector2I HILL { get => new Vector2I(15, 14); }
    }
}