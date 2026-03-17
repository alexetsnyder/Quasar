using Godot;
using Quasar.core.blackboard;
using System;

namespace Quasar.core.goap.goals
{
    public partial class NullAdjToGoal : GoalBase
    {
        public NullAdjToGoal()
        {
            _key = new("AdjTo");
            _value = true;
        }

        public override bool Satisify(WorldState worldState, Blackboard<int> blackboard)
        {
            return false;
        }
    }
}