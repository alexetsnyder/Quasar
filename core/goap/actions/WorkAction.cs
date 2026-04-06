using Catcophony.core.blackboard;
using Catcophony.core.enums;
using Catcophony.core.goap.goals;
using Catcophony.core.naming;

namespace Catcophony.core.goap.actions
{
    public partial class WorkAction : ActionBase
    {
        public override FastName Name { get => _name; }

        public override int Cost { get => _cost; }

        public readonly FastName _name;
        
        public readonly int _cost;

        public WorkAction(Blackboard<FastName> blackboard, FastName name, int cost, ActionType actionType)
            : base(blackboard, actionType)
        {
            _name = name;
            _cost = cost;

            var workGoal = new WorkGoal();
            _effects.Add(workGoal);

            var hasProfGoal = new HasProfGoal(_blackboard);
            var hasWorkGoal = new HasWorkGoal(_blackboard, _actionType);
            var adjToGoal = new AdjToGoal(_blackboard);
            _preconditions.Add(hasWorkGoal);
            _preconditions.Add(hasProfGoal);
            _preconditions.Add(adjToGoal);
        }
    }
}