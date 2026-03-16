using Quasar.core.blackboard;
using Quasar.core.goap.interfaces;
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

        public bool IsEnd { get; set; }
    }

    public partial class Planner(IWorkSystem workSystem, IPathingSystem pathingSystem) : IPlanner
    {
        private WorldState _worldState = null;

        private readonly IWorkSystem _workSystem = workSystem;

        private readonly IPathingSystem _pathingSystem = pathingSystem;

        public Plan Plan(IAgent agent, IGoal goal)
        {
            _worldState = new(agent, _workSystem, _pathingSystem);

            Leaf root = new()
            {
                Parent = null,
                CumulativeCost = 0,
                Blackboard = new(_worldState.GetBlackboard()),
                Action = null,
                IsSuccess = false,
                IsEnd = false,
            };

            List<Leaf> leaves = [];
            if (BuildPlanRec(root, leaves, goal))
            {
                Queue<IAction> minCostPlan = [];
                int minCost = int.MaxValue;
                Blackboard blackboard = null;

                foreach (var leaf in leaves)
                {
                    if (leaf.IsEnd && leaf.IsSuccess && leaf.CumulativeCost < minCost)
                    {
                        AssemblePlan(leaf, minCostPlan);
                        minCost = leaf.CumulativeCost;
                        blackboard = leaf.Blackboard;
                    }
                }

                return new(blackboard, minCostPlan);
            }

            return null;
        }

        private bool BuildPlanRec(Leaf current, List<Leaf> leaves, IGoal goal)
        {
            bool success = false;

            foreach (var action in _worldState.AvailableActions)
            {
                bool actionSuccess = false;

                if (action.SatisfyGoal(goal))
                {
                    actionSuccess = true;

                    Leaf leaf = new()
                    {
                        Parent = current,
                        CumulativeCost = current.CumulativeCost + action.Cost,
                        Blackboard = new(current.Blackboard),
                        Action = action,
                        IsSuccess = false,
                        IsEnd = false,
                    };

                    leaves.Add(leaf);

                    if (!action.SatisfyPreconditions(leaf.Blackboard))
                    {
                        var preconditons = action.GetUnsatisfiedPreconditions(leaf.Blackboard);

                        foreach (var precondition in preconditons)
                        {
                            if (!BuildPlanRec(leaf, leaves, precondition))
                            {
                                actionSuccess = false;
                                break;
                            }
                        }
                    }
                    else
                    {
                        leaf.IsEnd = true;
                        leaf.IsSuccess = true;
                    }
                }

                success = success || actionSuccess;
            }

            return success;
        }

        private static void AssemblePlan(Leaf leaf, Queue<IAction> plan)
        {
            while (leaf.Parent != null)
            {
                plan.Enqueue(leaf.Action);
                leaf = leaf.Parent;
            }
        }
    }
}