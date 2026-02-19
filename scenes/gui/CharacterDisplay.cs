using Godot;
using Quasar.scenes.cats;

namespace Quasar.scenes.gui
{
    public partial class CharacterDisplay : MarginContainer
    {
        private VBoxContainer _nameTab;

        private VBoxContainer _statusTab;

        private VBoxContainer _inventoryTab;

        private Label _catNameLabel;

        private Label _catDescriptionLabel;

        private Label _healthLabelValue;

        private Label _feelingsLabelValue;

        public override void _Ready()
        {
            _nameTab = GetNode<VBoxContainer>("TabsAndContent/PanelContainer/NameTab");
            _statusTab = GetNode<VBoxContainer>("TabsAndContent/PanelContainer/StatusTab");
            _inventoryTab = GetNode<VBoxContainer>("TabsAndContent/PanelContainer/InventoryTab");
            _catNameLabel = GetNode<Label>("TabsAndContent/PanelContainer/NameTab/Name");
            _catDescriptionLabel = GetNode<Label>("TabsAndContent/PanelContainer/NameTab/Description");
            _healthLabelValue = GetNode<Label>("TabsAndContent/PanelContainer/StatusTab/HealthLabelValue");
            _feelingsLabelValue = GetNode<Label>("TabsAndContent/PanelContainer/StatusTab/FeelingsLabelValue");
        }

        public void FillUI(CatData catData)
        {
            _catNameLabel.Text = catData.Name;
            _catDescriptionLabel.Text = catData.Description;
            _healthLabelValue.Text = catData.Health.ToString();
            _feelingsLabelValue.Text = catData.Feelings;
        }

        public void OnNameTabButtonPressed()
        {
            _nameTab.Visible = true;
            _statusTab.Visible = false;
            _inventoryTab.Visible = false;
        }

        public void OnStatusTabButtonPressed()
        {
            _nameTab.Visible = false;
            _statusTab.Visible = true;
            _inventoryTab.Visible = false;
        }

        public void OnInventoryTabButtonPressed()
        {
            _nameTab.Visible = false;
            _statusTab.Visible = false;
            _inventoryTab.Visible = true;
        }

        public void OnCloseButtonPressed()
        {
            Visible = false;
        }
    }
}
