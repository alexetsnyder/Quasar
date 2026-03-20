using Godot;
using Quasar.data.enums;
using Quasar.scenes.systems.items;
using System.Collections.Generic;

namespace Quasar.scenes.common.interfaces
{
    public interface IItemSystem
    {
        public Dictionary<Vector2I, List<Item>> GetAllItems();

        public void CreateItem(TileType tileType, Vector2 localPos, Color? color = null);

        public void RemoveItem(Item item);

        public List<Item> GetInventoryItems(int catId);

        public List<Item> GetItems(Vector2 localPos);

        public void PickUpItem(int id, Item item);

        public void StoreItem(int agentId, int storageId, Item item, Vector2 localPos);

        public void PlaceItem(int id, Item item, Vector2 localPos);
    }
}