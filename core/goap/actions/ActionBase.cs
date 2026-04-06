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
        public abstract FastName Name { get; }

        public abstract int Cost { get; }

        protected Blackboard<FastName> _blackboard;

        protected readonly List<IGoal> _preconditions = [];

        protected readonly List<IGoal> _effects = [];

        protected ActionType _actionType;

        public ActionBase(Blackboard<FastName> blackboard, ActionType actionType)
        {
            CopyBlackboard(blackboard);
            SetActionType(actionType);
        }

        private void CopyBlackboard(Blackboard<FastName> blackboard)
        {
            _blackboard = new Blackboard<FastName>(blackboard);
        }

        private void SetActionType(ActionType actionType)
        {
            _blackboard.Set(Constants.Names.ActionType, (int)actionType);
            _actionType = actionType;
        }

        public Action GetAction()
        {
            switch (_actionType)
            {
                case ActionType.MOVE_TO:
                    return null;
                default:
                    if (_blackboard.TryGetAction(Constants.Names.Action, out var action))
                    {
                        return action;
                    }
                    break;
            }
            
            return null;
        }

        public ActionType GetActionType()
        {
            if (_blackboard.TryGetInt(Constants.Names.ActionType, out var actionTypeInt))
            {
                return (ActionType)actionTypeInt;
            }

            return ActionType.NONE;
        }

        public Vector2? GetLocalPos()
        {
            if (_blackboard.TryGetVector2(Constants.Names.LocalPos, out var localPos))
            {
                return localPos;
            }

            return null;
        }

        public Blackboard<FastName> GetBlackboard()
        {
            return _blackboard;
        }

        public List<IGoal> GetUnsatisfiedPreconditions(WorldState worldState)
        {
            return [.. _preconditions.Where(g => !g.Satisify(worldState))];
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
                if (!cond.Satisify(worldState))
                {
                    return false;
                }
            }

            return true;
        }
    }
}