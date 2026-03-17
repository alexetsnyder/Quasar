using Quasar.core.blackboard;
using Quasar.core.naming;
using Quasar.scenes.cats;
using System.Collections.Generic;

namespace Quasar.core.goap.interfaces
{
    public interface IAction
    {
        public int Id { get; }

        public FastName Name { get; }

        public int Cost { get; }

        public void SetId(int id);

        public List<IGoal> GetUnsatisfiedPreconditions(WorldState worldState, Blackboard<int> blackboard);

        public bool SatisfyGoal(IGoal goal);

        public bool SatisfyPreconditions(WorldState worldState, Blackboard<int> blackboard);

        public void Execute(Cat cat, Blackboard<int> blackboard);
    }
}