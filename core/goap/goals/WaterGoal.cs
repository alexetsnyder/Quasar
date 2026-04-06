namespace Catcophony.core.goap.goals
{
    public partial class WaterGoal : GoalBase
    {
        public WaterGoal()
            : base(null)
        {
            _key = new("Water");
            _value = true;
        }

        public override bool Satisify(WorldState worldState)
        {
            throw new System.NotImplementedException();
        }
    }
}