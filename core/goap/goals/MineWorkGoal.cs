using Quasar.core.blackboard;
using Quasar.core.common;
using Quasar.core.goap.interfaces;
using Quasar.core.naming;
using Quasar.data.enums;

namespace Quasar.core.goap.goals
{
    public partial class MineWorkGoal : IGoal
    {
        public FastName Key => _key;

        public bool Value => _value;

        private readonly FastName _key = new("MineWork");

        private readonly bool _value = true;

        public bool Satisify(IGoal goal)
        {
            return (Key == goal.Key && Value == goal.Value);
        }

        public bool Satisify(Blackboard blackboard)
        {
            if (blackboard.TryGetWorkList(new(WorkType.MINING.ToString()), out var workList))
            {
                if (workList.Count > 0)
                {
                    blackboard.Set(Constants.Names.SelectedWorkType, (int)WorkType.MINING);

                    return true;
                }
                return workList.Count > 0;
            }
            
            return false;
        }
    }
}