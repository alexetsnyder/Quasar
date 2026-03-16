using Quasar.core.blackboard;
using Quasar.core.naming;
using Quasar.scenes.cats;
using System.Collections.Generic;

namespace Quasar.core.goap.interfaces
{
    public interface IAction
    {
        public FastName Name { get; }

        public int Cost { get; }

        public List<IGoal> GetUnsatisfiedPreconditions(Blackboard blackboard);

        public bool SatisfyGoal(IGoal goal);

        public bool SatisfyPreconditions(Blackboard blackboard);

        public void Execute(Cat cat, Blackboard blackboard);
    }
}