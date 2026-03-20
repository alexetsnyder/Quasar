using Quasar.data.enums;
using Quasar.scenes.cats;
using Quasar.scenes.systems.pathing;
using Quasar.scenes.systems.work;
using System;
using System.Collections.Generic;

namespace Quasar.scenes.common.interfaces
{
    public interface IWorkSystem
    {
        public bool AssignWork(Work work);

        public List<Work> GetWork(WorkType workType);

        public List<Work> CheckForWork(WorkType workType);

        public Tuple<List<Work>, Path> CheckForWork(Cat cat, bool assign = true);
    }
}