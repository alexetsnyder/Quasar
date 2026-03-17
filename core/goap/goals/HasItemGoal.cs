using Quasar.core.blackboard;
using Quasar.core.common;

namespace Quasar.core.goap.goals
{
    public partial class HasItemGoal : GoalBase
    {
        public HasItemGoal() 
        {
            _key = new("HasItem");
            _value = true;
        }

        public override bool Satisify(WorldState worldState, Blackboard<int> blackboard)
        {
            var worldStateBlackboard = worldState.GetBlackboard();

            if (worldStateBlackboard.TryGetItem(Constants.Names.Item, out var item))
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