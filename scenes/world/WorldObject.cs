using Godot;
using Quasar.scenes.common.interfaces;

namespace Quasar.scenes.world
{
    public partial class WorldObject(int id, IGameObject gameObject, Vector2I cellCoord)
    {
        public int ID { get; set; } = id;

        public IGameObject GameObject {  get; set; } = gameObject;

        public Vector2I CellCoord { get; set; } = cellCoord;
    }
}