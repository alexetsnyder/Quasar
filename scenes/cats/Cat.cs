using Godot;
using Quasar.data.enums;
using Quasar.scenes.common.interfaces;
using Quasar.scenes.time;
using System.Collections.Generic;

namespace Quasar.scenes.cats
{
    public partial class Cat : Node2D, IGameObject
    {
        [Export]
        public int Speed { get; set; } = 10;

        [Signal]
        public delegate void CatClickedOnEventHandler(Cat cat);

        [Signal]
        public delegate void MovedOneEventHandler(Vector2 lastPos, Vector2 newPos);

        [Signal]
        public delegate void PathCompleteEventHandler();

        public int ID { get; set; }

        public new Vector2 Position 
        {
            get => base.Position; 
            set
            {
                base.Position = value;
            }
        }

        public float Width { get => _catSprite.GetRect().Size.X;  }

        public float Height { get => _catSprite.GetRect().Size.Y; }

        public CatData CatData { get; private set; }

        private Sprite2D _catSprite;

        private bool _isMoving = false;

        private Queue<Vector2> _movePath = [];

        private Vector2 _lastPos = new();

        private Vector2 _nextPos = new();

        public override void _Ready()
        {
            _catSprite = GetNode<Sprite2D>("CatSprite");
            CatData = new("Fern", "Stinky Cat", "Uncomfortable", 100, WorkType.NONE.ToString());
        }

        public override void _Process(double delta)
        {
            if (_isMoving)
            {
                Move(TimeSystem.Instance.TicksPerSecond * delta);
            }
            else if (_movePath.Count > 0)
            {
                _isMoving = true;
                var tileLocalPos = _movePath.Dequeue();
                _lastPos = Position;
                _nextPos = new(tileLocalPos.X + Width / 2.0f, tileLocalPos.Y + Height / 2.0f);
            }
        }

        public void SetWork(WorkType workType)
        {
            CatData.Work = workType.ToString();
        }

        public void SetPath(List<Vector2> path)
        {
            _movePath.Clear();

            foreach (var v in path)
            {
                _movePath.Enqueue(v);
            }
        }

        private void Move(double delta)
        {
            Position = Position.Lerp(_nextPos, (float)(delta * Speed));

            if (Position.IsEqualApprox(_nextPos))
            {
                _isMoving = false;
                EmitSignal(SignalName.MovedOne, _lastPos, Position);
                if (_movePath.Count == 0)
                {
                    EmitSignal(SignalName.PathComplete);
                }
            }
        }

        private void OnCatAreaInputEvent(Viewport viewport, InputEvent @event, long shapeIdx)
        {
            if (@event is InputEventMouseButton inputEventMouseButton)
            {
                if (inputEventMouseButton.ButtonIndex == MouseButton.Left)
                {
                    if (@event.IsPressed())
                    {
                        EmitSignal(SignalName.CatClickedOn, this);
                    }
                }
            }
        }
    }
}