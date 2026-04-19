using Catcophony.core.blackboard;
using Catcophony.core.naming;

namespace Catcophony.core.goap.goals
{
    public partial class AdjToGoal : GoalBase
    {
        public AdjToGoal(Blackboard<FastName> blackboard)
            : base(blackboard)
        {
            _key = new("AdjTo");
            _value = true;
        }

        public override bool Satisify(WorldState worldState)
        {
            var agentPos = worldState.GetAgentPos();

            if (agentPos != null)
            {
                if (_blackboard.TryGetVector2(Constants.Names.LocalPos, out var localPos))
                {
                    if (worldState.AdjToPos(agentPos.Value, localPos))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}