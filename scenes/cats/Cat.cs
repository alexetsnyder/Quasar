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

        [Export]
        public int WorkTicks { get; set; } = 10;

        [Signal]
        public delegate void CatClickedOnEventHandler(Cat cat);

        [Signal]
        public delegate void MovedOneEventHandler(Vector2 lastPos, Vector2 newPos);

        [Signal]
        public delegate void PathCompleteEventHandler();

        [Signal]
        public delegate void CatWorkEventHandler(Cat cat, Vector2 workPos);

        public int ID { get; set; }

        public CatData CatData { get; private set; }

        public bool IsWorking { get; private set; } = false;

        public float Width { get => _catSprite.GetRect().Size.X;  }

        public float Height { get => _catSprite.GetRect().Size.Y; }

        private TextureProgressBar _workProgress;

        private Sprite2D _catSprite;

        private bool _isMoving = false;

        private Queue<Vector2> _movePath = [];

        private Vector2 _lastPos = new();

        private Vector2 _nextPos = new();

        private double ElapsedWorkTime = 0.0;

        public override void _Ready()
        {
            _catSprite = GetNode<Sprite2D>("CatSprite");
            _workProgress = GetNode<TextureProgressBar>("WorkProgress");
            _workProgress.Visible = false;

            CatData = new("Fern", "Stinky Cat", "Warm", 100, WorkType.MINING);
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
            else if (IsWorking)
            {
                Work(TimeSystem.Instance.TicksPerSecond * delta);
            }
        }

        public void SetWork(WorkType workType, Vector2 workPos)
        {
            IsWorking = true;
            CatData.WorkPos = workPos;
            _workProgress.Value = 0;
            _workProgress.Visible = true;
        }

        public void CompleteWork()
        {
            IsWorking = false;
            _workProgress.Visible = false;
            CatData.WorkPos = null;
        }

        public void SetPath(List<Vector2> path)
        {
            _movePath.Clear();

            foreach (var v in path)
            {
                _movePath.Enqueue(v);
            }
        }

        public bool CanWork()
        {
            return !IsWorking;
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

        private void Work(double delta)
        {
            ElapsedWorkTime += delta;

            _workProgress.Value = ElapsedWorkTime * 10;

            if (ElapsedWorkTime >= WorkTicks)
            {
                EmitSignal(SignalName.CatWork, this, CatData.WorkPos.Value);
                ElapsedWorkTime %= WorkTicks;
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