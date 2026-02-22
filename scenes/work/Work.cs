using Godot;
using Quasar.data.enums;

namespace Quasar.scenes.work
{
    public partial class Work(string name, WorkType workType, Vector2 worldPos, bool isReachable) : Resource
    {
        public string Name { get; set; } = name;

        public WorkType WorkType { get; set; } = workType;

        public Vector2 WorldPos { get; set; } = worldPos;

        public bool IsReachable { get; set; } = isReachable;
    }
}
