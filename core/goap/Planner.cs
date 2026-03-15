using Quasar.core.blackboard;
using Quasar.core.goap.goals;
using Quasar.core.goap.interfaces;
using Quasar.scenes.cats;
using Quasar.scenes.common.interfaces;
using System.Collections.Generic;

namespace Quasar.core.goap
{
    public class Leaf
    {
        public Leaf Parent { get; set; }

        public int CumulativeCost { get; set; }

        public Blackboard Blackboard { get; set; }

        public IAction Action { get; set; }

        public bool IsSuccess { get; set; }
    }

    public partial class Planner
    {
        private readonly WorldState _worldState = null;

        List<IGoal> _goals =
        [
            new WorkGoal(),
        ];

        public Planner(Cat cat, IWorkSystem workSystem, IPathingSystem pathingSystem)
        {
            _worldState = new(cat, workSystem, pathingSystem);
        }

        public Plan Plan()
        {
            if (_worldState == null)
            {
                return null;
            }

            Leaf root = new()
            {
                Parent = null,
                CumulativeCost = 0,
                Blackboard = new(_worldState.GetBlackboard()),
                Action = null,
                IsSuccess = false,
            };

            List<Leaf> leaves = BuildPlan(root);

            Queue<IAction> minCostPlan = [];
            int minCost = int.MaxValue;
            Blackboard blackboard = null;

            foreach (var leaf in leaves)
            {
                if (leaf.IsSuccess && leaf.CumulativeCost < minCost)
                {
                    RebuildPlan(leaf, minCostPlan);
                    minCost = leaf.CumulativeCost;
                    blackboard = leaf.Blackboard;
                }    
            }

            return new(blackboard, minCostPlan);
        }

        private List<Leaf> BuildPlan(Leaf root)
        {
            List<Leaf> plan = [];

            var currentNode = root;

            var goal = _goals[0];
            Stack<IGoal> preconds = [];
            preconds.Push(goal);

            while (preconds.Count > 0)
            {
                var newGoal = preconds.Pop();

                bool IsGoalSatisied = false;

                foreach (var action in _worldState.AvailableActions)
                {
                    if (action.SatisfyGoal(newGoal))
                    {
                        IsGoalSatisied = true;

                        Leaf leaf = new()
                        {
                            Parent = currentNode,
                            CumulativeCost = currentNode.CumulativeCost + action.Cost,
                            Blackboard = new(currentNode.Blackboard),
                            Action = action,
                            IsSuccess = false,
                        };

                        plan.Add(leaf);

                        currentNode = leaf;

                        if (!action.SatisfyPreconditions(leaf.Blackboard))
                        {
                            var preconditons = action.GetUnsatisfiedPreconditions(leaf.Blackboard);

                            foreach (var precondition in  preconditons)
                            {
                                preconds.Push(precondition);
                            }
                        }

                        if (preconds.Count == 0)
                        {
                            leaf.IsSuccess = true;
                            currentNode = root;
                        }
                    }
                }

                if (!IsGoalSatisied)
                {
                    currentNode = root;
                }
            }

            return plan;
        }

        private void RebuildPlan(Leaf leaf, Queue<IAction> plan)
        {
            while (leaf.Parent != null)
            {
                plan.Enqueue(leaf.Action);
                leaf = leaf.Parent;
            }
        }
    }
}