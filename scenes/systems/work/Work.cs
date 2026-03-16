using Godot;
using Quasar.data.enums;
using Quasar.scenes.common.interfaces;
using System.Collections.Generic;

namespace Quasar.scenes.systems.work
{
    public partial class Work(int workId, WorkType workType, Vector2 localPos, ICommand command, List<Vector2> adjPos = null) : Resource
    {
        public int WorkId { get; set; } = workId;

        public bool IsAssigned { get; set; } = false;

        //public bool IsDependent { get; set; } = false;

        public Vector2 LocalPos { get; set; } = localPos;

        public WorkType WorkType { get; set; } = workType;

        public List<Vector2> AdjPos { get; set; } = adjPos;

        public ICommand Command { get; set; } = command;

        //public int LinkedWorkId { get; set; } = -1;
    }
}
