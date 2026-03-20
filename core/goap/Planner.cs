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

        public IAction Action { get; set; }

        public bool IsSuccess { get; set; }
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
                Action = null,
                IsSuccess = false,
            };

            List<Leaf> leaves = [];
            Stack<IGoal> goals = [];
            goals.Push(goal);

            if (BuildPlanRec(root, leaves, goals, nextActionId: 0))
            {
                int minCost = int.MaxValue;
                Leaf minLeaf = null;

                foreach (var leaf in leaves)
                {
                    if (leaf.IsSuccess && leaf.CumulativeCost < minCost)
                    {
                        minCost = leaf.CumulativeCost;
                        minLeaf = leaf;
                    }
                }

                if (minLeaf != null)
                {
                    Queue<IAction> minCostPlan = [];
                    if (AssemblePlan(minLeaf, minCostPlan))
                    {
                        return new Plan(minCostPlan);
                    }
                }
            }

            return null;
        }

        private bool BuildPlanRec(Leaf current, List<Leaf> leaves, Stack<IGoal> goals, int nextActionId)
        {
            bool success = false;

            while (goals.Count > 0)
            {
                var goal = goals.Pop();
                foreach (WorldState.Actions actionType in Enum.GetValues(typeof(WorldState.Actions)))
                {
                    if (actionType == WorldState.Actions.NONE)
                    {
                        continue;
                    }

                    var action = _worldState.BuildAction(actionType);

                    if (action.SatisfyGoal(goal))
                    {
                        action.SetId(nextActionId++);
                        action.LinkParent(current.Action);

                        Leaf leaf = new()
                        {
                            Parent = current,
                            CumulativeCost = current.CumulativeCost + action.Cost,
                            Action = action,
                            IsSuccess = false,
                        };

                        leaves.Add(leaf);


                        Stack<IGoal> newGoals = new(goals);                        

                        if (action.SatisfyPreconditions(_worldState))
                        {
                            if (goals.Count == 0)
                            {
                                leaf.IsSuccess = true;
                                success = true;
                            }
                        }
                        else
                        {
                            var preconditions = action.GetUnsatisfiedPreconditions(_worldState);

                            preconditions.Reverse();

                            foreach (var precondition in preconditions)
                            {
                                newGoals.Push(precondition);
                            }
                        }

                        if (!success && newGoals.Count > 0)
                        {
                            success = BuildPlanRec(leaf, leaves, newGoals, nextActionId);
                        }
                    }
                }

                if (!success)
                {
                    return false;
                }
            }

            return success;
        }

        private bool AssemblePlan(Leaf leaf, Queue<IAction> plan)
        {
            while (leaf.Parent != null)
            {
                if (!leaf.Action.SkipAssign && !leaf.Action.Assign(_workSystem))
                {
                    return false;
                }

                plan.Enqueue(leaf.Action);
                leaf = leaf.Parent;
            }

            return true;
        }
    }
}