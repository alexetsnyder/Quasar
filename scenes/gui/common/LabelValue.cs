using Godot;

namespace Catcophony.scenes.gui.common
{
    public partial class LabelValue : Control
    {
        private Label _labelTitle;

        private Label _labelValue;

        public override void _Ready()
        {
            _labelTitle = GetNode<Label>("%LabelTitle");
            _labelValue = GetNode<Label>("%LabelValue");
        }

        public void SetTitle(string title)
        {
            _labelTitle.Text = title;
        }

        public void SetValue(string value)
        {
            _labelValue.Text = value;
        }
    }
}