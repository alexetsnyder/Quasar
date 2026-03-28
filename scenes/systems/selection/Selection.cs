using Godot;
using Catcophony.data.enums;
using System.Collections.Generic;

namespace Catcophony.scenes.systems.selection
{
    public partial class Selection(WorkType workType, List<Vector2> points, Rect2I selectionRect) : Resource
    {
        public WorkType WorkType { get; set; } = workType;

        public List<Vector2> Points { get; set; } = points;

        public Rect2I SelectionRect { get; set; } = selectionRect;
    }
}
