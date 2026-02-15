using Godot;

namespace Quasar.data
{
    public static partial class ColorConstants
    {
        public static Color WHITE { get => new("#FFFFFF"); }

        public static Color BLACK { get => new("#000000"); }

        public static Color RED { get => new("#FF0000"); }

        public static Color BLUE { get => new("#0000FF"); }

        public static Color GREEN { get => new("#00FF00"); }

        public static Color YELLOW { get => new("#FFFF00"); }

        public static Color LAVENDER { get => new("#E6E6FA"); }

        public static Color ORANGE { get => new("#FFA500"); }

        public static Color AMBER { get => new("#FFBF00"); }

        public static Color FOREST_GREEN { get => new("#228B22"); }

        public static Color EMERALD_GREEN { get => new("#50C878");  }

        public static Color GREY { get => new("#808080"); }

        public static Color GRASS_GREEN { get => new("#7CFC00"); }

        public static Color WALL_PURPLE { get => new("#8361E0"); }

        public static string GetColorType(Color color)
        {
            if (color == WHITE)
            {
                return "WHITE";
            }
            else if (color == BLACK)
            {
                return "BLACK";
            }
            else if (color == RED)
            {
                return "RED";
            }
            else if (color == BLUE)
            {
                return "BLUE";
            }
            else if (color == GREEN)
            {
                return "GREEN";
            }
            else if (color == YELLOW)
            {
                return "YELLOW";
            }
            else if (color == LAVENDER)
            {
                return "LAVENDER";
            }
            else if (color == ORANGE)
            {
                return "ORANGE";
            }
            else if (color == AMBER)
            {
                return "AMBER";
            }
            else if (color == FOREST_GREEN)
            {
                return "FOREST GREEN";
            }
            else if (color == EMERALD_GREEN)
            {
                return "EMERALD GREEN";
            }
            else if (color == GREY)
            {
                return "GREY";
            }
            else if (color == WALL_PURPLE)
            {
                return "WALL PURPLE";
            }
            else if (color == GRASS_GREEN)
            {
                return "GRASS GREEN";
            }
            else
            {
                return "NONE";
            }
        }
    }
}