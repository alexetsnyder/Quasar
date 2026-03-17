using Quasar.core.blackboard;

namespace Quasar.core.goap.goals
{
    public partial class WorkGoal : GoalBase
    {
        public WorkGoal() 
        {
            _key = new("HasWork");
            _value = true;
        }

        public override bool Satisify(WorldState worldState, Blackboard<int> blackboard)
        {
            throw new System.NotImplementedException();
        }
    }
}