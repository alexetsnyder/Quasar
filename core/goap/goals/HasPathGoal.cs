using Catcophony.core.actions;
using Catcophony.core.blackboard;
using Catcophony.core.enums;
using Catcophony.core.goap.interfaces;
using Catcophony.core.naming;
using Catcophony.scenes.common.interfaces;
using Godot;
using System.Collections.Generic;

namespace Catcophony.core.goap.goals
{
    public partial class HasPathGoal : GoalBase
    {
        private readonly IWorld _world;

        private readonly IPathingSystem _pathingSystem;

        public HasPathGoal(IAction parent, IWorld world, IPathingSystem pathingSystem)
        {
            _key = new("HasPath");
            _value = true;

            _world = world;
            _pathingSystem = pathingSystem;
            _parentAction = parent;
        }

        public override bool Satisify(WorldState worldState, Blackboard<FastName> blackboard)
        {
            var worldStateBlackboard = worldState.GetBlackboard();

            if (worldStateBlackboard.TryGetVector2(Constants.Names.AgentPos, out var agentPos))
            {
                var parentBlackboard = _parentAction.GetParentBlackboard();

                if (parentBlackboard.TryGetInt(Constants.Names.ActionType, out var actionTypeInt))
                {
                    var actionType = (ActionType)actionTypeInt;

                    if (worldStateBlackboard.TryGetActionList(new(actionType.ToString()), out var actionList))
                    {
                        return HasPath(agentPos, actionList);
                    }
                }
            }

            return false;
        }

        private bool HasPath(Vector2 fromPos, List<Action> actionList)
        {
            var blackboard = _parentAction.GetBlackboard();
            var parentBlackboard = _parentAction.GetParentBlackboard();

            foreach (var action in actionList)
            {
                if(HasPath(fromPos, action))
                {
                    parentBlackboard.Set(Constants.Names.Action, action);
                    blackboard.Set(Constants.Names.LocalPos, action.LocalPos);
                    return true;
                }
            }

            return false;
        }

        private bool HasPath(Vector2 fromPos, Action action)
        {
            var path = _pathingSystem.ShortestPath(fromPos, _world.GetAdjacentTiles(action.LocalPos));

            if (path != null)
            {
                _pathingSystem.RemovePath(path.Id);

                return true;
            }

            return false;
        }
    }
}