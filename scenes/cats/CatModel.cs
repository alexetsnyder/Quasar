using Godot;
using Catcophony.data.enums;
using Catcophony.core.enums;

namespace Catcophony.scenes.cats
{
    public partial class CatModel
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string Feelings { get; set; }

        public int Health { get; set; }

        public int Stamina { get; set; }

        public int Hunger { get; set; }

        public int Thirst { get; set; }

        public ActionType ActionType { get; set; }

        public Vector2? WorkPos { get; set; }
    }
}