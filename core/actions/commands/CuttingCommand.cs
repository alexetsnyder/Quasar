using Catcophony.core.actions.interfaces;
using Catcophony.data.enums;
using Catcophony.scenes.common.interfaces;
using Godot;

namespace Catcophony.core.actions.commands
{
    public partial class CuttingCommand(IWorld world, IItemSystem itemSystem, IPathingSystem pathingSystem,
                                        ISelectionSystem selectionSystem, Vector2 localPos) : ICommand
    {
        private readonly IWorld _world = world;

        private readonly IItemSystem _itemSystem = itemSystem;

        private readonly IPathingSystem _pathingSystem = pathingSystem;

        private readonly ISelectionSystem _selectionSystem = selectionSystem;

        private readonly Vector2 _localPos = localPos;

        public void Execute(IActor actor = null)
        {
            var material = _world.Cut(_localPos);

            _itemSystem.CreateItem(TileType.WOOD, _localPos, material);

            _pathingSystem.SetPointSolid(_localPos, false);

            _selectionSystem.Deselect(_localPos);
        }
    }
}