using Catcophony.core.goap.goals;
using Catcophony.core.naming;
using Catcophony.core.enums;
using Catcophony.scenes.common.interfaces;
using Catcophony.core.blackboard;

namespace Catcophony.core.goap.actions
{
    public partial class GetItemAction : ActionBase
    {
        public override FastName Name { get => _name; }

        public override int Cost { get => 1; }

        private readonly FastName _name = new("GetItemAction");

        public GetItemAction(Blackboard<FastName> blackboard)
            : base(blackboard, ActionType.GET_ITEM)
        {
            var hasItemGoal = new HasItemGoal(_blackboard);
            _effects.Add(hasItemGoal);

            var hasWorkGoal = new HasWorkGoal(_blackboard, ActionType.GET_ITEM);
            var adjToGoal = new AdjToGoal(_blackboard);
            _preconditions.Add(hasWorkGoal);
            _preconditions.Add(adjToGoal);
        }
    }
}