using Godot;
using Quasar.data.enums;
using Quasar.scenes.cats;
using Quasar.scenes.common.interfaces;
using Quasar.scenes.systems.items;
using System.Collections.Generic;

namespace Quasar.scenes.ai.goap
{
    public partial class WorldState
    {
        public WorkType agentProf { get; set; }
         
        public Vector2 agentPos { get; set; }

        public Dictionary<Vector2, WorkType> Work {  get; set; }

        public Dictionary<Vector2I, List<Item>> Items { get; set; }

        public WorldState(Cat cat, IWorkSystem workSystem, IPathingSystem pathingSystem, IItemSystem itemSystem)
        {
            agentProf = cat.CatData.WorkType;
            agentPos = cat.Position;

            foreach (var work in workSystem.GetWork(agentProf))
            {
                if (agentPos.IsEqualApprox(work.LocalPos))
                {
                    Work.Add(work.LocalPos, work.WorkType);
                }
                else
                {
                    var path = pathingSystem.FindPath(agentPos, work.LocalPos);

                    if (path != null)
                    {
                        Work.Add(work.LocalPos, work.WorkType);
                    }
                }
            }

            Items = new(itemSystem.GetAllItems());
        }
    }
}