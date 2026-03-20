namespace Quasar.core.goap.interfaces
{
    public interface IPlanner
    {
        public Plan Plan(IAgent agent, IGoal goal);
    }
}