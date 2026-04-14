using Catcophony.core.actions.commands;
using Catcophony.core.actions.interfaces;
using Catcophony.core.components;
using Catcophony.core.enums;
using Catcophony.core.goap.interfaces;
using Godot;
using System.Collections.Generic;
using System.Linq;

namespace Catcophony.core.actions
{
    [GlobalClass]
    public partial class ActionManager : Node, IActionManager
    {
        [Export]
        public CommandFactory CommandFactory { get; set; }

        private readonly UniqueIdComponent _uniqueIdComponent = new();

        private readonly Dictionary<int, Action> _actions = [];

        private static readonly object _lock = new();

        private static readonly object _assignLock = new();

        public ActionType GetActionType(Vector2 localPos)
        {
            foreach (var action in _actions.Values)
            {
                if (action.LocalPos == localPos)
                {
                    return action.ActionType;
                }
            }

            return ActionType.NONE;
        }

        public Action GetAction(int actionId)
        {
            return _actions[actionId];
        }

        public List<Action> CheckForWork(ActionType actionType)
        {
            List<Action> actions = [];

            foreach (var action in _actions.Values.Where(a => !a.IsAssinged))
            {
                if (action.ActionType == actionType)
                {
                    actions.Add(action);
                }
            }

            return actions;
        }

        public Action RegisterAction(IAction goapAction)
        {
            lock (_lock)
            {
                var actionType = goapAction.GetActionType();
                var localPos = goapAction.GetLocalPos();

                if (actionType != ActionType.NONE && localPos != null)
                {
                    return RegisterAction(actionType, localPos.Value);
                }

                return null;
            }
        }

        public Action RegisterAction(ActionType actionType, Vector2 localPos)
        {
            lock ( _lock)
            {
                var id = _uniqueIdComponent.NextId;
                var command  = CommandFactory.BuildCommand(actionType, localPos);
                var action = new Action(id, actionType, localPos, GetTicks(actionType), command);

                _actions.Add(id, action);

                return _actions[id];
            }
        }

        public bool AssignActions(List<Action> actions)
        {
            lock (_assignLock)
            {
                bool success = true;
                List<Action> actionsToUpdate = [];

                foreach (var action in actions)
                {
                    if (action.IsAssinged)
                    {
                        success = false;
                        break;
                    }

                    actionsToUpdate.Add(action);
                }

                if (success)
                {
                    foreach (var action in actionsToUpdate)
                    {
                        action.IsAssinged = true;
                    }
                }

                return success;
            } 
        }

        public void RemoveActions(List<Vector2> points)
        {
            List<int> actionsToDelete = [];

            foreach (var action in _actions.Values)
            {
                if (points.Contains(action.LocalPos))
                {
                    actionsToDelete.Add(action.Id);
                }
            }

            foreach (var actionId in actionsToDelete)
            {
                RemoveAction(actionId);
            }
        }

        public bool RemoveAction(int actionId)
        {
            return _actions.Remove(actionId);
        }

        private static int GetTicks(ActionType actionType)
        {
            switch(actionType)
            {
                case ActionType.MINING:
                case ActionType.WOOD_CUTTING:
                case ActionType.BUILDING:
                case ActionType.FARMING:
                case ActionType.GATHERING:
                case ActionType.FISHING:
                case ActionType.DRINKING:
                    return 10;
                case ActionType.HAULING:
                case ActionType.GET_ITEM:
                    return 5;
                case ActionType.MOVE_TO:
                    return 0;
                default:
                    GD.Print($"Incorrect ActionType {actionType} in ActionManger::GetTicks.");
                    return 0;
            }
        }
    }
}