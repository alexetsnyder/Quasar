using Quasar.core.blackboard;
using Quasar.core.common;
using Quasar.core.goap.goals;
using Quasar.core.goap.interfaces;
using Quasar.core.naming;
using Quasar.data.enums;
using Quasar.scenes.cats;
using Quasar.scenes.common.interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Quasar.core.goap.actions
{
    public partial class MineAction : IAction
    {
        public readonly static FastName Name = new("MineAction");

        public int Cost { get => 1; }

        private readonly Dictionary<FastName, IGoal> _preconditions = [];

        private readonly Dictionary<FastName, IGoal> _effects = [];

        public MineAction(WorkType workType, IWorkSystem workSystem)
        {
            WorkGoal workGoal = new();
            _effects.Add(workGoal.Key, workGoal);

            AdjToGoal adjToGoal = new();
            MineWorkGoal mineWorkGoal = new();
            _preconditions.Add(adjToGoal.Key, adjToGoal);
            _preconditions.Add(mineWorkGoal.Key, mineWorkGoal);
        }

        public List<IGoal> GetUnsatisfiedPreconditions(Blackboard blackboard)
        {
            return [.. _preconditions.Select(kvp => kvp.Value).Where(g => !g.Satisify(blackboard))];
        }

        public bool SatisfyGoal(IGoal goal)
        {
            if (_effects.TryGetValue(goal.Key, out IGoal effect))
            {
                return effect.Satisify(goal);
            }

            return false;
        }

        public bool SatisfyPreconditions(Blackboard blackboard)
        {
            foreach (var cond in _preconditions.Values)
            {
                if (!cond.Satisify(blackboard))
                {
                    return false;
                }
            }

            return true;
        }

        public void Execute(Cat cat, Blackboard blackboard)
        {
            if (blackboard.TryGetWork(Constants.Names.SelectedWork, out var work))
            {
                cat.SetWork([ work ]);
            }
        }
    }
}