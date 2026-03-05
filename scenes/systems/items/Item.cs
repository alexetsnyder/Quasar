using Godot;
using Quasar.data.enums;

namespace Quasar.scenes.systems.items
{
    public partial class Item(int iD, TileType tileType, Vector2 position) : Resource
    {
        public int ID { get; set; } = iD;

        public TileType TileType { get; set; } = tileType;

        public Vector2 Position { get; set; } = position;
    }
}