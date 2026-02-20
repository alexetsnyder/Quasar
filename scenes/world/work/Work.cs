using Godot;
using Quasar.data.enums;

namespace Quasar.scenes.world.work
{
    public partial class Work
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public WorkTypes WorkType { get; set; }

        public Vector2I WorldCoord { get; set; }

        public Work(int iD, string name, WorkTypes workType, Vector2I worldCoord)
        {
            ID = iD;
            Name = name;
            WorkType = workType;
            WorldCoord = worldCoord;
        }
    }
}
