using Quasar.core.blackboard;
using Quasar.core.goap.interfaces;
using Quasar.core.naming;

namespace Quasar.core.goap.goals
{
    public abstract partial class GoalBase: IGoal
    {
        public int ActionId { get; private set; }

        public FastName Key { get => _key; }

        public bool Value { get => _value; }

        protected FastName _key;

        protected bool _value;

        protected IAction _parentAction;

        public void SetActionId(int actionId)
        {
            ActionId = actionId;
        }

        public bool Satisify(IGoal goal)
        {
            return(Key == goal.Key && Value == goal.Value);
        }

        public abstract bool Satisify(WorldState worldState, Blackboard<FastName> blackboard);
    }
}