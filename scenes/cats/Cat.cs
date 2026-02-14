using Godot;

namespace Quasar.scenes.cats
{
    public partial class Cat : Node2D
    {
        public float Width { get => _catSprite.GetRect().Size.X;  }

        public float Height { get => _catSprite.GetRect().Size.Y; }

        private Sprite2D _catSprite;

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            _catSprite = GetNode<Sprite2D>("CatSprite");
        }

        // Called every frame. 'delta' is the elapsed time since the previous frame.
        public override void _Process(double delta)
        {
        }
    }
}