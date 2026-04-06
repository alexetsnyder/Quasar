using Catcophony.core.blackboard;
using Catcophony.core.enums;
using Catcophony.core.naming;

namespace Catcophony.core.goap.goals
{
    public partial class HasProfGoal : GoalBase
    {
        public HasProfGoal(Blackboard<FastName> blackboard)
            : base(blackboard)
        {
            _key = new("HasProf");
            _value = true;
        }

        public override bool Satisify(WorldState worldState)
        {
            if (_blackboard.TryGetInt(Constants.Names.ActionType, out var actionTypeInt))
            {
                var actionType = (ActionType)actionTypeInt;
                var agentProf = worldState.GetAgentProf();

                if (agentProf != ActionType.NONE)
                {
                    if (agentProf == actionType)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}