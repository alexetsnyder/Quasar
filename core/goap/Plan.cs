using Quasar.core.blackboard;
using Quasar.core.goap.interfaces;
using System.Collections.Generic;

namespace Quasar.core.goap
{
    public partial class Plan(Queue<IAction> actions)
    {
        public Queue<IAction> Actions { get; set; } = actions;
    }
}