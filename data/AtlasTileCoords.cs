using Godot;

namespace Quasar.data
{
    public static partial class AtlasTileCoords
    {
        public static Vector2I SOLID { get => new(11, 13); }

        public static Vector2I SOLID_WALL { get => new(1, 11); }

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

        public static string GetTileType(Vector2I atlasCoord)
        {
            if (atlasCoord == SOLID)
            {
                return "SOLID";
            }
            else if (atlasCoord == SOLID_WALL)
            {
                return "SOLID_WALL";
            }
            else if (atlasCoord == WATER)
            {
                return "WATER";
            }
            else if (atlasCoord == GRASSLAND_01 ||
                     atlasCoord == GRASSLAND_02 ||
                     atlasCoord == GRASSLAND_03)
            {
                return "GRASSLAND";
            }
            else if (atlasCoord == FOREST_01 ||
                     atlasCoord == FOREST_02 ||
                     atlasCoord == FOREST_03)
            {
                return "FOREST";
            }
            else if (atlasCoord == MOUNTAINS)
            {
                return "MOUNTAINS";
            }
            else if (atlasCoord == TALLER_MOUNTAINS)
            {
                return "TALLER MOUNTAINS";
            }
            else if (atlasCoord == HILLS_01 ||
                     atlasCoord == HILLS_02)
            {
                return "HILLS";
            }
            else if (atlasCoord == DIRT)
            {
                return "DIRT";
            }
            else if (atlasCoord == GRASS_01 ||
                     atlasCoord == GRASS_02 ||
                     atlasCoord == GRASS_03)
            {
                return "GRASS";
            }
            else
            {
                return "NONE";
            }
        }
    }
}