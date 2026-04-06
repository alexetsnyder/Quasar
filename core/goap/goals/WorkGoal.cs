namespace Catcophony.core.goap.goals
{
    public partial class WorkGoal : GoalBase
    {
        public WorkGoal()
            : base(null)
        {
            _key = new("HasWorked");
            _value = true;
        }

        public override bool Satisify(WorldState worldState)
        {
            throw new System.NotImplementedException();
        }
    }
}