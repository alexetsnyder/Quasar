using Quasar.core.blackboard;

namespace Quasar.core.goap.goals
{
    public partial class NullGoal : GoalBase
    {
        public NullGoal()
        {
            _key = new("NullGoal");
            _value = false;
        }

        public override bool Satisify(WorldState worldState, Blackboard<int> blackboard)
        {
            return false;
        }
    }
}