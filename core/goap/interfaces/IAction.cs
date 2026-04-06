using Catcophony.core.actions;
using Catcophony.core.blackboard;
using Catcophony.core.enums;
using Catcophony.core.naming;
using Godot;
using System.Collections.Generic;

namespace Catcophony.core.goap.interfaces
{
    public interface IAction
    {
        public FastName Name { get; }

        public int Cost { get; }

        public Action GetAction();

        public ActionType GetActionType();

        public Vector2? GetLocalPos();

        public Blackboard<FastName> GetBlackboard();

        public List<IGoal> GetUnsatisfiedPreconditions(WorldState worldState);

        public bool SatisfyGoal(IGoal goal);

        public bool SatisfyPreconditions(WorldState worldState);
    }
}