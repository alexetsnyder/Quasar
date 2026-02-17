using Godot;

namespace Quasar.scenes.gui
{
    public partial class TimeDisplayControl : Control
    {
        private Label _timeDisplayLabel;

        public override void _Ready()
        {
            _timeDisplayLabel = GetNode<Label>("MarginContainer/TimeDisplayLabel");
        }

        public override void _Process(double delta)
        {
        }

        public void OnTimeChange(string dateTimeStr)
        {
            _timeDisplayLabel.Text = dateTimeStr;
        }
    }
}
