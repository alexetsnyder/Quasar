using Quasar.core.blackboard;
using Quasar.core.common;
using Quasar.core.naming;
using System.Collections.Generic;

namespace Quasar.core.goap.goals
{
    public partial class WorkGoal
    {
        private readonly Dictionary<FastName, bool> _goals = new()
        {
            { Constants.Names.HasWorked, true },
        };

        public bool Satisfy(Blackboard blackboard)
        {
            foreach (var goal in _goals)
            {
                if (blackboard.TryGetBool(goal.Key, out bool value))
                {
                    if (goal.Value != value)
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}