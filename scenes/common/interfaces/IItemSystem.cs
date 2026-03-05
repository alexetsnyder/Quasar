using Godot;
using Quasar.data.enums;
using Quasar.scenes.systems.items;
using System.Collections.Generic;

namespace Quasar.scenes.common.interfaces
{
    public interface IItemSystem
    {
        public void CreateItem(TileType tileType, Vector2 localPos);

        public void RemoveItem(Item item);

        public List<Item> GetItems(Vector2 localPos);
    }
}