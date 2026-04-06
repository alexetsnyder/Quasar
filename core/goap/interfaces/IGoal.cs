using Catcophony.core.naming;

namespace Catcophony.core.goap.interfaces
{
    public interface IGoal
    {
        public FastName Key { get; }

        public bool Value { get; }

        public bool Satisify(IGoal goal);

        public bool Satisify(WorldState worldState);
    }
}