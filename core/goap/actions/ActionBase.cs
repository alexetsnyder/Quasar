using Quasar.core.blackboard;
using Quasar.core.goap.interfaces;
using Quasar.core.naming;
using Quasar.scenes.cats;
using System.Collections.Generic;
using System.Linq;

namespace Quasar.core.goap.actions
{
    public abstract partial class ActionBase : IAction
    {
        public int Id { get; private set; }

        public abstract FastName Name { get; }

        public abstract int Cost { get; }

        protected readonly List<IGoal> _preconditions = [];

        protected readonly List<IGoal> _effects = [];

        public void SetId(int id)
        {
            Id = id;

            foreach (var goal in _preconditions)
            {
                goal.SetActionId(id);
            }
        }

        public List<IGoal> GetUnsatisfiedPreconditions(WorldState worldState, Blackboard<int> blackboard)
        {
            return [.. _preconditions.Where(g => !g.Satisify(worldState, blackboard))];
        }

        public bool SatisfyGoal(IGoal goal)
        {
            foreach (var effect in _effects)
            {
                return effect.Satisify(goal);
            }

            return false;
        }

        public bool SatisfyPreconditions(WorldState worldState, Blackboard<int> blackboard)
        {
            foreach (var cond in _preconditions)
            {
                if (!cond.Satisify(worldState, blackboard))
                {
                    return false;
                }
            }

            return true;
        }

        public virtual void Execute(Cat cat, Blackboard<int> blackboard)
        {
            if (blackboard.TryGetWork(Id, out var work))
            {
                cat.SetWork(work);
            }
            else if (blackboard.TryGetWork(Id + 1, out work))
            {
                cat.SetWork(work);
            }
        }
    }
}