using Godot;
using Quasar.data.enums;
using System.Collections.Generic;

namespace Quasar.data
{
    public static partial class AtlasConstants
    {
        public static Dictionary<TileType, List<Vector2I>> AtlasCoords { get; set; } = new Dictionary<TileType, List<Vector2I>>
        {
            { TileType.SOLID, [ new(11, 13) ] },
            { TileType.NATURAL_WALL, [ new(1, 11) ] },
            { TileType.WALL, [ new(10, 11), new(10, 12), new(13, 12), new(9, 12), new(2, 11) ] },
            { TileType.GRASS, [ new(10, 15), new(12, 2), new(0, 6), new(7, 2) ] },
            { TileType.DIRT, [ new(7, 15) ] },
            { TileType.WATER, [ new(7, 15) ] },
            { TileType.TILLED, [ new(7, 15) ] },
            { TileType.GRASSLAND, [ new(2, 2), new(12, 15), new(7, 14) ] },
            { TileType.FOREST, [ new(8, 1), new(7, 1), new(4, 15) ] },
            { TileType.MOUNTAINS, [ new(15, 7), new(14, 1) ] },
            { TileType.HILLS, [ new(15, 14), new(14, 6) ] },

        };

        public static Dictionary<TileType, List<Color>> Colors { get; set; } = new Dictionary<TileType, List<Color>>
        {
            { TileType.SOLID, [ ColorConstants.BLACK ] },
            { TileType.NATURAL_WALL, [ ColorConstants.WALL_PURPLE ] },
            { TileType.WALL, [ ColorConstants.GREY ] },
            { TileType.GRASS, [ ColorConstants.RED, ColorConstants.GREEN, ColorConstants.GRASS_GREEN, ColorConstants.YELLOW,
                                ColorConstants.ORANGE, ColorConstants.AMBER ] },
            { TileType.DIRT, [ ColorConstants.BURNT_ORANGE ] },
            { TileType.WATER, [ ColorConstants.BLUE ] },
            { TileType.TILLED, [ ColorConstants.LAVENDER ] },
            { TileType.GRASSLAND, [ ColorConstants.GRASS_GREEN ] },
            { TileType.FOREST, [ ColorConstants.FOREST_GREEN ] },
            { TileType.MOUNTAINS, [ ColorConstants.GREY ] },
            { TileType.HILLS, [ ColorConstants.EMERALD_GREEN ] },
        };
    }
}