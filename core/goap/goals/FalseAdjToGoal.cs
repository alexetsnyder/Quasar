using Quasar.core.blackboard;
using Quasar.core.goap.interfaces;
using Quasar.core.naming;

namespace Quasar.core.goap.goals
{
    public partial class FalseAdjToGoal : GoalBase
    {
        public FalseAdjToGoal(IAction parent)
        {
            _key = new("AdjTo");
            _value = true;

            _parentAction = parent;
        }

        public override bool Satisify(WorldState worldState, Blackboard<FastName> blackboard)
        {
            return false;
        }
    }
}