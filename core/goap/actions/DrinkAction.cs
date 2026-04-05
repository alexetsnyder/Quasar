using Catcophony.core.enums;
using Catcophony.core.goap.goals;
using Catcophony.core.naming;
using Catcophony.scenes.common.interfaces;

namespace Catcophony.core.goap.actions
{
    public partial class DrinkAction : ActionBase
    {
        public override FastName Name { get => _name; }

        public override int Cost { get => 1; }

        public override int Ticks { get => 5; }

        private readonly FastName _name = new("DrinkAction");

        public DrinkAction(IWorld world)
        {
            SetActionType(ActionType.DRINKING);

            var waterGoal = new WaterGoal();
            _effects.Add(waterGoal);

            var findWaterGoal = new FindWaterGoal(this, world);
            var adjToWaterGoal = new AdjToWaterGoal(this, world);
            _preconditions.Add(findWaterGoal);
            _preconditions.Add(adjToWaterGoal);
        }
    }
}