using Godot;
using System.Collections.Generic;

namespace Quasar.scenes.world
{
    public partial class WorldManager
    {
        private Dictionary<int, WorldObject> _worldObjects = [];

        private int _nextId = 0;

        public int Register(Vector2I cellCoord)
        {
            _worldObjects.Add(_nextId, new(_nextId, cellCoord));
            return _nextId++;
        }

        public void Unregister(int id)
        {
            _worldObjects.Remove(id);
        }

        public Vector2I? GetCellCoord(int id)
        {
            if (_worldObjects.TryGetValue(id, out WorldObject worldObject))
            {
                return worldObject.CellCoord;
            }

            return null;
        }

        public void UpdateCellCoord(int id, Vector2I cellCoord)
        {
            _worldObjects[id].CellCoord = cellCoord;
        }
    }
}