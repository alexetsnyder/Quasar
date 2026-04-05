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
        public int Id { get; }

        public FastName Name { get; }

        public int Cost { get; }

        public Action GetAction();

        public ActionType GetActionType();

        public Vector2? GetLocalPos();

        public Blackboard<FastName> GetParentBlackboard();

        public Blackboard<FastName> GetBlackboard();

        public void SetId(int id);

        public void LinkParent(IAction parent);

        public void LinkChild(IAction child);

        public List<IGoal> GetUnsatisfiedPreconditions(WorldState worldState);

        public bool SatisfyGoal(IGoal goal);

        public bool SatisfyPreconditions(WorldState worldState);
    }
}