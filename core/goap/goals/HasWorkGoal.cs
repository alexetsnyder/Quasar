using Catcophony.core.blackboard;
using Catcophony.core.enums;
using Catcophony.core.naming;

namespace Catcophony.core.goap.goals
{
    public partial class HasWorkGoal : GoalBase
    {
        private readonly ActionType _actionType;

        public HasWorkGoal(Blackboard<FastName> blackboard, ActionType actionType)
            : base(blackboard)
        {
            _key = new("HasWork");
            _value = true;

            _actionType = actionType;
        }

        public override bool Satisify(WorldState worldState)
        {
            var agentPos = worldState.GetAgentPos();
            var actions = worldState.GetActions(_actionType);

            if (agentPos != null && actions.Count > 0)
            {
                foreach (var action in actions)
                {
                    if (worldState.HasPath(agentPos.Value, action))
                    {
                        _blackboard.Set(Constants.Names.Action, action);
                        _blackboard.Set(Constants.Names.LocalPos, action.LocalPos);

                        return true;
                    }
                }
            }

            return false;
        }
    }
}