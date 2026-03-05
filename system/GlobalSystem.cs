using Godot;

namespace Quasar.system
{
    public partial class GlobalSystem : Node
    {
        public static GlobalSystem Instance { get; private set; }

        public override void _Ready()
        {
            Instance = this;
        }

        public void LoadInterface<T>(Node node, out T toInterface)
        {
            toInterface = default;

            if (node is T tempInterface)
            {
                toInterface = tempInterface;
            }
            else
            {
                Quit(1, $"Failed to Load Interface: {typeof(T)}");
            }
        }

        public void Quit(int exitCode = 0, string errorMessage = "")
        {
            if (errorMessage != "")
            {
                GD.Print(errorMessage);
            }

            GetTree().Quit(exitCode);
        }
    }
}
