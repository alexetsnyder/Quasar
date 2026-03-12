using Quasar.core.common;

namespace Quasar.core.goap.goals
{
    public partial class WorkGoal : GoalBase
    {
        public WorkGoal()
        {
            _goals.Add(Constants.Names.HasWorked, true);
        }
    }
}