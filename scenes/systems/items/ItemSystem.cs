using Godot;
using Quasar.data;
using Quasar.data.enums;
using Quasar.scenes.common.interfaces;
using System.Collections.Generic;


namespace Quasar.scenes.systems.items
{
    public partial class ItemSystem : Node2D, IItemSystem
    {
        private int _nextId = 0;

        private readonly Dictionary<Vector2I, List<Item>> _items = [];

        private readonly Dictionary<int, List<Item>> _inventory = [];

        private readonly Dictionary<int, List<Item>> _storage = [];

        private IMultiColorTileMapLayer _itemTileMapLayer;

        public override void _Ready()
        {
            _itemTileMapLayer = GetNode<IMultiColorTileMapLayer>("ItemTileMapLayer");
        }

        public Dictionary<Vector2I, List<Item>> GetAllItems()
        {
            return _items;
        }

        public void CreateItem(TileType tileType, Vector2 localPos, Color? color = null)
        {
            var coords = _itemTileMapLayer.LocalToMap(localPos);
            var atlasCoords = GetAtlasCoords(tileType);
            if (color == null)
            {
                color = GetColor(tileType);
            }

            _items.TryAdd(coords, []);
            _items[coords].Add(new(_nextId++, tileType, localPos));

            SetCell(coords, atlasCoords, color);
        }

        public void TryAddStorage(int storageId)
        {
            _storage.TryAdd(storageId, []);
        }

        public void RemoveItem(Item item)
        {
            var coords = _itemTileMapLayer.LocalToMap(item.Position);

            _items[coords].Remove(item);

            TryClearCell(coords);
        }

        public List<Item> GetInventoryItems(int catId)
        {
            if (_inventory.TryGetValue(catId, out var items))
            {
                return items;
            }

            return [];
        }

        public List<Item> GetItems(Vector2 localPos)
        {
            var coords = _itemTileMapLayer.LocalToMap(localPos);

            if (_items.TryGetValue(coords, out List<Item> items))
            {
                return items;
            }

            return [];
        }

        public List<Item> GetStoredItems(int storageId)
        {
            if (_storage.TryGetValue(storageId, out List<Item> items))
            {
                return items;
            }

            return [];
        }

        public void PickUpItem(int id, Item item)
        {
            var coords = _itemTileMapLayer.LocalToMap(item.Position);

            _inventory.TryAdd(id, []);
            _inventory[id].Add(item);

            RemoveItem(item);
        }

        public void StoreItem(int agentId, int storageId, Item item, Vector2 localPos)
        {
            item.Position = localPos;

            var coords = _itemTileMapLayer.LocalToMap(localPos);

            _items.TryAdd(coords, []);
            _items[coords].Add(item);

            TryAddStorage(storageId);

            _storage[storageId].Add(item);
            _inventory[agentId].Remove(item);
        }

        public void PlaceItem(int id, Item item, Vector2 localPos)
        {
            item.Position = localPos;

            var coords = _itemTileMapLayer.LocalToMap(localPos);
            var atlasCoords = GetAtlasCoords(item.TileType);
            var color = GetColor(item.TileType);

            _items.TryAdd(coords, []);
            _items[coords].Add(item);

            _inventory[id].Remove(item);

            SetCell(coords, atlasCoords, color);
        }

        private void TryClearCell(Vector2I coords)
        {
            if (_items[coords].Count == 0)
            {
                SetCell(coords);
            }
        }

        private void SetCell(Vector2I coords, Vector2I? atlasCoords = null, Color? color = null)
        {
            _itemTileMapLayer.SetCell(coords, atlasCoords, color);
        }

        private static Vector2I GetAtlasCoords(TileType tileType)
        {
            return AtlasConstants.GetAtlasCoords(tileType);
        }

        private static Color GetColor(TileType tileType)
        {
            return AtlasConstants.GetColor(tileType);
        }
    }
}
