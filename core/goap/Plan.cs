using Quasar.core.blackboard;
using Quasar.core.goap.interfaces;
using System.Collections.Generic;

namespace Quasar.core.goap
{
    public partial class Plan(Blackboard<int> blackboard, Queue<IAction> actions)
    {
        public Blackboard<int> Blackboard { get; set; } = blackboard;

        public Queue<IAction> Actions { get; set; } = actions;
    }
}