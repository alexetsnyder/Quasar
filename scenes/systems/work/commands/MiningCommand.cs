using Godot;
using Quasar.data.enums;
using Quasar.scenes.common.interfaces;

namespace Quasar.scenes.systems.work.commands
{
    public partial class MiningCommand(IWorld world, IItemSystem itemSystem, IPathingSystem pathingSystem,
                                       ISelectionSystem selectionSystem, Vector2 localPos, TileType tileType) : ICommand
    {
        private readonly IWorld _world = world;

        private readonly IItemSystem _itemSystem = itemSystem;

        private readonly IPathingSystem _pathingSystem = pathingSystem;

        private readonly ISelectionSystem _selectionSystem = selectionSystem;

        private readonly TileType _tileType = tileType;

        private readonly Vector2 _localPos = localPos;

        public void Execute()
        {
            _world.Mine(_localPos);

            _itemSystem.CreateItem(_tileType, _localPos);

            _pathingSystem.SetPointSolid(_localPos, false);

            _selectionSystem.Deselect(_localPos);
        }
    }
}
