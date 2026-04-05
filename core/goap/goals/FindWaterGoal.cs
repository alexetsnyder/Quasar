using Catcophony.core.blackboard;
using Catcophony.core.goap.interfaces;
using Catcophony.core.naming;
using Catcophony.data.enums;
using Catcophony.scenes.common.interfaces;

namespace Catcophony.core.goap.goals
{
    public partial class FindWaterGoal : GoalBase
    {
        private readonly IWorld _world;

        public FindWaterGoal(IAction parentAction, IWorld world)
        {
            _key = new("FindWater");
            _value = true;

            _parentAction = parentAction;
            _world = world;
        }

        public override bool Satisify(WorldState worldState, Blackboard<FastName> blackboard)
        {
            if (worldState.GetBlackboard().TryGetVector2(Constants.Names.AgentPos, out var agentPos))
            {
                var nearestWater = _world.SearchForNearest(agentPos, TileType.WATER);

                if (nearestWater != null)
                {
                    blackboard.Set(Constants.Names.LocalPos, nearestWater.Value);
                    return true;
                }
            }

            return false;
        }
    }
}