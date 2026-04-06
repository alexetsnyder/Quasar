using Catcophony.core.actions;
using Catcophony.core.actions.interfaces;
using Catcophony.core.blackboard;
using Catcophony.core.enums;
using Catcophony.core.goap.actions;
using Catcophony.core.goap.interfaces;
using Catcophony.core.naming;
using Catcophony.data.enums;
using Catcophony.scenes.common.interfaces;
using Catcophony.scenes.systems.items;
using Godot;
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

            foreach (ActionType actionType in System.Enum.GetValues(typeof(ActionType)))
            {
                if (!_excludedActionTypes.Contains(actionType))
                {
                   actionTypeList.Add(actionType);
                }
            }

            return actionTypeList;
        }

        public IAction BuildAction(Blackboard<FastName> blackboard, ActionType actionType)
        {
            switch (actionType)
            {
                case ActionType.MINING:
                case ActionType.WOOD_CUTTING:
                case ActionType.BUILDING:
                case ActionType.FARMING:
                case ActionType.GATHERING:
                case ActionType.FISHING:
                    return new WorkAction(blackboard, new FastName(actionType.ToString()), 1, actionType);
                case ActionType.MOVE_TO:
                    return new MoveToAction(blackboard);
                case ActionType.HAULING:
                    return new HaulAction(blackboard);
                case ActionType.GET_ITEM:
                    return new GetItemAction(blackboard);
                case ActionType.DRINKING:
                    return new DrinkAction(blackboard, _world);
                default:
                    GD.Print($"Action {actionType} not implimented in WorldState::BuildAction method.");
                    return null;
            }
        }

        public Vector2? GetAgentPos()
        {
            if (_blackboard.TryGetVector2(Constants.Names.AgentPos, out var agentPos))
            {
                return agentPos;
            }

            return null;
        }

        public Item GetAgentItem()
        {
            if (_blackboard.TryGetItem(Constants.Names.AgentItem, out var item))
            {
                return item;
            }

            return null;
        }

        public ActionType GetAgentProf()
        {
            if (_blackboard.TryGetInt(Constants.Names.AgentProf, out var agentProfInt))
            {
                return (ActionType)agentProfInt;
            }

            return ActionType.NONE;
        }

        public List<core.actions.Action> GetActions(ActionType actionType)
        {
            if (_blackboard.TryGetActionList(new FastName(actionType.ToString()), out var actions))
            {
                return actions;
            }

            return [];
        }

        public bool HasPath(Vector2 agentPos, Action action)
        {
            return HasPath(agentPos, action.LocalPos);
        }

        public bool HasPath(Vector2 agentPos, Vector2 localPos)
        {
            var path = _pathingSystem.ShortestPath(agentPos, _world.GetAdjacentTiles(localPos));

            if (path != null)
            {
                _pathingSystem.RemovePath(path.Id);

                return true;
            }

            return false;
        }

        public Vector2? SearchForNearest(Vector2 agentPos, TileType tileType)
        {
            return _world.SearchForNearest(agentPos, tileType);
        }
    }
}