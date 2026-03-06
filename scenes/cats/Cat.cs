using Godot;
using Quasar.scenes.common.interfaces;
using Quasar.scenes.systems.pathing;
using Quasar.scenes.systems.work;
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
        public delegate void PathCompleteEventHandler(Path path);

        [Signal]
        public delegate void CatWorkEventHandler(Cat cat, Work work);

        public IWorld World { get; set; }

        public IPathingSystem PathingSystem { get; set; }

        public int ID { get; set; }

        public CatData CatData { get; private set; }

        public bool IsWorking { get; private set; } = false;

        //public int WorkId { get; set; }
        private Queue<Work> _workQueue = [];

        public float Width { get => _catSprite.GetRect().Size.X;  }

        public float Height { get => _catSprite.GetRect().Size.Y; }

        private TextureProgressBar _workProgress;

        private Sprite2D _catSprite;

        private bool _isMoving = false;

        private Path _movePath = null;

        private readonly Queue<Vector2> _movePathQueue = [];

        private Vector2 _lastPos = new();

        private Vector2 _nextPos = new();

        private double ElapsedWorkTime = 0.0;

        public override void _Ready()
        {
            _catSprite = GetNode<Sprite2D>("CatSprite");
            _workProgress = GetNode<TextureProgressBar>("WorkProgress");
            _workProgress.Visible = false;
        }

        public override void _Process(double delta)
        {
            if (_isMoving)
            {
                Move(TimeSystem.Instance.TicksPerSecond * delta);
            }
            else if (_movePathQueue.Count > 0)
            {
                _isMoving = true;
                var tileLocalPos = _movePathQueue.Dequeue();
                _lastPos = Position;
                _nextPos = new(tileLocalPos.X + Width / 2.0f, tileLocalPos.Y + Height / 2.0f);
            }
            else if (IsWorking)
            {
                Work(TimeSystem.Instance.TicksPerSecond * delta);
            }
        }

        public void SetCatData(CatData data)
        {
            CatData = data;
        }

        public void SetWork(Work work, Path path)
        {
            _workQueue.Enqueue(work);
            SetPath(path);
            IsWorking = true;
            CatData.WorkPos = work.LocalPos;
            _workProgress.Value = 0;
            _workProgress.Visible = true;
        }

        public void CompleteWork()
        {
            _workProgress.Visible = false;

            var work = _workQueue.Dequeue();

            EmitSignal(SignalName.CatWork, this, work);

            if (_workQueue.Count > 0)
            {
                StartNextWork();
            }
            else
            {
                IsWorking = false;
                CatData.WorkPos = null;
            }     
        }

        public void SetPath(Path path)
        {
            _movePath = path;
            _movePathQueue.Clear();

            foreach (var v in path.Points)
            {
                _movePathQueue.Enqueue(v);
            }
        }

        public Path GetCurrentPath()
        {
            return _movePath; 
        }

        public bool CanWork()
        {
            return !IsWorking;
        }

        public bool IsMoving()
        {
            return (_isMoving || _movePathQueue.Count > 0);
        }

        private void Move(double delta)
        {
            Position = Position.Lerp(_nextPos, (float)(delta * Speed));

            if (Position.IsEqualApprox(_nextPos))
            {
                _isMoving = false;
                EmitSignal(SignalName.MovedOne, _lastPos, Position);
                if (_movePathQueue.Count == 0)
                {
                    EmitSignal(SignalName.PathComplete, _movePath);
                }
            }
        }

        private void Work(double delta)
        {
            ElapsedWorkTime += delta;

            _workProgress.Value = ElapsedWorkTime * 10;

            if (ElapsedWorkTime >= WorkTicks)
            {
                CompleteWork();
                //EmitSignal(SignalName.CatWork, this, CatData.WorkPos.Value);
                ElapsedWorkTime %= WorkTicks;
            }
        }

        private void StartNextWork()
        {
            var work = _workQueue.Peek();
            var path = PathingSystem.ShortestPath(Position, World.GetAdjacentTiles(work.LocalPos, true));

            SetPath(path);
            CatData.WorkPos = work.LocalPos;

            _workProgress.Value = 0;
            _workProgress.Visible = true;
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