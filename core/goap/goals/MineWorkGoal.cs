using Quasar.core.blackboard;
using Quasar.data.enums;

namespace Quasar.core.goap.goals
{
    public partial class MineWorkGoal : GoalBase
    {
        public MineWorkGoal() 
        {
            _key = new("MineWork");
            _value = true;
        }

        public override bool Satisify(WorldState worldState, Blackboard<int> blackboard)
        {
            var worldStateBlackboard = worldState.GetBlackboard();

            if (worldStateBlackboard.TryGetWorkList(new(WorkType.MINING.ToString()), out var workList))
            {
                if (workList.Count > 0)
                {
                    blackboard.Set(ActionId, (int)WorkType.MINING);

                    return true;
                }
            }
            
            return false;
        }
    }
}