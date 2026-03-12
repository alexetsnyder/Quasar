using Quasar.core.blackboard;
using Quasar.core.goap.interfaces;
using Quasar.core.naming;
using System.Collections.Generic;

namespace Quasar.core.goap.goals
{
    public partial class GoalBase : IGoal
    {
        protected readonly Dictionary<FastName, bool> _goals = [];

        public Dictionary<FastName, bool> Goals()
        {
            return _goals;
        }
    }
}