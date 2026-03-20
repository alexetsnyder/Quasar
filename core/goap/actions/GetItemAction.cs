using Quasar.core.goap.goals;
using Quasar.core.naming;
using Quasar.data.enums;

namespace Quasar.core.goap.actions
{
    public partial class GetItemAction : ActionBase
    {
        public override FastName Name { get => _name; }

        public override int Cost { get => 1; }

        private readonly FastName _name = new("GetItemAction");

        public GetItemAction()
        {
            HasItemGoal hasItemGoal = new(this);
            _effects.Add(hasItemGoal);

            HasWorkGoal hasWorkGoal = new(WorkType.GET_ITEM, this);
            AdjToGoal adjToGoal = new(this);
            _preconditions.Add(hasWorkGoal);
            _preconditions.Add(adjToGoal);
        }
    }
}