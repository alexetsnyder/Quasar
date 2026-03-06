using Godot;
using Quasar.data.enums;
using Quasar.scenes.common.interfaces;

namespace Quasar.scenes.systems.work
{
    public partial class Work(int workId, WorkType workType, Vector2 localPos, ICommand command) : Resource
    {
        public int WorkId { get; set; } = workId;

        public bool IsAssigned { get; set; } = false;

        public Vector2 LocalPos { get; set; } = localPos;

        public WorkType WorkType { get; set; } = workType;

        public ICommand Command { get; set; } = command;
    }
}
