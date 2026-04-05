using Catcophony.core.actions.interfaces;
using Catcophony.core.enums;
using Godot;

namespace Catcophony.core.actions
{
    public partial class Action(int id, ActionType actionType, Vector2 localPos, int ticks, ICommand command)
    {
        public int Id { get; set; } = id;

        public ActionType ActionType { get; set; } = actionType;
        
        public Vector2 LocalPos { get; set; } = localPos;

        public int Ticks { get; set; } = ticks;

        public ICommand Command { get; set; } = command;

        public bool IsAssinged { get; set; } = false;
    }
}