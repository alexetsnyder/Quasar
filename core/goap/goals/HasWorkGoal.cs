using Quasar.core.blackboard;
using Quasar.core.goap.interfaces;
using Quasar.core.naming;
using Quasar.data.enums;

namespace Quasar.core.goap.goals
{
    public partial class HasWorkGoal : GoalBase
    {
        private readonly WorkType _workType;

        public HasWorkGoal(WorkType workType, IAction parent)
        {
            _key = new("HasWork");
            _value = true;

            _workType = workType;
            _parentAction = parent;
        }

        public override bool Satisify(WorldState worldState, Blackboard<FastName> blackboard)
        {
            var worldStateBlackboard = worldState.GetBlackboard();
            FastName workTypeFastName = new(_workType.ToString());

            if (worldStateBlackboard.TryGetWorkList(workTypeFastName, out var workList))
            {
                if (workList.Count > 0)
                {
                    blackboard.Set(Constants.Names.WorkType, (int)_workType);

                    return true;
                }
            }

            return false;
        }
    }
}