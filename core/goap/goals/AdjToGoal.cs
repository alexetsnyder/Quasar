using Quasar.core.blackboard;
using Quasar.core.common;
using Quasar.data.enums;
using System.Linq;

namespace Quasar.core.goap.goals
{
    public partial class AdjToGoal : GoalBase
    {
        public AdjToGoal()
        {
            _key = new("AdjTo");
            _value = true;
        }

        public override bool Satisify(WorldState worldState, Blackboard<int> blackboard)
        {
            var worldStateBlackboard = worldState.GetBlackboard();

            if (worldStateBlackboard.TryGetVector2(Constants.Names.Position, out var agentPos))
            {
                if (blackboard.TryGetInt(ActionId, out var currentWorkTypeInt))
                {
                    var currentWorkType = (WorkType)currentWorkTypeInt;

                    if (blackboard.TryGetWork(ActionId, out var currentWork))
                    {
                        foreach (var adjPos in currentWork.AdjPos)
                        {
                            if (adjPos.IsEqualApprox(agentPos))
                            {
                                return true;
                            }
                        }
                    }

                    if (worldStateBlackboard.TryGetWorkList(new(currentWorkType.ToString()), out var workList))
                    {
                        if (workList.Count > 0)
                        {
                            foreach (var work in workList.ToDictionary(w => w, w => w.AdjPos ?? []))
                            {
                                foreach (var adjPos in work.Value)
                                {
                                    if (adjPos.IsEqualApprox(agentPos))
                                    {
                                        blackboard.Set(ActionId, work.Key);
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