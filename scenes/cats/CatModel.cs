using Catcophony.core.enums;
using Catcophony.core.reflection.attributes;
using Godot;

namespace Catcophony.scenes.cats
{
    public partial class CatModel
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string Feelings { get; set; }

        [StatusAttribute]
        public int Health { get; set; }

        [StatusAttribute]
        public int Stamina { get; set; }

        [StatusAttribute]
        public int Hunger { get; set; }

        [StatusAttribute]
        public int Thirst { get; set; }

        [StatusAttribute]
        public ActionType ActionType { get; set; }

        [StatusAttribute]
        public Vector2? WorkPos { get; set; }
    }
}