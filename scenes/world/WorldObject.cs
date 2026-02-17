using Godot;

namespace Quasar.scenes.world
{
    public partial class WorldObject
    {
        public int ID { get; set; }

        public Vector2I CellCoord { get; set; }

        public WorldObject(int id, Vector2I cellCoord) 
        {
            ID = id;
            CellCoord = cellCoord;
        }
    }
}