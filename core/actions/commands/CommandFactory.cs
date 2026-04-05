using Catcophony.core.actions.interfaces;
using Catcophony.core.enums;
using Catcophony.core.goap.interfaces;
using Catcophony.data.enums;
using Catcophony.scenes.cats;
using Catcophony.scenes.common.interfaces;
using Catcophony.system;
using Godot;

namespace Catcophony.core.actions.commands
{
    [GlobalClass]
    public partial class CommandFactory : Node, ICommandFactory
    {
        [Export]
        public Node WorldNode { get; set; }

        [Export]
        public Node PathingSystemNode { get; set; }

        [Export]
        public Node ItemSystemNode { get; set; }

        [Export]
        public Node BuildingSystemNode { get; set; }

        [Export]
        public Node SelectionSystemNode { get; set; }

        private IWorld _world;

        private IPathingSystem _pathingSystem;

        private IItemSystem _itemSystem;

        private IBuildingSystem _buildingSystem;

        private ISelectionSystem _selectionSystem;

        public override void _Ready()
        {
            GlobalSystem.Instance.LoadInterface<IWorld>(WorldNode, out _world);
            GlobalSystem.Instance.LoadInterface<IPathingSystem>(PathingSystemNode, out _pathingSystem);
            GlobalSystem.Instance.LoadInterface<IItemSystem>(ItemSystemNode, out _itemSystem);
            GlobalSystem.Instance.LoadInterface<IBuildingSystem>(BuildingSystemNode, out _buildingSystem);
            GlobalSystem.Instance.LoadInterface<ISelectionSystem>(SelectionSystemNode, out _selectionSystem);
        }

        public ICommand BuildCommand(ActionType actionType, Vector2 localPos)
        {
            switch (actionType)
            {
                case ActionType.MINING:
                    return new MiningCommand(_world, _itemSystem, _pathingSystem, _selectionSystem, localPos, TileType.STONE);
                case ActionType.BUILDING:
                    return new BuildingCommand(_world, _pathingSystem, _selectionSystem, localPos, _buildingSystem.Current);
                case ActionType.HAULING:
                case ActionType.GET_ITEM:
                    return new HaulingCommand(_world, _itemSystem, _selectionSystem, localPos);
                case ActionType.WOOD_CUTTING:
                    return new CuttingCommand(_world, _itemSystem, _pathingSystem, _selectionSystem, localPos);
                case ActionType.FARMING:
                    return new FarmingCommand(_world, _selectionSystem, localPos);
                case ActionType.GATHERING:
                    return new GatheringCommand(_world, _selectionSystem, localPos);
                case ActionType.FISHING:
                    return new FishingCommand(_world, _selectionSystem, localPos);
                case ActionType.DRINKING:
                    return new DrinkingCommand();
                case ActionType.MOVE_TO:
                    return new MoveToCommand(localPos);
                default:
                    GD.Print($"Could not create command for action {actionType} at {localPos}.");
                    return null;
            }
        }
    }
}