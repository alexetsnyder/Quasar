using Catcophony.core.enums;
using Catcophony.core.goap.goals;
using Catcophony.core.goap.interfaces;
using Catcophony.core.naming;
using Catcophony.scenes.common.interfaces;

namespace Catcophony.core.goap.actions
{
    public partial class MoveToWaterAction : ActionBase
    {
        public override FastName Name { get => _name; }

        public override int Cost { get => 2; }

        public override int Ticks { get => 0; }

        private readonly FastName _name = new("MoveToWaterAction");

        private readonly IWorld _world;

        private readonly IPathingSystem _pathingSystem;

        public MoveToWaterAction(IWorld world, IPathingSystem pathingSystem)
        {
            SetActionType(ActionType.MOVE_TO_WATER);

            _world = world;
            _pathingSystem = pathingSystem;

            AdjToWaterGoal adjToWaterGoal = new(this, _world);
            _effects.Add(adjToWaterGoal);

            HasPathWaterGoal hasPathWaterGoal = new(this, _world, _pathingSystem);
            _preconditions.Add(hasPathWaterGoal);
        }

        public override void LinkParent(IAction parent)
        {
            base.LinkParent(parent);
            _blackboard = parent.GetBlackboard();
        }
    }
}