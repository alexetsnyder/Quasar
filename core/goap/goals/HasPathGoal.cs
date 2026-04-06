using Catcophony.core.blackboard;
using Catcophony.core.naming;

namespace Catcophony.core.goap.goals
{
    public partial class HasPathGoal : GoalBase
    {
        public HasPathGoal(Blackboard<FastName> blackboard)
            : base(blackboard)
        {
            _key = new("HasPath");
            _value = true;
        }

        public override bool Satisify(WorldState worldState)
        {
            var agentPos = worldState.GetAgentPos();

            if (agentPos != null)
            {
                if (_blackboard.TryGetAction(Constants.Names.Action, out var action))
                {
                    return worldState.HasPath(agentPos.Value, action);
                }
                else if (_blackboard.TryGetVector2(Constants.Names.LocalPos, out var localPos))
                {
                    return worldState.HasPath(agentPos.Value, localPos);
                }
            }

            return false;
        }
    }
}