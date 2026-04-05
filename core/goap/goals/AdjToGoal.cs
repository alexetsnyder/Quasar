using Catcophony.core.blackboard;
using Catcophony.core.enums;
using Catcophony.core.goap.interfaces;
using Catcophony.core.naming;
using Catcophony.scenes.common.interfaces;

namespace Catcophony.core.goap.goals
{
    public partial class AdjToGoal : GoalBase
    {
        private readonly IWorld _world;

        public AdjToGoal(IAction parent, IWorld world)
        {
            _key = new("AdjTo");
            _value = true;

            _parentAction = parent;
            _world = world;
        }

        public override bool Satisify(WorldState worldState, Blackboard<FastName> blackboard)
        {
            var worldStateBlackboard = worldState.GetBlackboard();

            if (worldStateBlackboard.TryGetVector2(Constants.Names.AgentPos, out var agentPos))
            {
                if (blackboard.TryGetInt(Constants.Names.ActionType, out var actionTypeInt))
                {
                    var actionType = (ActionType)actionTypeInt;

                    if (blackboard.TryGetAction(Constants.Names.Action, out var action))
                    {
                        foreach (var adjPos in _world.GetAdjacentTiles(action.LocalPos))
                        {
                            if (adjPos.IsEqualApprox(agentPos))
                            {
                                return true;
                            }
                        }
                    }

                    if (worldStateBlackboard.TryGetActionList(new(actionType.ToString()), out var actionList))
                    {
                        if (actionList.Count > 0)
                        {
                            foreach (var nextAction in actionList)
                            {
                                foreach (var adjPos in _world.GetAdjacentTiles(nextAction.LocalPos))
                                {
                                    if (adjPos.IsEqualApprox(agentPos))
                                    {
                                        blackboard.Set(Constants.Names.Action, nextAction);
                                        return true;
                                    }
                                }
                            }
                        }
                    }
                } 
            }

            return false;
        }
    }
}