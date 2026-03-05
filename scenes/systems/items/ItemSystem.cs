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

        private IMultiColorTileMapLayer _itemTileMapLayer;

        public override void _Ready()
        {
            _itemTileMapLayer = GetNode<IMultiColorTileMapLayer>("ItemTileMapLayer");
        }

        public void CreateItem(TileType tileType, Vector2 localPos)
        {
            var coords = _itemTileMapLayer.LocalToMap(localPos);
            var atlasCoords = GetAtlasCoords(tileType);
            var color = GetColor(tileType);

            _items.TryAdd(coords, []);
            _items[coords].Add(new(_nextId++, tileType, localPos));

            SetCell(coords, atlasCoords, color);
        }

        public void RemoveItem(Item item)
        {
            var coords = _itemTileMapLayer.LocalToMap(item.Position);

            _items[coords].Remove(item);

            SetCell(coords);
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

        public void PickUpItem(Item item)
        {
            var coords = _itemTileMapLayer.LocalToMap(item.Position);
            SetCell(coords);
        }

        public void PlaceItem(Item item, Vector2 localPos)
        {
            var originalCoords = _itemTileMapLayer.LocalToMap(item.Position);
            if (_itemTileMapLayer.GetCellSourceId(originalCoords) != -1)
            {
                SetCell(originalCoords);
            }

            var coords = _itemTileMapLayer.LocalToMap(localPos);
            var atlasCoords = GetAtlasCoords(item.TileType);
            var color = GetColor(item.TileType);

            SetCell(coords, atlasCoords, color);
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
