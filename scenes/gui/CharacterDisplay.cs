using Catcophony.core.reflection.attributes;
using Catcophony.scenes.cats;
using Catcophony.scenes.gui.common;
using Catcophony.system;
using Godot;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Catcophony.scenes.gui
{
    public partial class CharacterDisplay : MarginContainer
    {
        private VBoxContainer _nameTab;

        private VBoxContainer _characterStatusTab;

        private VBoxContainer _inventoryTab;

        private Label _catNameLabel;

        private Label _catDescriptionLabel;

        private bool _isMoving = false;

        private Vector2 _prevMousePos;

        private CatModel _catData;

        private List<LabelValue> _labelValues = new();

        public override void _Ready()
        {
            _nameTab = GetNode<VBoxContainer>("TabsAndContent/PanelContainer/NameTab");
            _characterStatusTab = GetNode<VBoxContainer>("%CharacterStatusTab");
            _inventoryTab = GetNode<VBoxContainer>("TabsAndContent/PanelContainer/InventoryTab");
            _catNameLabel = GetNode<Label>("TabsAndContent/PanelContainer/NameTab/Name");
            _catDescriptionLabel = GetNode<Label>("TabsAndContent/PanelContainer/NameTab/Description");
        }

        public override void _Process(double delta)
        {
            FillUI();
        }

        public void SetCatData(CatModel catData)
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

            FillStatusUI();
        }

        private void FillStatusUI()
        {
            ClearStatusList();

            var type = typeof(CatModel);
            var membersWithStatus = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                        .Where(m => m.GetCustomAttributes(typeof(StatusAttribute), false).Length > 0);

            foreach ( var member in membersWithStatus )
            {
                var labelValue = GlobalSystem.Instance.InstantiateScene<LabelValue>("res://scenes/gui/common/label_value.tscn");
                if ( labelValue != null )
                {
                    _labelValues.Add(labelValue);
                    _characterStatusTab.AddChild(labelValue);

                    labelValue.SetTitle($"{member.Name}: ");

                    var value = member.GetValue(_catData);
                    labelValue.SetValue(value?.ToString() ?? "Null");
                }
            }
        }

        private void ClearStatusList()
        {
            foreach (var label in _labelValues)
            {
                label.QueueFree();
            }

            _labelValues.Clear();
        }

        public void OnNameTabButtonPressed()
        {
            _nameTab.Visible = true;
            _characterStatusTab.Visible = false;
            _inventoryTab.Visible = false;
        }

        public void OnStatusTabButtonPressed()
        {
            _nameTab.Visible = false;
            _characterStatusTab.Visible = true;
            _inventoryTab.Visible = false;
        }

        public void OnInventoryTabButtonPressed()
        {
            _nameTab.Visible = false;
            _characterStatusTab.Visible = false;
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
