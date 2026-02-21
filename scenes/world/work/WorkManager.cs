using Godot;
using Quasar.data.enums;
using System.Collections.Generic;

namespace Quasar.scenes.world.work
{
    public partial class WorkManager
    {
        private int _nextId = 0;

        public int Count { get => workQueue.Count; }

        private Queue<Work> workQueue = new();

        public void CreateWork(WorkType workType, Vector2I worldCoord, bool isReachable = false)
        {
            var work = new Work(_nextId++, workType.ToString(), workType, worldCoord, isReachable);
            AddWork(work);
        }

        private void AddWork(Work work)
        {
            workQueue.Enqueue(work);
        }
    }
}
