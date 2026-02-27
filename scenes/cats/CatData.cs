using Godot;
using Quasar.data.enums;

namespace Quasar.scenes.cats
{
    public partial class CatData(string name, string description, string feelings, int health, WorkType workType)
    {
        public string Name { get; set; } = name;

        public string Description { get; set; } = description;

        public string Feelings { get; set; } = feelings;

        public int Health { get; set; } = health;

        public WorkType WorkType { get; set; } = workType;

        public Vector2? WorkPos { get; set; } = null;
    }
}