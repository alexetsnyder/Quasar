using Godot;
using Quasar.scenes.systems.items;
using Quasar.system;
using System.Collections.Generic;

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
    }
}

