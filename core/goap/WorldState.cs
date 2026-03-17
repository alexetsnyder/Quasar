using Godot;
using Quasar.core.blackboard;
using Quasar.core.common;
using Quasar.core.goap.actions;
using Quasar.core.goap.interfaces;
using Quasar.core.naming;
using Quasar.data.enums;
using Quasar.scenes.common.interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Quasar.core.goap
{
    public partial class WorldState
    {
        private readonly Blackboard<FastName> _blackboard = new();

        private readonly IAgent _agent;

        private readonly IWorkSystem _workSystem;

        private readonly IPathingSystem _pathingSystem;

        private IItemSystem _itemSystem;

        private readonly List<WorkType> _availableWorkTypes =
        [
            WorkType.MINING,
            WorkType.BUILDING,
        ];

        public enum Actions
        {
            NONE,
            MINE,
            BUILD,
            MOVE_TO,
            HAUL,
            GET_ITEM,
        }

        public WorldState(IAgent agent, IWorkSystem workSystem, IPathingSystem pathingSystem, IItemSystem itemSystem) 
        { 
            _agent = agent;
            _workSystem = workSystem;
            _pathingSystem = pathingSystem;
            _itemSystem = itemSystem;

            _blackboard.Set(Constants.Names.Position, _agent.Position);
            _blackboard.Set(Constants.Names.AgentWorkType, (int)_agent.WorkType);
            var item = _itemSystem.GetInventoryItems(_agent.Id).FirstOrDefault();

            if (item != null)
            {
                _blackboard.Set(Constants.Names.Item, item);
            }
            
            foreach (var workType in _availableWorkTypes)
            {
                var workList = _workSystem.CheckForWork(workType);
                _blackboard.Set(new(workType.ToString()), workList);
            }
        }

        public IAction BuildAction(Actions action)
        {
            switch (action)
            {
                case Actions.MINE:
                    return new MineAction();
                case Actions.BUILD: 
                    return new BuildAction();
                case Actions.MOVE_TO:
                    return new MoveToAction(_pathingSystem);
                case Actions.HAUL:
                    return new HaulAction();
                case Actions.GET_ITEM:
                    return new GetItemAction();
                default:
                    GD.Print($"Action {action} not implimented in BuildAction method.");
                    return null;
            }
        }

        public Blackboard<FastName> GetBlackboard()
        {
            return _blackboard; 
        }
    }
}