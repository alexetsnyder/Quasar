using Godot;
using Quasar.scenes.systems.items;
using Quasar.system;

namespace Quasar.scenes.gui.items
{
    public partial class InventoryControl : Control
    {
        private GridContainer _grid;

        public override void _Ready()
        {
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

