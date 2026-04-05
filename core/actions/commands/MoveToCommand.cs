using Catcophony.core.actions.interfaces;
using Godot;

namespace Catcophony.core.actions.commands
{
    public partial class MoveToCommand(Vector2 localPos) : ICommand
    {
        private readonly Vector2 _localPos = localPos;

        public void Execute(IActor actor = null)
        {
            actor?.SetDestination(_localPos);
        }
    }
}