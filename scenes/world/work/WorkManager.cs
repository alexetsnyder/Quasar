using System.Collections.Generic;

namespace Quasar.scenes.world.work
{
    public partial class WorkManager
    {
        public Work Next { get => workQueue.Dequeue(); }

        public int Count { get => workQueue.Count; }

        private Queue<Work> workQueue = new();

        public void CreateWork(Work work)
        {
            workQueue.Enqueue(work);
        }
    }
}
