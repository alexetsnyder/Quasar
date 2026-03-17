using Quasar.core.blackboard;
using Quasar.data.enums;

namespace Quasar.core.goap.goals
{
    public partial class GetItemGoal : GoalBase
    {
        public GetItemGoal()
        {
            _key = new("GetItemGoal");
            _value = true;
        }

        public override bool Satisify(WorldState worldState, Blackboard<int> blackboard)
        {
            var worldStateBlackboard = worldState.GetBlackboard();

            if (worldStateBlackboard.TryGetWorkList(new(WorkType.GET_ITEM.ToString()), out var workList))
            {
                if (workList.Count > 0)
                {
                    blackboard.Set(ActionId, (int)WorkType.GET_ITEM);

                    return true;
                }
            }

            return false;
        }
    }
}