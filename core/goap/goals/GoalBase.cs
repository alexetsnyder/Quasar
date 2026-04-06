using Catcophony.core.blackboard;
using Catcophony.core.goap.interfaces;
using Catcophony.core.naming;

namespace Catcophony.core.goap.goals
{
    public abstract partial class GoalBase: IGoal
    {
        public FastName Key { get => _key; }

        public bool Value { get => _value; }

        protected FastName _key;

        protected bool _value;

        protected readonly Blackboard<FastName> _blackboard;

        public GoalBase(Blackboard<FastName> blackboard)
        {
            _blackboard = blackboard;
        }

        public bool Satisify(IGoal goal)
        {
            return(Key == goal.Key && Value == goal.Value);
        }

        public abstract bool Satisify(WorldState worldState);
    }
}