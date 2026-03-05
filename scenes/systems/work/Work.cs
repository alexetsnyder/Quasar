using Godot;
using Quasar.data.enums;
using Quasar.scenes.systems.building;
using Quasar.scenes.systems.items;

namespace Quasar.scenes.systems.work
{
    public partial class Work(int workId, WorkType workType, Vector2 worldPos, bool isAssigned = false, Buildable buildable = null, Item item = null) : Resource
    {
        public int WorkId { get; set; } = workId;

        public WorkType WorkType { get; set; } = workType;

        public Vector2 WorldPos { get; set; } = worldPos;

        public bool IsAssigned { get; set; } = isAssigned;

        public Buildable Buildable { get; set; } = buildable;

        public Item Item { get; set; } = item;
    }
}
