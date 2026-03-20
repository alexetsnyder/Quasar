using Godot;
using Quasar.data.enums;
using Quasar.scenes.systems.pathing;
using Quasar.scenes.systems.work;

namespace Quasar.core.goap.interfaces
{
    public interface IAgent
    {
        public int Id { get; }

        public Vector2 Position { get; set; }

        public WorkType WorkType { get; }

        public void SetPath(Path path);

        public void SetWork(Work work);
    }
}