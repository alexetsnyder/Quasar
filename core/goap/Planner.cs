using Quasar.core.goap.actions;
using Quasar.core.goap.goals;
using Quasar.core.goap.interfaces;
using Quasar.core.naming;
using Quasar.scenes.cats;
using Quasar.scenes.common.interfaces;
using System.Collections.Generic;

namespace Quasar.core.goap
{
    public class Leaf
    {
        public Leaf Parent { get; set; }

        public int CumulativeCost { get; set; }

        public IAction Action { get; set; }

        public bool IsSuccess { get; set; }
    }

    public partial class Planner
    {
        private WorldState _worldState = null;

        List<IGoal> _goals =
        [
            new WorkGoal(),
        ];

        List<IAction> _availableActions = 
        [
            new Mine(),
            new MoveTo(),
        ];

        public Planner(Cat cat, IWorkSystem workSystem)
        {
            _worldState = new(cat, workSystem);
        }

        public Queue<IAction> Plan()
        {
            if (_worldState == null)
            {
                return [];
            }

            Leaf root = new()
            {
                Parent = null,
                CumulativeCost = 0,
                Action = null,
                IsSuccess = false,
            };

            List<Leaf> leaves = BuildPlan(root);

            Queue<IAction> minCostPlan = [];
            int minCost = int.MaxValue;

            foreach (var leaf in leaves)
            {
                if (leaf.IsSuccess && leaf.CumulativeCost < minCost)
                {
                    RebuildPlan(leaf, minCostPlan);
                    minCost = leaf.CumulativeCost;
                }    
            }

            return minCostPlan;
        }

        private List<Leaf> BuildPlan(Leaf root)
        {
            List<Leaf> plan = [];

            var currentNode = root;

            var goals = _goals[0].Goals();
            Stack<KeyValuePair<FastName, bool>> preconds = [];

            foreach (var goal in goals)
            {
                preconds.Push(goal);
            }

            while (preconds.Count > 0)
            {
                var goal = preconds.Pop();

                bool IsGoalSatisied = false;

                foreach (var action in _availableActions)
                {
                    if (action.SatisfyGoal(goal))
                    {
                        IsGoalSatisied = true;

                        Leaf leaf = new()
                        {
                            Parent = currentNode,
                            CumulativeCost = currentNode.CumulativeCost + action.Cost,
                            Action = action,
                            IsSuccess = false,
                        };

                        plan.Add(leaf);

                        currentNode = leaf;

                        foreach (var precond in action.GetPreconds())
                        {
                            if (!Satisfy(precond, _worldState))
                            {
                                preconds.Push(precond);
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

        private bool Satisfy(KeyValuePair<FastName, bool> kvp, WorldState worldState) 
        {
            var blackboard = worldState.GetBlackboard();

            if (blackboard.TryGetBool(kvp.Key, out var value))
            {
                if (kvp.Value == value)
                {
                    return true; 
                }
            }

            return false;
        }
    }
}