using Catcophony.core.actions.interfaces;
using Catcophony.scenes.common.interfaces;
using Godot;

namespace Catcophony.core.actions.commands
{
    public partial class GatheringCommand(IWorld world, ISelectionSystem selectionSystem, Vector2 localPos) : ICommand
    {
        private readonly IWorld _world = world;

        private readonly ISelectionSystem _selectionSystem = selectionSystem;

        private readonly Vector2 _localPos = localPos;

        public void Execute(IActor actor = null)
        {
            _world.Gather(_localPos);

            _selectionSystem.Deselect(_localPos);
        }
    }
}