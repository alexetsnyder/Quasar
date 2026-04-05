using Catcophony.core.actions.interfaces;
using Catcophony.scenes.common.interfaces;
using Catcophony.scenes.systems.building;
using Godot;

namespace Catcophony.core.actions.commands
{
    public partial class BuildingCommand(IWorld world, IPathingSystem pathingSystem, ISelectionSystem selectionSystem, Vector2 localPos, Buildable buildable) : ICommand
    {
        private readonly IWorld _world = world;

        public readonly IPathingSystem _pathingSystem = pathingSystem;

        private readonly ISelectionSystem _selectionSystem = selectionSystem;

        private readonly Buildable _buildable = buildable;

        private readonly Vector2 _localPos = localPos;

        public void Execute(IActor actor = null)
        {
            _world.Build(_localPos, _buildable);

            _pathingSystem.SetPointSolid(_localPos);

            _selectionSystem.Deselect(_localPos);
        }
    }
}
