using Godot;
using Quasar.scenes.systems.items;

namespace Quasar.scenes.gui.items
{
    public partial class InventoryControl : Control
    {
        private ItemList _inventoryItemList;

        public override void _Ready()
        {
            _inventoryItemList = GetNode<ItemList>("InventoryItemList");

            _inventoryItemList.Clear();
        }

        public void Add(Item item)
        {
            _inventoryItemList.AddItem(item.TileType.ToString());
        }
    }
}

