using Quasar.core.goap.goals;
using Quasar.core.naming;

namespace Quasar.core.goap.actions
{
    public partial class HaulAction : ActionBase
    {
        public override FastName Name { get => _name; }

        public override int Cost { get => 1; }

        private readonly FastName _name = new("HaulAction");

        public HaulAction()
        {
            WorkGoal workGoal = new();
            _effects.Add(workGoal);
      
            HaulWorkGoal haulWorkGoal = new();
            AdjToGoal adjToGoal = new();
            HasItemGoal hasItemGoal = new();
            _preconditions.Add(haulWorkGoal);
            _preconditions.Add(adjToGoal);
            _preconditions.Add(hasItemGoal);
        }
    }
}