using Quasar.core.blackboard;
using Quasar.core.goap.interfaces;
using Quasar.core.naming;

namespace Quasar.core.goap.goals
{
    public partial class HasItemGoal : GoalBase
    {
        public HasItemGoal(IAction parent) 
        {
            _key = new("HasItem");
            _value = true;

            _parentAction = parent;
        }

        public override bool Satisify(WorldState worldState, Blackboard<FastName> blackboard)
        {
            var worldStateBlackboard = worldState.GetBlackboard();

            if (worldStateBlackboard.TryGetItem(Constants.Names.AgentItem, out var item))
            {
                if (item != null)
                {
                    return true;
                }
            }

            return false;
        }
    }
}