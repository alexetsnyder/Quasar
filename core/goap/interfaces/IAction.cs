using Quasar.core.blackboard;
using Quasar.core.naming;
using System.Collections.Generic;

namespace Quasar.core.goap.interfaces
{
    public interface IAction
    {
        public FastName Name { get; protected set; }

        public int Cost { get; set; }

        public Dictionary<FastName, bool> GetPreconds();

        public bool SatisfyPreconds(Blackboard blackboard);

        public bool SatisfyGoal(KeyValuePair<FastName, bool> goal);

        public void Excecute(Blackboard blackboard);
    }
}