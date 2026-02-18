using Godot;
using Quasar.data.resources;

namespace Quasar.scenes.time
{
    public partial class TimeSystem : Node
    {
        public static TimeSystem Instance { get; private set; }

        [Export]
        public DateTime DateTime { get; set; }

        [Export]
        public int TicksPerSecond { get; set; } = 6;

        [Signal]
        public delegate void TimeChangeEventHandler(string dateTimeStr);

        public override void _Ready()
        {
            DateTime = GD.Load<DateTime>("res://data/resources/DateTime.tres");
            Instance = this;
        }

        public override void _Process(double delta)
        {
            DateTime.IncreaseTimeInSeconds(delta * TicksPerSecond);
            EmitSignal(SignalName.TimeChange, DateTime.ToString());
        }
    }
}
