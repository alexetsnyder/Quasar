using Godot;
using Catcophony.scenes.systems.items;
using Catcophony.system;
using Catcophony.scenes.gui.common;

namespace Catcophony.scenes.gui.items
{
    public partial class InventoryControl : MovableControl
    {
        private GridContainer _grid;

        public override void _Ready()
        {
            base._Ready();

            _grid = GetNode<GridContainer>("%InventoryGridContainer");
        }

        public void Add(Item item)
        {
            var slot = GlobalSystem.Instance.InstantiateScene<InventorySlot>("res://scenes/gui/items/inventory_slot.tscn");
            if (slot != null)
            {
                _grid.AddChild(slot);

                slot.Add(item);
            }
        }

        private void OnInventoryButtonClosePressed()
        {
            Visible = false;
            ClearItems();
        }

        public void ClearItems()
        {
            foreach (var child in _grid.GetChildren())
            {
                if (child is InventorySlot slot)
                {
                    _grid.RemoveChild(slot);
                    slot.QueueFree();
                }    
            }
        }
    }
}

