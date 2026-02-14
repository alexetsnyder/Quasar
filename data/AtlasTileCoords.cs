using Godot;

namespace Quasar.data
{
    public static partial class AtlasTileCoords
    {
        public static Vector2I WATER { get => new(7, 15); }

        public static Vector2I GRASS_01 { get => new(2, 2); }

        public static Vector2I GRASS_02 { get => new(12, 15); }

        public static Vector2I GRASS_03 { get => new(12, 15); }

        public static Vector2I TREE_01 { get => new(8, 1); }

        public static Vector2I TREE_02 { get => new(7, 1); }

        public static Vector2I TREE_03 { get => new(4, 15); }

        public static Vector2I MOUNTAIN { get =>  new(15, 7); }

        public static Vector2I TALLER_MOUNTAIN { get => new(14, 1); }

        public static Vector2I HILL_01 { get => new(15, 14); }

        public static Vector2I HILL_02 { get => new(14, 6); }
    }
}