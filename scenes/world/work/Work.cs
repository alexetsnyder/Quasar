using Godot;
using Quasar.data.enums;

namespace Quasar.scenes.world.work
{
    public partial class Work
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public WorkType WorkType { get; set; }

        public Vector2I WorldCoord { get; set; }

        public bool IsReachable { get; set; }

        public Work(int iD, string name, WorkType workType, Vector2I worldCoord, bool isReachable)
        {
            ID = iD;
            Name = name;
            WorkType = workType;
            WorldCoord = worldCoord;
            IsReachable = isReachable;
        }
    }
}
