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

        private Label _workLabelValue;

        private Label _workPosLabel;

        private Label _workPosLabelValue;

        private bool _isMoving = false;

        private Vector2 _prevMousePos;

        private CatData _catData;

        public override void _Ready()
        {
            _nameTab = GetNode<VBoxContainer>("TabsAndContent/PanelContainer/NameTab");
            _statusTab = GetNode<VBoxContainer>("TabsAndContent/PanelContainer/StatusTab");
            _inventoryTab = GetNode<VBoxContainer>("TabsAndContent/PanelContainer/InventoryTab");
            _catNameLabel = GetNode<Label>("TabsAndContent/PanelContainer/NameTab/Name");
            _catDescriptionLabel = GetNode<Label>("TabsAndContent/PanelContainer/NameTab/Description");
            _healthLabelValue = GetNode<Label>("TabsAndContent/PanelContainer/StatusTab/HealthLabelValue");
            _feelingsLabelValue = GetNode<Label>("TabsAndContent/PanelContainer/StatusTab/FeelingsLabelValue");
            _workLabelValue = GetNode<Label>("TabsAndContent/PanelContainer/StatusTab/WorkLabelValue");
            _workPosLabel = GetNode<Label>("TabsAndContent/PanelContainer/StatusTab/WorkPosLabel");
            _workPosLabelValue = GetNode<Label>("TabsAndContent/PanelContainer/StatusTab/WorkPosLabelValue");
        }

        public override void _Process(double delta)
        {
            FillUI();
        }

        public void SetCatData(CatData catData)
        {
            _catData = catData;
            FillUI();
        }

        public void FillUI()
        {
            if (_catData == null)
            {
                return;
            }

            _catNameLabel.Text = _catData.Name;
            _catDescriptionLabel.Text = _catData.Description;
            _healthLabelValue.Text = _catData.Health.ToString();
            _feelingsLabelValue.Text = _catData.Feelings;
            _workLabelValue.Text = _catData.Profession.ToString();

            if (_catData.WorkPos != null)
            {
                _workPosLabel.Visible = true;
                _workPosLabelValue.Text = _catData.WorkPos.Value.ToString();
            }
            else
            {
                _workPosLabel.Visible = false;
                _workPosLabelValue.Text = "";
            }
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

        public void OnGUIInput(InputEvent @event)
        {
            if (@event is InputEventMouseButton inputEventMouseButton)
            {
                if (inputEventMouseButton.ButtonIndex == MouseButton.Left)
                {
                    if (@event.IsPressed())
                    {
                        _isMoving = true;
                        _prevMousePos = inputEventMouseButton.Position;
                    }
                    else
                    {
                        _isMoving = false;
                    }
                }
            }
            else if (@event is InputEventMouseMotion mouseEventMouseMotion)
            {
                if (_isMoving)
                {
                    var dv = mouseEventMouseMotion.Position - _prevMousePos;
                    Position += dv;
                }
            }
        }
    }
}
