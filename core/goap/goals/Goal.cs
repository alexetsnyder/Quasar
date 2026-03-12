using Quasar.core.naming;
using System.Collections.Generic;

namespace Quasar.core.goap.goals
{
    public partial class Goal : GoalBase
    {
        public Goal(Dictionary<FastName, bool> goals)
        {
            foreach (var goal in goals)
            {
                _goals.Add(goal.Key, goal.Value);
            }
        }
    }
}