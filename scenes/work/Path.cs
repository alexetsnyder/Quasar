using Godot;
using System.Collections.Generic;

namespace Quasar.scenes.work
{
    public partial class Path(int id, Queue<Vector2> points) : Resource
    {
        public int Id { get; set; } = id;

        public Queue<Vector2> Points { get; set; } = points;
    }
}
