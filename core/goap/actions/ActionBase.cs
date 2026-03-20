using Quasar.core.blackboard;
using Quasar.core.goap.interfaces;
using Quasar.core.naming;
using Quasar.scenes.cats;
using Quasar.scenes.common.interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Quasar.core.goap.actions
{
    public abstract partial class ActionBase : IAction
    {
        public int Id { get; private set; }

        public abstract FastName Name { get; }

        public virtual bool SkipAssign { get => false; }

        public abstract int Cost { get; }

        protected Blackboard<FastName> _blackboard = new();

        protected IAction _parent;

        protected IAction _child;

        protected readonly List<IGoal> _preconditions = [];

        protected readonly List<IGoal> _effects = [];

        public Blackboard<FastName> GetBlackboard()
        {
            return _blackboard;
        }

        public void SetId(int id)
        {
            Id = id;

            foreach (var goal in _preconditions)
            {
                goal.SetActionId(id);
            }
        }

        public void SetPreconditions(List<IGoal> preconditions)
        {
            _preconditions.AddRange(preconditions);
        }

        public void SetEffects(List<IGoal> effects)
        {
            effects.AddRange(effects);
        }

        public virtual void LinkParent(IAction parent)
        {
            _parent = parent;
            if (_parent != null)
            {
                _parent.LinkChild(this);
            }    
        }

        public void LinkChild(IAction child)
        {
            _child = child;
        }

        public List<IGoal> GetUnsatisfiedPreconditions(WorldState worldState)
        {
            return [.. _preconditions.Where(g => !g.Satisify(worldState, _blackboard))];
        }

        public bool SatisfyGoal(IGoal goal)
        {
            foreach (var effect in _effects)
            {
                return effect.Satisify(goal);
            }

            return false;
        }

        public bool SatisfyPreconditions(WorldState worldState)
        {
            foreach (var cond in _preconditions)
            {
                if (!cond.Satisify(worldState, _blackboard))
                {
                    return false;
                }
            }

            return true;
        }

        public bool Assign(IWorkSystem workSytem)
        {
            if (_blackboard.TryGetWork(Constants.Names.Work, out var work) || 
                _child.GetBlackboard().TryGetWork(Constants.Names.Work, out work))
            {
                return workSytem.AssignWork(work);
            }

            return false;
        }

        public virtual void Execute(Cat cat)
        {
            if (_blackboard.TryGetWork(Constants.Names.Work, out var work) ||
                _child.GetBlackboard().TryGetWork(Constants.Names.Work, out work))
            {
                cat.SetWork(work);
            }
        }
    }
}