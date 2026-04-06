using Catcophony.core.blackboard;
using Catcophony.core.enums;
using Catcophony.core.goap.goals;
using Catcophony.core.naming;

namespace Catcophony.core.goap.actions
{
    public partial class MoveToAction : ActionBase
    {
        public override FastName Name { get => _name; }

        public override int Cost { get => 2; }

        private readonly FastName _name = new("MoveToAction");

        public MoveToAction(Blackboard<FastName> blackboard)
            : base(blackboard, ActionType.MOVE_TO)
        {
            var adjToGoal = new AdjToGoal(_blackboard);
            _effects.Add(adjToGoal);

            var hasPathGoal = new HasPathGoal(_blackboard);
            _preconditions.Add(hasPathGoal);
        }
    }
}