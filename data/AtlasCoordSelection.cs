using Godot;

namespace Quasar.data
{
    public static partial class AtlasCoordSelection
    {
        public static Vector2I DIG { get => new(0, 0); }

        public static Vector2I CANCEL {  get => new(2, 0); }

        public static Vector2I MIDDLE_SELECTION { get => new(1, 0); }

        public static Vector2I LEFT_SELECTION { get => new(0, 1); }

        public static Vector2I TOP_SELECTION { get => new(1, 1); }

        public static Vector2I RIGHT_SELECTION { get => new(0, 2); }

        public static Vector2I BOTTOM_SELECTION { get => new(1, 2); }

        public static Vector2I LEFT_TOP_SELECTION { get => new(0, 3); }

        public static Vector2I RIGHT_TOP_SELECTION { get => new(1, 3); }

        public static Vector2I LEFT_BOTTOM_SELECTION { get => new(0, 4); }

        public static Vector2I RIGHT_BOTTOM_SELECTION { get => new(1, 4); }
    }
}