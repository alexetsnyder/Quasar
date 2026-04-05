using Catcophony.core.enums;
using Godot;

namespace Catcophony.core.actions.interfaces
{
    public interface ICommandFactory
    {
        public ICommand BuildCommand(ActionType actionType, Vector2 localPos);
    }
}