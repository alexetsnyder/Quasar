using Godot;

namespace Quasar.scenes.gui
{
    public partial class BasicLabelDisplay : MarginContainer
    {
        private Label _basicLabel;

        public override void _Ready()
        {
            _basicLabel = GetNode<Label>("BasicLabel");
        }

        public void SetLabelText(string text)
        {
            _basicLabel.Text = text;
        }

        public void SetLabelColor(Color color)
        {
            _basicLabel.AddThemeColorOverride("font_color", color);
        }
    }
}
