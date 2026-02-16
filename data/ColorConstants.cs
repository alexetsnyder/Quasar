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

        public static Color BURNT_ORANGE { get => new("#CC5500"); }

        public static string GetColorStrReflection(Color color)
        {
            var type = typeof(ColorConstants);

            foreach (var property in type.GetProperties())
            {
                var getter = property.GetGetMethod();
                if (getter != null && (Color)getter.Invoke(null, null) == color)
                {
                    return property.Name;
                }
            }

            return "NONE";
        }
    }
}