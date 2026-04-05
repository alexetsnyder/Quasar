using Catcophony.core.blackboard;
using Catcophony.core.enums;
using Catcophony.core.goap.interfaces;
using Catcophony.core.naming;

namespace Catcophony.core.goap.goals
{
    public partial class HasWorkGoal : GoalBase
    {
        private readonly ActionType _actionType;

        public HasWorkGoal(ActionType actionType, IAction parent)
        {
            _key = new("HasWork");
            _value = true;

            _actionType = actionType;
            _parentAction = parent;
        }

        public override bool Satisify(WorldState worldState, Blackboard<FastName> blackboard)
        {
            var worldStateBlackboard = worldState.GetBlackboard();
            FastName actionTypeFastName = new(_actionType.ToString());

            if (worldStateBlackboard.TryGetActionList(actionTypeFastName, out var actionList))
            {
                if (actionList.Count > 0)
                {
                    blackboard.Set(Constants.Names.ActionType, (int)_actionType);

                    return true;
                }
            }

            return false;
        }
    }
}