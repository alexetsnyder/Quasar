using Catcophony.core.blackboard;
using Catcophony.core.enums;
using Catcophony.core.goap.interfaces;
using Catcophony.core.naming;

namespace Catcophony.core.goap.goals
{
    public partial class HasProfGoal : GoalBase
    {
        public HasProfGoal(IAction parent) 
        {
            _key = new("HasProf");
            _value = true;

            _parentAction = parent;
        }

        public override bool Satisify(WorldState worldState, Blackboard<FastName> blackboard)
        {
            var worldStateBlackboard = worldState.GetBlackboard();

            if (blackboard.TryGetInt(Constants.Names.ActionType, out var actionTypeInt))
            {
                var actionType = (ActionType)actionTypeInt;

                if (worldStateBlackboard.TryGetInt(Constants.Names.AgentProf, out var agentProfInt))
                {
                    var agentProf = (ActionType)agentProfInt;

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