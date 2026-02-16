using Godot;
using Quasar.data.resources;

namespace Quasar.scenes.time
{
    [GlobalClass]
    public partial class TimeSystem : Node
    {
        [Export]
        public DateTime DateTime { get; set; }

        [Export]
        public int TicksPerSecond { get; set; } = 6;

        [Signal]
        public delegate void TimeChangeEventHandler(string dateTimeStr);

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
        }

        // Called every frame. 'delta' is the elapsed time since the previous frame.
        public override void _Process(double delta)
        {
            DateTime.IncreaseTimeInSeconds(delta * TicksPerSecond);
            EmitSignal(SignalName.TimeChange, DateTime.ToString());
        }
    }
}
