using Quasar.core.blackboard;
using Quasar.core.common;
using Quasar.core.goap.goals;
using Quasar.core.naming;
using Quasar.data.enums;
using Quasar.scenes.cats;
using Quasar.scenes.common.interfaces;
using Quasar.scenes.systems.work.commands;

namespace Quasar.core.goap.actions
{
    public partial class MoveToAction : ActionBase
    {
        public override FastName Name { get => _name; }

        public override int Cost { get => 2; }

        private readonly FastName _name = new("MoveToAction");

        private readonly IPathingSystem _pathingSystem;

        public MoveToAction(IPathingSystem pathingSystem)
        {
            _pathingSystem = pathingSystem;

            AdjToGoal adjToGoal = new();
            _effects.Add(adjToGoal);

            HasPathGoal hasPathGoal = new(_pathingSystem);
            _preconditions.Add(hasPathGoal);
        }

        public override void Execute(Cat cat, Blackboard<int> blackboard)
        {
            if (blackboard.TryGetPath(Id, out var path))
            {
                var command = new MoveToCommand(path);
                command.Execute(cat);
            }   
        }
    }
}