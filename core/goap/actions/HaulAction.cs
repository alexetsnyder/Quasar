using Catcophony.core.goap.goals;
using Catcophony.core.naming;
using Catcophony.core.enums;
using Catcophony.core.blackboard;

namespace Catcophony.core.goap.actions
{
    public partial class HaulAction : ActionBase
    {
        public override FastName Name { get => _name; }

        public override int Cost { get => 1; }

        private readonly FastName _name = new("HaulAction");

        public HaulAction(Blackboard<FastName> blackboard) 
            : base(blackboard, ActionType.HAULING)
        {
            var workGoal = new WorkGoal();
            _effects.Add(workGoal);

            var hasWorkGoal = new HasWorkGoal(_blackboard, ActionType.HAULING);
            var adjToGoal = new FalseAdjToGoal(_blackboard);
            var hasItemGoal = new HasItemGoal(_blackboard);
            _preconditions.Add(hasWorkGoal);
            _preconditions.Add(adjToGoal);
            _preconditions.Add(hasItemGoal);
        }
    }
}