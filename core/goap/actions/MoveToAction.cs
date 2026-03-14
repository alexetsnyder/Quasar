using Quasar.core.blackboard;
using Quasar.core.goap.goals;
using Quasar.core.goap.interfaces;
using Quasar.core.naming;
using Quasar.data.enums;
using Quasar.scenes.common.interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Quasar.core.goap.actions
{
    public partial class MoveToAction : IAction
    {
        public readonly static FastName Name = new("MoveToAction");

        public int Cost { get => 2; }

        public ICommand Command { get => null; }

        public readonly FastName AdjPosName;

        private readonly Dictionary<FastName, IGoal> _preconditions = [];

        private readonly Dictionary<FastName, IGoal> _effects = [];

        private readonly WorkType _workType;

        private readonly IWorkSystem _workSystem;

        private readonly IPathingSystem _pathingSystem;

        public MoveToAction(WorkType workType, IWorkSystem workSystem, IPathingSystem pathingSystem)
        {
            _workType = workType;
            _workSystem = workSystem;
            _pathingSystem = pathingSystem;

            AdjToGoal adjToGoal = new();
            _effects.Add(adjToGoal.Key, adjToGoal);

            HasPathGoal hasPathGoal = new(_workType, _workSystem, _pathingSystem);
            _preconditions.Add(hasPathGoal.Key, hasPathGoal);
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
            foreach(var cond in _preconditions.Values)
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