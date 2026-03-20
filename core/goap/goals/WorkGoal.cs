using Quasar.core.blackboard;
using Quasar.core.naming;

namespace Quasar.core.goap.goals
{
    public partial class WorkGoal : GoalBase
    {
        public WorkGoal() 
        {
            _key = new("HasWorked");
            _value = true;
        }

        public override bool Satisify(WorldState worldState, Blackboard<FastName> blackboard)
        {
            throw new System.NotImplementedException();
        }
    }
}