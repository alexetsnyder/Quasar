using Godot;
using Quasar.data.enums;
using Quasar.scenes.common.interfaces;
using Quasar.system;

namespace Quasar.scenes.systems.work.commands
{
    [GlobalClass]
    public partial class CommandFactory : Node
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

        public ICommand BuildCommand(WorkType workType, Vector2 localPos)
        {
            switch (workType)
            {
                case WorkType.MINING:
                    return new MiningCommand(_world, _itemSystem, _pathingSystem, _selectionSystem, localPos, TileType.STONE);
                case WorkType.BUILDING:
                    return new BuildingCommand(_world, _pathingSystem, _selectionSystem, localPos, _buildingSystem.Current);
                case WorkType.HAULING:
                case WorkType.GET_ITEM:
                    return new HaulingCommand(_world, _itemSystem, _selectionSystem, localPos);
                case WorkType.WOOD_CUTTING:
                    return new CuttingCommand(_world, _itemSystem, _pathingSystem, _selectionSystem, localPos);
                case WorkType.FARMING:
                    return new FarmingCommand(_world, _selectionSystem, localPos);
                case WorkType.GATHERING:
                    return new GatheringCommand(_world, _selectionSystem, localPos);
                case WorkType.FISHING:
                    return new FishingCommand(_world, _selectionSystem, localPos);
                default:
                    GD.Print($"Could not create command for WorkType {workType} at {localPos}.");
                    return null;
            }
        }
    }
}