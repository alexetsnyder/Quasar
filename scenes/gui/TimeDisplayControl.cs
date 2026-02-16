using Godot;

namespace Quasar.scenes.gui
{
    public partial class TimeDisplayControl : Control
    {
        private Label _timeDisplayLabel;

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            _timeDisplayLabel = GetNode<Label>("MarginContainer/TimeDisplayLabel");
        }

        // Called every frame. 'delta' is the elapsed time since the previous frame.
        public override void _Process(double delta)
        {
        }

        public void OnTimeChange(string dateTimeStr)
        {
            _timeDisplayLabel.Text = dateTimeStr;
        }
    }
}
