using Catcophony.core.enums;
using Catcophony.core.goap.goals;
using Catcophony.core.goap.interfaces;
using Catcophony.core.naming;
using Catcophony.scenes.common.interfaces;

namespace Catcophony.core.goap.actions
{
    public partial class MoveToAction : ActionBase
    {
        public override FastName Name { get => _name; }

        public override int Cost { get => 2; }

        public override int Ticks { get => 0; }

        private readonly FastName _name = new("MoveToAction");

        private readonly IPathingSystem _pathingSystem;

        public MoveToAction(IWorld world, IPathingSystem pathingSystem)
        {
            SetActionType(ActionType.MOVE_TO);

            _pathingSystem = pathingSystem;

            AdjToGoal adjToGoal = new(this, world);
            _effects.Add(adjToGoal);

            HasPathGoal hasPathGoal = new(this, world, _pathingSystem);
            _preconditions.Add(hasPathGoal);
        }
    }
}