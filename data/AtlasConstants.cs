using Godot;
using Catcophony.data.enums;
using System.Collections.Generic;

namespace Catcophony.data
{
    public static partial class AtlasConstants
    {
        public static Dictionary<TileType, List<Vector2I>> AtlasCoords { get; set; } = new Dictionary<TileType, List<Vector2I>>
        {
            { TileType.SOLID, [ new(11, 13) ] },
            { TileType.NATURAL_WALL, [ new(1, 11) ] },

            { TileType.WALL, [ new(10, 11), new(13, 12) ] },
            { TileType.CORNER_WALL, [ new(11, 11), new(12, 11), new(8, 12), new(9, 12),  ] },
            { TileType.THREE_CONNECT_WALL, [ new(9, 11), new(10, 12), new(11, 12), new(12, 12) ] },
            { TileType.FOUR_CONNECT_WALL, [ new(14, 12) ] },
            { TileType.STORAGE, [ new(12, 14) ] },

            { TileType.STONE, [ new(9, 15) ] },
            { TileType.WOOD, [ new(13, 3) ] },
            { TileType.TREE, [ new(15, 4) ] },

            { TileType.GRASS, [ new(10, 15), new(12, 2), new(0, 6), new(7, 2) ] },
            { TileType.DIRT, [ new(7, 15) ] },
            { TileType.WATER, [ new(7, 15) ] },
            { TileType.TILLED, [ new(7, 15) ] },
            { TileType.GRASSLAND, [ new(2, 2), new(12, 15), new(7, 14) ] },
            { TileType.FOREST, [ new(8, 1), new(7, 1), new(4, 15) ] },
            { TileType.MOUNTAINS, [ new(15, 7), new(14, 1) ] },
            { TileType.HILLS, [ new(15, 14), new(14, 6) ] },

            { TileType.MINE, [ new(0, 0) ] },
            { TileType.BUILD, [ new(2, 3) ] },
            { TileType.TILL, [ new(2, 1) ] },
            { TileType.FISH, [ new(2, 2) ] },
            { TileType.CUT, [ new(2, 4) ] },
            { TileType.HAUL, [ new(3, 1) ] },
            { TileType.GATHER, [ new(3, 0) ] },

            { TileType.CANCEL, [ new(2, 0) ] },
            { TileType.SELECTION, [ new(1, 0), new(0, 1), new(1, 1),
                                    new(0, 2), new(1, 2), new(0, 3),
                                    new(1, 3), new(0, 4), new(1, 4) ] },

            { TileType.REGION, [ new(0, 0), new(1, 0), new(0, 1), new(1, 1),
                               new(0, 2), new(1, 2), new(0, 3), new(1, 3) ] },
        };

        public static Dictionary<TileType, List<Color>> Colors { get; set; } = new Dictionary<TileType, List<Color>>
        {
            { TileType.SOLID, [ ColorConstants.BLACK ] },
            { TileType.NATURAL_WALL, [ ColorConstants.WALL_PURPLE ] },

            { TileType.WALL, [ ColorConstants.GREY ] },
            { TileType.CORNER_WALL, [ ColorConstants.GREY ] },
            { TileType.THREE_CONNECT_WALL, [ ColorConstants.GREY ] },
            { TileType.FOUR_CONNECT_WALL, [ ColorConstants.GREY ] },
            { TileType.STORAGE, [ ColorConstants.GREY ] },

            { TileType.STONE, [ ColorConstants.GREY ] },
            { TileType.WOOD, [ ColorConstants.BIRCH_WOOD, ColorConstants.OAK_WOOD, ColorConstants.SPRUCE_WOOD,
                               ColorConstants.DARK_OAK_WOOD, ColorConstants.JUNGLE_WOOD] },
            { TileType.TREE, [ ColorConstants.BIRCH_WOOD, ColorConstants.OAK_WOOD, ColorConstants.SPRUCE_WOOD,
                               ColorConstants.DARK_OAK_WOOD, ColorConstants.JUNGLE_WOOD] },

            { TileType.GRASS, [ ColorConstants.RED, ColorConstants.GREEN, ColorConstants.GRASS_GREEN, 
                                ColorConstants.YELLOW, ColorConstants.ORANGE, ColorConstants.AMBER ] },
            { TileType.DIRT, [ ColorConstants.BURNT_ORANGE ] },
            { TileType.WATER, [ ColorConstants.BLUE ] },
            { TileType.TILLED, [ ColorConstants.LAVENDER ] },
            { TileType.GRASSLAND, [ ColorConstants.GRASS_GREEN ] },
            { TileType.FOREST, [ ColorConstants.FOREST_GREEN ] },
            { TileType.MOUNTAINS, [ ColorConstants.GREY ] },
            { TileType.HILLS, [ ColorConstants.EMERALD_GREEN ] },

            { TileType.MINE, [ ColorConstants.GREY ] },
            { TileType.BUILD, [ ColorConstants.GREY ] },
            { TileType.TILL, [ ColorConstants.GREY ] },
            { TileType.FISH, [ ColorConstants.ORANGE ] },
            { TileType.CUT, [ ColorConstants.GREY ] },
            { TileType.HAUL, [ ColorConstants.GREY ] },
            { TileType.GATHER, [ ColorConstants.GREY ] },

            { TileType.CANCEL, [ ColorConstants.WARNING_RED ] },
            { TileType.SELECTION, [] },

            { TileType.REGION, [ ColorConstants.GREEN ]  },
        };

        public static Vector2I GetAtlasCoords(TileType tileType, int index = 0)
        {
            return AtlasCoords[tileType][index];
        }

        public static Color GetColor(TileType tileType, int index = 0)
        {
            return Colors[tileType][index];
        }
    }
}