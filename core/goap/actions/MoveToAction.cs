using Quasar.core.goap.goals;
using Quasar.core.goap.interfaces;
using Quasar.core.naming;
using Quasar.scenes.cats;
using Quasar.scenes.common.interfaces;
using Quasar.scenes.systems.work.commands;

namespace Quasar.core.goap.actions
{
    public partial class MoveToAction : ActionBase
    {
        public override FastName Name { get => _name; }

        public override int Cost { get => 2; }

        public override bool SkipAssign { get => true; }

        private readonly FastName _name = new("MoveToAction");

        private readonly IPathingSystem _pathingSystem;

        public MoveToAction(IPathingSystem pathingSystem)
        {
            _pathingSystem = pathingSystem;

            AdjToGoal adjToGoal = new(this);
            _effects.Add(adjToGoal);

            HasPathGoal hasPathGoal = new(this, _pathingSystem);
            _preconditions.Add(hasPathGoal);
        }

        public override void LinkParent(IAction parent)
        {
            base.LinkParent(parent);
            _blackboard = parent.GetBlackboard();
        }

        public override void Execute(Cat cat)
        {
            if (_blackboard.TryGetWork(Constants.Names.Work, out var work))
            {
                var path = _pathingSystem.ShortestPath(cat.Position, work.AdjPos);

                var command = new MoveToCommand(path);
                command.Execute(cat);
            }   
        }
    }
}