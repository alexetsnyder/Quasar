using Quasar.core.blackboard;
using Quasar.data.enums;

namespace Quasar.core.goap.goals
{
    public partial class BuildWorkGoal : GoalBase
    {
        public BuildWorkGoal()
        {
            _key = new("BuildWork");
            _value = true;
        }

        public override bool Satisify(WorldState worldState, Blackboard<int> blackboard)
        {
            var worldStateBlackboard = worldState.GetBlackboard();

            if (worldStateBlackboard.TryGetWorkList(new(WorkType.BUILDING.ToString()), out var workList))
            {
                if (workList.Count > 0)
                {
                    blackboard.Set(ActionId, (int)WorkType.BUILDING);

                    return true;
                }
            }

            return false;
        }
    }
}