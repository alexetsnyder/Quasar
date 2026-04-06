using Catcophony.core.enums;
using Catcophony.core.goap.interfaces;
using Godot;
using System.Collections.Generic;

namespace Catcophony.core.actions.interfaces
{
    public interface IActionManager
    {
        public ActionType GetActionType(Vector2 localPos);

        public Action GetAction(int actionId);

        public List<Action> CheckForWork(ActionType actionType);

        public Action RegisterAction(IAction goapAction);

        public Action RegisterAction(ActionType actionType, Vector2 localPos);

        public bool AssignActions(List<Action> actions);

        public void RemoveActions(List<Vector2> points);

        public bool RemoveAction(int actionId);
    }
}