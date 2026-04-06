using Catcophony.core.actions.interfaces;
using Catcophony.core.blackboard;
using Catcophony.core.enums;
using Catcophony.core.goap.interfaces;
using Catcophony.core.naming;
using Catcophony.scenes.common.interfaces;
using System.Collections.Generic;

namespace Catcophony.core.goap
{
    public class Leaf
    {
        public Leaf Parent { get; set; }

        public int CumulativeCost { get; set; }

        public IAction Action { get; set; }
        
        public bool IsSuccess { get; set; }
    }

    public partial class Planner(IWorld world, IActionManager actionManager, IPathingSystem pathingSystem, IItemSystem itemSystem) : IPlanner
    {
        private WorldState _worldState = null;

        private readonly IWorld _world = world;

        private readonly IActionManager _actionManager = actionManager;

        private readonly IPathingSystem _pathingSystem = pathingSystem;

        private readonly IItemSystem _itemSystem = itemSystem;

        public Plan Plan(IAgent agent, IGoal goal)
        {
            _worldState = new(agent, _world, _actionManager, _pathingSystem, _itemSystem);

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

            if (BuildPlanRec(new Blackboard<FastName>(), root, leaves, goals, nextActionId: 0))
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

        private bool BuildPlanRec(Blackboard<FastName> blackboard, Leaf current, List<Leaf> leaves, Stack<IGoal> goals, int nextActionId)
        {
            bool success = false;
            var goal = goals.Pop();

            foreach (ActionType actionType in _worldState.GetAvailableActionTypes())
            {
                var action = _worldState.BuildAction(blackboard, actionType);

                if (action.SatisfyGoal(goal))
                {
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

                    if (newGoals.Count > 0)
                    {
                        success = success || BuildPlanRec(action.GetBlackboard(), leaf, leaves, newGoals, nextActionId);
                    }
                }
            }

            return success;
        }

        private static bool AssemblePlan(Leaf leaf, Queue<IAction> plan)
        {
            while (leaf.Parent != null)
            {
                plan.Enqueue(leaf.Action);
                leaf = leaf.Parent;
            }

            return true;
        }
    }
}