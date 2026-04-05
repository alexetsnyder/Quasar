using Catcophony.core.enums;
using Godot;
using System.Collections.Generic;

namespace Catcophony.scenes.systems.selection
{
    public partial class Selection(ActionType actionType, List<Vector2> points, Rect2I selectionRect) : Resource
    {
        public ActionType actionType { get; set; } = actionType;

        public List<Vector2> Points { get; set; } = points;

        public Rect2I SelectionRect { get; set; } = selectionRect;
    }
}
