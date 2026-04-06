using Catcophony.core.blackboard;
using Catcophony.core.naming;

namespace Catcophony.core.goap.goals
{
    public partial class HasItemGoal : GoalBase
    {
        public HasItemGoal(Blackboard<FastName> blackboard) 
            : base(blackboard)
        {
            _key = new("HasItem");
            _value = true;
        }

        public override bool Satisify(WorldState worldState)
        {
            var item = worldState.GetAgentItem();

            if (item != null)
            {
                return true;
            }

            return false;
        }
    }
}