using Catcophony.core.actions.interfaces;
using Catcophony.core.blackboard;
using Catcophony.core.enums;
using Catcophony.core.goap.actions;
using Catcophony.core.goap.interfaces;
using Catcophony.core.naming;
using Catcophony.scenes.common.interfaces;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Catcophony.core.goap
{
    public partial class WorldState
    {
        private readonly Blackboard<FastName> _blackboard = new();

        private readonly IAgent _agent;

        private readonly IWorld _world;

        private readonly IActionManager _actionManager;

        private readonly IPathingSystem _pathingSystem;

        private readonly IItemSystem _itemSystem;

        private readonly List<ActionType> _predefinedActionTypes =
        [
            ActionType.MINING,
            ActionType.BUILDING,
            ActionType.HAULING,
            ActionType.GET_ITEM,
            ActionType.WOOD_CUTTING,
            ActionType.FARMING,
            ActionType.GATHERING,
            ActionType.FISHING,
        ];

        private readonly List<ActionType> _excludedActionTypes =
        [
            ActionType.NONE,
            ActionType.CANCEL,
            ActionType.CREATE_REGION,
        ];

        public WorldState(IAgent agent, IWorld world, IActionManager actionManager, IPathingSystem pathingSystem, IItemSystem itemSystem) 
        { 
            _agent = agent;
            _world = world;
            _actionManager = actionManager;
            _pathingSystem = pathingSystem;
            _itemSystem = itemSystem;

            _blackboard.Set(Constants.Names.AgentPos, _agent.Position);
            _blackboard.Set(Constants.Names.AgentProf, (int)_agent.ActionType);
            var item = _itemSystem.GetInventoryItems(_agent.Id).FirstOrDefault();

            if (item != null)
            {
                _blackboard.Set(Constants.Names.AgentItem, item);
            }
            
            foreach (var actionType in _predefinedActionTypes)
            {
                var actionList = _actionManager.CheckForWork(actionType);
                _blackboard.Set(new(actionType.ToString()), actionList);
            }
        }

        public List<ActionType> GetAvailableActionTypes()
        {
            var actionTypeList = new List<ActionType>();

            foreach (ActionType actionType in Enum.GetValues(typeof(ActionType)))
            {
                if (!_excludedActionTypes.Contains(actionType))
                {
                   actionTypeList.Add(actionType);
                }
            }

            return actionTypeList;
        }

        public IAction BuildAction(ActionType actionType)
        {
            switch (actionType)
            {
                case ActionType.MINING:
                case ActionType.WOOD_CUTTING:
                case ActionType.BUILDING:
                case ActionType.FARMING:
                case ActionType.GATHERING:
                case ActionType.FISHING:
                    //var workType = GetWorkType(action);
                    return new WorkAction(new(actionType.ToString()), 1, actionType, _world);
                case ActionType.MOVE_TO:
                    return new MoveToAction(_world, _pathingSystem);
                case ActionType.MOVE_TO_WATER:
                    return new MoveToWaterAction(_world, _pathingSystem);
                case ActionType.HAULING:
                    return new HaulAction();
                case ActionType.GET_ITEM:
                    return new GetItemAction(_world);
                case ActionType.DRINKING:
                    return new DrinkAction(_world);
                default:
                    GD.Print($"Action {actionType} not implimented in WorldState::BuildAction method.");
                    return null;
            }
        }

        public Blackboard<FastName> GetBlackboard()
        {
            return _blackboard; 
        }
    }
}