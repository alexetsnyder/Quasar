using Catcophony.core.blackboard;
using Catcophony.core.goap.interfaces;
using Catcophony.core.naming;
using Catcophony.scenes.common.interfaces;

namespace Catcophony.core.goap.goals
{
    public partial class HasPathWaterGoal : GoalBase
    {
        private readonly IWorld _world;

        private readonly IPathingSystem _pathingSystem;

        public HasPathWaterGoal(IAction parent, IWorld world, IPathingSystem pathingSystem)
        {
            _key = new("HasPathWater");
            _value = true;

            _world = world;
            _pathingSystem = pathingSystem;
            _parentAction = parent;
        }

        public override bool Satisify(WorldState worldState, Blackboard<FastName> blackboard)
        {
            if (worldState.GetBlackboard().TryGetVector2(Constants.Names.AgentPos, out var agentPos))
            {
                var parentBlackboard = _parentAction.GetBlackboard();

                if (parentBlackboard.TryGetVector2(Constants.Names.LocalPos, out var localPos))
                {
                    var path = _pathingSystem.ShortestPath(agentPos, _world.GetAdjacentTiles(localPos));

                    if (path != null)
                    {
                        _pathingSystem.RemovePath(path.Id);

                        return true;
                    }
                }
            }

            return false;
        }
    }
}