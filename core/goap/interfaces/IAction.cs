using Quasar.core.blackboard;
using Quasar.core.naming;
using Quasar.scenes.cats;
using Quasar.scenes.common.interfaces;
using System.Collections.Generic;

namespace Quasar.core.goap.interfaces
{
    public interface IAction
    {
        public int Id { get; }

        public FastName Name { get; }

        public int Cost { get; }

        public bool SkipAssign { get; }

        public Blackboard<FastName> GetBlackboard();

        public void SetId(int id);

        public void SetPreconditions(List<IGoal> preconditions);

        public void SetEffects(List<IGoal> effects);

        public void LinkParent(IAction parent);

        public void LinkChild(IAction child);

        public List<IGoal> GetUnsatisfiedPreconditions(WorldState worldState);

        public bool SatisfyGoal(IGoal goal);

        public bool SatisfyPreconditions(WorldState worldState);

        public bool Assign(IWorkSystem workSystem);

        public void Execute(Cat cat);
    }
}