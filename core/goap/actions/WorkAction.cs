using Catcophony.core.enums;
using Catcophony.core.goap.goals;
using Catcophony.core.naming;
using Catcophony.scenes.common.interfaces;

namespace Catcophony.core.goap.actions
{
    public partial class WorkAction : ActionBase
    {
        public override FastName Name { get => _name; }

        public override int Cost { get => _cost; }

        public override int Ticks { get => 10; }

        public readonly FastName _name;
        
        public readonly int _cost;

        private readonly ActionType _actionType;

        public WorkAction(FastName name, int cost, ActionType actionType, IWorld world)
        {
            SetActionType(actionType);

            _name = name;
            _cost = cost;
            _actionType = actionType;

            WorkGoal workGoal = new();
            _effects.Add(workGoal);

            HasWorkGoal hasWorkGoal = new(_actionType, this);
            HasProfGoal hasProfGoal = new(this);
            AdjToGoal adjToGoal = new(this, world);
            _preconditions.Add(hasWorkGoal);
            _preconditions.Add(hasProfGoal);
            _preconditions.Add(adjToGoal);
        }
    }
}