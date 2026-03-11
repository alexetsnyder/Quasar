using Quasar.core.blackboard;
using Quasar.core.common;
using Quasar.scenes.cats;
using Quasar.scenes.common.interfaces;

namespace Quasar.core.goap
{
    public partial class WorldState
    {
        private readonly Blackboard blackboard = new();

        private readonly Cat _cat;

        private readonly IWorkSystem _workSystem;

        public WorldState(Cat cat, IWorkSystem workSystem) 
        { 
            _cat = cat;
            _workSystem = workSystem;

            blackboard.Set(Constants.Names.Position, _cat.Position);

            var workInfo = _workSystem.CheckForWork(_cat);

            if (workInfo == null)
            {
                blackboard.Set(Constants.Names.HasWork, false);
                blackboard.Set(Constants.Names.IsAdjToWork, false);
                blackboard.Set(Constants.Names.HasPath, false);
            }
            else if (workInfo.Item2 == null)
            {
                blackboard.Set(Constants.Names.HasWork, true);
                blackboard.Set(Constants.Names.IsAdjToWork, false);
                blackboard.Set(Constants.Names.HasPath, false);
            }
            else if (workInfo.Item2.IsEmpty())
            {
                blackboard.Set(Constants.Names.HasWork, true);
                blackboard.Set(Constants.Names.IsAdjToWork, true);
                blackboard.Set(Constants.Names.HasPath, false);
            }
            else
            {
                blackboard.Set(Constants.Names.HasWork, true);
                blackboard.Set(Constants.Names.IsAdjToWork, false);
                blackboard.Set(Constants.Names.HasPath, true);
            }
        }
    }
}