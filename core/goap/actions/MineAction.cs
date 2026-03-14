using Quasar.core.blackboard;
using Quasar.core.goap.goals;
using Quasar.core.goap.interfaces;
using Quasar.core.naming;
using Quasar.data.enums;
using Quasar.scenes.common.interfaces;
using Quasar.scenes.systems.work;
using System.Collections.Generic;
using System.Linq;

namespace Quasar.core.goap.actions
{
    public partial class MineAction : IAction
    {
        public readonly static FastName Name = new("MineAction");

        public int Cost { get => 1; }

        public ICommand Command { get => _work.Command; }

        private readonly Dictionary<FastName, IGoal> _preconditions = [];

        private readonly Dictionary<FastName, IGoal> _effects = [];

        private IWorkSystem _workSystem;

        private Work _work;

        public MineAction(WorkType workType, IWorkSystem workSystem)
        {
            _workSystem = workSystem;

            WorkGoal workGoal = new();
            _effects.Add(workGoal.Key, workGoal);

            AdjToGoal adjToGoal = new(workType, workSystem);
            MineWorkGoal mineWorkGoal = new(workType, workSystem);
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
    }
}