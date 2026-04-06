using Catcophony.core.blackboard;
using Catcophony.core.naming;
using Catcophony.data.enums;

namespace Catcophony.core.goap.goals
{
    public partial class FindWaterGoal : GoalBase
    {
        public FindWaterGoal(Blackboard<FastName> blackboard)
            : base(blackboard)
        {
            _key = new("FindWater");
            _value = true;
        }

        public override bool Satisify(WorldState worldState)
        {
            var agentPos = worldState.GetAgentPos();

            if (agentPos != null)
            {
                var nearestWater = worldState.SearchForNearest(agentPos.Value, TileType.WATER);

                if (nearestWater != null)
                {
                    _blackboard.Set(Constants.Names.LocalPos, nearestWater.Value);
                    return true;
                }
            }

            return false;
        }
    }
}