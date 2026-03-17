using Quasar.core.blackboard;
using Quasar.data.enums;

namespace Quasar.core.goap.goals
{
    public partial class HaulWorkGoal : GoalBase
    {
        public HaulWorkGoal()
        {
            _key = new("HaulWork");
            _value = true;
        }

        public override bool Satisify(WorldState worldState, Blackboard<int> blackboard)
        {
            var worldStateBlackboard = worldState.GetBlackboard();

            if (worldStateBlackboard.TryGetWorkList(new(WorkType.HAULING.ToString()), out var workList))
            {
                if (workList.Count > 0)
                {
                    blackboard.Set(ActionId, (int)WorkType.HAULING);

                    return true;
                }
            }

            return false;
        }
    }
}