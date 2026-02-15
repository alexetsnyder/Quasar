using Godot;

namespace Quasar.data
{
    public static partial class AtlasTileCoords
    {
        public static Vector2I SOLID { get => new(11, 13); }

        public static Vector2I WATER { get => new(7, 15); }

        public static Vector2I GRASSLAND_01 { get => new(2, 2); }

        public static Vector2I GRASSLAND_02 { get => new(12, 15); }

        public static Vector2I GRASSLAND_03 { get => new(12, 15); }

        public static Vector2I FOREST_01 { get => new(8, 1); }

        public static Vector2I FOREST_02 { get => new(7, 1); }

        public static Vector2I FOREST_03 { get => new(4, 15); }

        public static Vector2I MOUNTAINS { get =>  new(15, 7); }

        public static Vector2I TALLER_MOUNTAINS { get => new(14, 1); }

        public static Vector2I HILLS_01 { get => new(15, 14); }

        public static Vector2I HILLS_02 { get => new(14, 6); }

        public static Vector2I DIRT {  get => new(10, 15); }

        public static Vector2I GRASS_01 { get => new(12, 2); }

        public static Vector2I GRASS_02 { get => new(0, 6); }

        public static Vector2I GRASS_03 { get => new(7, 2); }
    }
}