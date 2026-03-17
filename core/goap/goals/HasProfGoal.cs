using Quasar.core.blackboard;
using Quasar.core.common;
using Quasar.data.enums;

namespace Quasar.core.goap.goals
{
    public partial class HasProfGoal : GoalBase
    {
        public HasProfGoal() 
        {
            _key = new("HasProf");
            _value = true;
        }

        public override bool Satisify(WorldState worldState, Blackboard<int> blackboard)
        {
            var worldStateBlackboard = worldState.GetBlackboard();

            if (blackboard.TryGetInt(ActionId, out var currentWorkTypeInt))
            {
                var currentWorkType = (WorkType)currentWorkTypeInt;

                if (worldStateBlackboard.TryGetInt(Constants.Names.AgentWorkType, out var agentWorkTypeInt))
                {
                    var agentWorkType = (WorkType)agentWorkTypeInt;

                    if (agentWorkType == currentWorkType)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}