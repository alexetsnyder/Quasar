using Quasar.core.goap.goals;
using Quasar.core.naming;

namespace Quasar.core.goap.actions
{
    public partial class MineAction : ActionBase
    {
        public override FastName Name { get => _name; }

        public override int Cost { get => 1; }

        private readonly FastName _name = new("MineAction");

        public MineAction()
        {
            WorkGoal workGoal = new();
            _effects.Add(workGoal);

            MineWorkGoal mineWorkGoal = new();
            HasProfGoal hasProfGoal = new();
            AdjToGoal adjToGoal = new();
            _preconditions.Add(mineWorkGoal);
            _preconditions.Add(hasProfGoal);
            _preconditions.Add(adjToGoal);
        }
    }
}