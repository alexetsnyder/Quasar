using Godot;
using Quasar.core.blackboard;
using Quasar.core.goap.interfaces;
using Quasar.core.naming;
using Quasar.data.enums;
using Quasar.scenes.common.interfaces;
using Quasar.scenes.systems.work;
using System.Collections.Generic;

namespace Quasar.core.goap.goals
{
    public partial class HasPathGoal : GoalBase
    {
        private readonly IPathingSystem _pathingSystem;

        public HasPathGoal(IAction parent, IPathingSystem pathingSystem)
        {
            _key = new("HasPath");
            _value = true;

            _pathingSystem = pathingSystem;
            _parentAction = parent;
        }

        public override bool Satisify(WorldState worldState, Blackboard<FastName> blackboard)
        {
            var worldStateBlackboard = worldState.GetBlackboard();

            if (worldStateBlackboard.TryGetVector2(Constants.Names.AgentPos, out var agentPos))
            {
                if (blackboard.TryGetBool(Constants.Names.HasPath, out var hasPath))
                {
                    return hasPath;
                }

                if (blackboard.TryGetWork(Constants.Names.Work, out var work))
                {
                    return HasPath(agentPos, work);
                }

                if (blackboard.TryGetInt(Constants.Names.WorkType, out var workTypeInt))
                {
                    var workType = (WorkType)workTypeInt;

                    if (worldStateBlackboard.TryGetWorkList(new(workType.ToString()), out var workList))
                    {
                        return HasPath(agentPos, workList);
                    }
                }
            }

            return false;
        }

        private bool HasPath(Vector2 fromPos, List<Work> workList)
        {
            var blackboard = _parentAction.GetBlackboard();

            foreach (var work in workList)
            {
                if(HasPath(fromPos, work))
                {
                    blackboard.Set(Constants.Names.Work, work);
                    return true;
                }
            }

            return false;
        }

        private bool HasPath(Vector2 fromPos, Work work)
        {
            if (work.AdjPos != null)
            {
                var path = _pathingSystem.ShortestPath(fromPos, work.AdjPos);

                if (path != null)
                {
                    return true;
                }
            }
            
            return false;
        }
    }
}