using Quasar.core.blackboard;
using Quasar.core.goap.interfaces;
using Quasar.core.naming;
using Quasar.data.enums;
using System.Linq;

namespace Quasar.core.goap.goals
{
    public partial class AdjToGoal : GoalBase
    {
        public AdjToGoal(IAction parent)
        {
            _key = new("AdjTo");
            _value = true;

            _parentAction = parent;
        }

        public override bool Satisify(WorldState worldState, Blackboard<FastName> blackboard)
        {
            var worldStateBlackboard = worldState.GetBlackboard();

            if (worldStateBlackboard.TryGetVector2(Constants.Names.AgentPos, out var agentPos))
            {
                if (blackboard.TryGetInt(Constants.Names.WorkType, out var workTypeInt))
                {
                    var workType = (WorkType)workTypeInt;

                    if (blackboard.TryGetWork(Constants.Names.Work, out var work))
                    {
                        foreach (var adjPos in work.AdjPos)
                        {
                            if (adjPos.IsEqualApprox(agentPos))
                            {
                                return true;
                            }
                        }
                    }

                    if (worldStateBlackboard.TryGetWorkList(new(workType.ToString()), out var workList))
                    {
                        if (workList.Count > 0)
                        {
                            foreach (var workKVP in workList.ToDictionary(w => w, w => w.AdjPos ?? []))
                            {
                                foreach (var adjPos in workKVP.Value)
                                {
                                    if (adjPos.IsEqualApprox(agentPos))
                                    {
                                        blackboard.Set(Constants.Names.Work, workKVP.Key);
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