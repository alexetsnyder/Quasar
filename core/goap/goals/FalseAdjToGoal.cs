using Catcophony.core.blackboard;
using Catcophony.core.naming;

namespace Catcophony.core.goap.goals
{
    public partial class FalseAdjToGoal : GoalBase
    {
        public FalseAdjToGoal(Blackboard<FastName> blackboard)
            : base(blackboard)
        {
            _key = new("AdjTo");
            _value = true;
        }

        public override bool Satisify(WorldState worldState)
        {
            return false;
        }
    }
}