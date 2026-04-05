using Catcophony.core.actions;
using Catcophony.core.blackboard;
using Catcophony.core.enums;
using Catcophony.core.goap.interfaces;
using Catcophony.core.naming;
using Godot;
using System.Collections.Generic;
using System.Linq;

namespace Catcophony.core.goap.actions
{
    public abstract partial class ActionBase : IAction
    {
        public int Id { get; private set; }

        public abstract FastName Name { get; }

        public abstract int Cost { get; }

        public abstract int Ticks { get; }

        protected Blackboard<FastName> _blackboard = new();

        protected IAction _parent;

        protected IAction _child;

        protected readonly List<IGoal> _preconditions = [];

        protected readonly List<IGoal> _effects = [];

        protected void SetActionType(ActionType actionType)
        {
            _blackboard.Set(Constants.Names.ActionType, (int)actionType);
        }

        public Action GetAction()
        {
            if (_blackboard.TryGetAction(Constants.Names.Action, out var action) ||
               (_child != null && 
                _child.GetBlackboard().TryGetAction(Constants.Names.Action, out action)))
            {
                return action;
            }

            return null;
        }

        public ActionType GetActionType()
        {
            if (_blackboard.TryGetInt(Constants.Names.ActionType, out var actionTypeInt) ||
               (_child != null &&
                _child.GetBlackboard().TryGetInt(Constants.Names.ActionType, out actionTypeInt)))
            {
                return (ActionType)actionTypeInt;
            }

            return ActionType.NONE;
        }

        public Vector2? GetLocalPos()
        {
            if (_blackboard.TryGetVector2(Constants.Names.LocalPos, out var localPos) ||
               (_child != null &&
                _child.GetBlackboard().TryGetVector2(Constants.Names.LocalPos, out localPos)))
            {
                return localPos;
            }

            return null;
        }

        public Blackboard<FastName> GetParentBlackboard()
        {
            if (_parent != null)
            {
                return _parent.GetBlackboard();
            }

            return null;
        }

        public Blackboard<FastName> GetBlackboard()
        {
            return _blackboard;
        }

        public void SetId(int id)
        {
            Id = id;

            foreach (var goal in _preconditions)
            {
                goal.SetActionId(id);
            }
        }

        public virtual void LinkParent(IAction parent)
        {
            _parent = parent;
            if (_parent != null)
            {
                _parent.LinkChild(this);
            }    
        }

        public void LinkChild(IAction child)
        {
            _child = child;
        }

        public List<IGoal> GetUnsatisfiedPreconditions(WorldState worldState)
        {
            return [.. _preconditions.Where(g => !g.Satisify(worldState, _blackboard))];
        }

        public bool SatisfyGoal(IGoal goal)
        {
            foreach (var effect in _effects)
            {
                return effect.Satisify(goal);
            }

            return false;
        }

        public bool SatisfyPreconditions(WorldState worldState)
        {
            foreach (var cond in _preconditions)
            {
                if (!cond.Satisify(worldState, _blackboard))
                {
                    return false;
                }
            }

            return true;
        }
    }
}