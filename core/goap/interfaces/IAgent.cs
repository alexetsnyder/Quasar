using Catcophony.core.enums;
using Godot;

namespace Catcophony.core.goap.interfaces
{
    public interface IAgent
    {
        public int Id { get; }

        public Vector2 Position { get; set; }

        public ActionType ActionType { get; }
    }
}