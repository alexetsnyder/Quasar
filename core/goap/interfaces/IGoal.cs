using Quasar.core.blackboard;
using Quasar.core.naming;
using System.Collections.Generic;

namespace Quasar.core.goap.interfaces
{
    public interface IGoal
    {
        public Dictionary<FastName, bool> Goals();

        //public bool Satisfy(Blackboard blackboard);
    }
}