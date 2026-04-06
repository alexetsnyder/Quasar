using Catcophony.core.blackboard;
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

        private readonly FastName _name = new("DrinkAction");

        public DrinkAction(Blackboard<FastName> blackboard, IWorld world)
            : base(blackboard, ActionType.DRINKING)
        {
            var waterGoal = new WaterGoal();
            _effects.Add(waterGoal);

            var findWaterGoal = new FindWaterGoal(_blackboard);
            var adjToWaterGoal = new AdjToGoal(_blackboard);
            _preconditions.Add(findWaterGoal);
            _preconditions.Add(adjToWaterGoal);
        }
    }
}