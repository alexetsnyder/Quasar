using Quasar.core.blackboard;
using Quasar.core.common;
using Quasar.core.naming;
using Quasar.data.enums;
using Quasar.scenes.common.interfaces;
using System.Linq;

namespace Quasar.core.goap.goals
{
    public partial class HasPathGoal : GoalBase
    {
        private readonly IPathingSystem _pathingSystem;

        public HasPathGoal(IPathingSystem pathingSystem)
        {
            _key = new("HasPath");
            _value = true;

            _pathingSystem = pathingSystem;
        }

        public override bool Satisify(WorldState worldState, Blackboard<int> blackboard)
        {
            var worldStateBlackboard = worldState.GetBlackboard();

            if (worldStateBlackboard.TryGetVector2(Constants.Names.Position, out var agentPos))
            {
                if (blackboard.TryGetInt(ActionId - 1, out var currentWorkTypeInt))
                {
                    var currentWorkTypeFastName = new FastName(((WorkType)currentWorkTypeInt).ToString());

                    if (blackboard.TryGetPath(ActionId - 1, out var selectedPath))
                    {
                        return true;
                    }
                    else if (blackboard.TryGetWork(ActionId - 1, out var currentWork))
                    {
                        foreach (var adjPos in currentWork.AdjPos)
                        {
                            if (adjPos.IsEqualApprox(agentPos))
                            {
                                blackboard.Set(ActionId, _pathingSystem.CreateEmptyPath());
                                return true;
                            }

                            var path = _pathingSystem.FindPath(agentPos, adjPos);
                            if (path != null)
                            {
                                blackboard.Set(ActionId, path);
                                return true;
                            }
                        }
                    }

                    if (worldStateBlackboard.TryGetWorkList(currentWorkTypeFastName, out var workList))
                    {
                        if (workList.Count > 0)
                        {
                            foreach (var work in workList.ToDictionary(w => w, w => w.AdjPos ?? []))
                            {
                                foreach (var adjPos in work.Value)
                                {
                                    if (agentPos.IsEqualApprox(adjPos))
                                    {
                                        blackboard.Set(ActionId, work.Key);
                                        blackboard.Set(ActionId, _pathingSystem.CreateEmptyPath());
                                        return true;
                                    }

                                    var path = _pathingSystem.FindPath(agentPos, adjPos);
                                    if (path != null)
                                    {
                                        blackboard.Set(ActionId, work.Key);
                                        blackboard.Set(ActionId, path);
                                        return true;
                                    }
                                }
                            }
                        }
                    }
                }
 
            }

            return false;
        }
    }
}