using Godot;
using Quasar.data.enums;
using System.Collections.Generic;

namespace Quasar.scenes.systems.selection
{
    public partial class Selection(WorkType workType, List<Vector2> points) : Resource
    {
        public WorkType WorkType { get; set; } = workType;

        public List<Vector2> Points { get; set; } = points;
    }
}
