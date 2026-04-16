using Godot;
using System.Collections.Generic;

namespace Catcophony.scenes.systems.pathing
{
    public partial class Path(int id, Queue<Vector2> points) : Resource
    {
        public int Id { get; set; } = id;

        public Queue<Vector2> Points { get; set; } = points;

        public bool IsShown { get; set; } = false;

        public bool IsEmpty()
        {
            return Points.Count == 0; 
        }
    }
}
