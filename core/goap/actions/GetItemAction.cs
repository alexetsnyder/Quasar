using Catcophony.core.goap.goals;
using Catcophony.core.naming;
using Catcophony.core.enums;
using Catcophony.scenes.common.interfaces;

namespace Catcophony.core.goap.actions
{
    public partial class GetItemAction : ActionBase
    {
        public override FastName Name { get => _name; }

        public override int Cost { get => 1; }

        public override int Ticks { get => 5; }

        private readonly FastName _name = new("GetItemAction");

        public GetItemAction(IWorld world)
        {
            SetActionType(ActionType.GET_ITEM);

            HasItemGoal hasItemGoal = new(this);
            _effects.Add(hasItemGoal);

            HasWorkGoal hasWorkGoal = new(ActionType.GET_ITEM, this);
            AdjToGoal adjToGoal = new(this, world);
            _preconditions.Add(hasWorkGoal);
            _preconditions.Add(adjToGoal);
        }
    }
}