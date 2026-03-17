using Quasar.core.blackboard;
using Quasar.core.goap.interfaces;
using Quasar.scenes.common.interfaces;
using System;
using System.Collections.Generic;

namespace Quasar.core.goap
{
    public class Leaf
    {
        public Leaf Parent { get; set; }

        public int CumulativeCost { get; set; }

        public Blackboard<int> Blackboard { get; set; }

        public IAction Action { get; set; }

        public bool IsSuccess { get; set; }

        public bool IsEnd { get; set; }
    }

    public partial class Planner(IWorkSystem workSystem, IPathingSystem pathingSystem, IItemSystem itemSystem) : IPlanner
    {
        private WorldState _worldState = null;

        private readonly IWorkSystem _workSystem = workSystem;

        private readonly IPathingSystem _pathingSystem = pathingSystem;

        private readonly IItemSystem _itemSystem = itemSystem;

        public Plan Plan(IAgent agent, IGoal goal)
        {
            _worldState = new(agent, _workSystem, _pathingSystem, _itemSystem);

            Leaf root = new()
            {
                Parent = null,
                CumulativeCost = 0,
                Blackboard = new(),
                Action = null,
                IsSuccess = false,
                IsEnd = false,
            };

            List<Leaf> leaves = [];
            if (BuildPlanRec(root, leaves, goal, nextActionId: 0))
            {
                Queue<IAction> minCostPlan = [];
                int minCost = int.MaxValue;
                Blackboard<int> blackboard = null;

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

        private bool BuildPlanRec(Leaf current, List<Leaf> leaves, IGoal goal, int nextActionId)
        {
            bool success = false;

            foreach (WorldState.Actions actionType in Enum.GetValues(typeof(WorldState.Actions)))
            {
                if (actionType == WorldState.Actions.NONE)
                {
                    continue;
                }

                bool actionSuccess = false;

                var action = _worldState.BuildAction(actionType);
                
                if (action.SatisfyGoal(goal))
                {
                    actionSuccess = true;

                    action.SetId(nextActionId++);

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

                    if (!action.SatisfyPreconditions(_worldState, leaf.Blackboard))
                    {
                        var preconditons = action.GetUnsatisfiedPreconditions(_worldState, leaf.Blackboard);

                        foreach (var precondition in preconditons)
                        {
                            if (!BuildPlanRec(leaf, leaves, precondition, nextActionId))
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