using Godot;
using Quasar.scenes.common.interfaces;

namespace Quasar.scenes.cats
{
    public partial class Cat : Node2D, IGameObject
    {
        public int ID { get; set; }

        public float Width { get => _catSprite.GetRect().Size.X;  }

        public float Height { get => _catSprite.GetRect().Size.Y; }

        public CatData CatData { get; private set; }

        private Sprite2D _catSprite;

        public override void _Ready()
        {
            _catSprite = GetNode<Sprite2D>("CatSprite");
            CatData = new("Fern", "Stinky Cat", "Uncomfortable", 100);
        }

        public override void _Process(double delta)
        {

        }
    }
}