using Catcophony.core.reflection.attributes;
using Catcophony.scenes.cats;
using Catcophony.scenes.gui.common;
using Catcophony.system;
using Godot;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Catcophony.scenes.gui.character
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

        private readonly List<LabelValue> _labelValues = new();

        public override void _Ready()
        {
            _nameTab = GetNode<VBoxContainer>("%CharacterNameTab");
            _characterStatusTab = GetNode<VBoxContainer>("%CharacterStatusTab");
            _inventoryTab = GetNode<VBoxContainer>("%CharacterInventoryTab");
            _catNameLabel = GetNode<Label>("%CharacterName");
            _catDescriptionLabel = GetNode<Label>("%CharacterDescription");
        }

        public override void _Process(double delta)
        {
            if (!Input.IsMouseButtonPressed(MouseButton.Left) && _isMoving)
            {
                _isMoving = false;
            }

            if(_catData != null && _catData.IsDirty)
            {
                FillUI(_catData.GetCopy());
            } 
        }

        public void SetCatData(CatModel catData)
        {
            _catData = catData;
            FillUI(_catData.GetCopy());
        }

        public void FillUI(CatModel catModel)
        {
            if (catModel == null)
            {
                return;
            }

            _catNameLabel.Text = catModel.Name;
            _catDescriptionLabel.Text = catModel.Description;

            FillStatusUI(catModel);
        }

        private void FillStatusUI(CatModel catModel)
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

                    var value = member.GetValue(catModel);
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
