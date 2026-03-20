using Godot;
using Quasar.core.goap;
using Quasar.core.goap.goals;
using Quasar.core.goap.interfaces;
using Quasar.data.enums;
using Quasar.scenes.common.interfaces;
using Quasar.scenes.systems.items;
using Quasar.scenes.systems.pathing;
using Quasar.scenes.systems.work;
using Quasar.scenes.time;
using System.Collections.Generic;
using System.Linq;

namespace Quasar.scenes.cats
{
    public partial class Cat : Node2D, IGameObject, IAgent
    {
        #region Exports

        [Export]
        public int Speed { get; set; } = 10;

        [Export]
        public int WorkTicks { get; set; } = 10;

        #endregion

        #region Signals

        [Signal]
        public delegate void CatClickedOnEventHandler(Cat cat);

        [Signal]
        public delegate void MovedOneEventHandler(Vector2 lastPos, Vector2 newPos);

        [Signal]
        public delegate void PathCompleteEventHandler(Path path);

        [Signal]
        public delegate void CatWorkEventHandler(Cat cat, Work work);

        #endregion

        public int Id { get; set; }

        public CatData CatData { get; private set; }

        public WorkType WorkType { get => CatData.WorkType; }

        public bool IsWorking { get; private set; } = false;

        public Item Item { get; set; } = null;

        public float Width { get => _catSprite.GetRect().Size.X; }

        public float Height { get => _catSprite.GetRect().Size.Y; }

        private readonly Queue<Work> _workQueue = [];

        private IWorld _world;

        private IPathingSystem _pathingSystem;

        private TextureProgressBar _workProgress;

        private Sprite2D _catSprite;

        private bool _isMoving = false;

        private Path _movePath = null;

        private readonly Queue<Vector2> _movePathQueue = [];

        private Vector2 _lastPos = new();

        private Vector2 _nextPos = new();

        private double ElapsedWorkTime = 0.0;

        private IGoal _goal = new WorkGoal();

        private IPlanner _planner;

        private Plan _currentPlan;

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
            else
            {
                Plan();

                if (_currentPlan != null && _currentPlan.Actions.Count > 0)
                {
                    var action = _currentPlan.Actions.Dequeue();
                    action.Execute(this);
                }
            }
        }

        public void Plan()
        {
            if (_currentPlan == null || _currentPlan.Actions.Count == 0)
            {
                _currentPlan = _planner.Plan(this, _goal);
            }
        }

        public void SetCatData(CatData data)
        {
            CatData = data;
        }

        public void SetDeps(IWorld world, IPathingSystem pathingSystem, IPlanner planner)
        {
            _world = world;
            _pathingSystem = pathingSystem;
            _planner = planner;
        }

        public void SetWork(Work work)
        {
            SetWork([ work ]);
        }

        public void SetWork(List<Work> workList)
        {
            foreach (var work in workList)
            {
                _workQueue.Enqueue(work);
            }

            IsWorking = true;
            CatData.WorkPos = workList.First().LocalPos;
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

            if (_movePathQueue.Count > 0)
            {
                _pathingSystem.ShowPath(path.Id);
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
                ElapsedWorkTime %= WorkTicks;
            }
        }

        private void StartNextWork()
        {
            var work = _workQueue.Peek();
            var path = _pathingSystem.ShortestPath(Position, _world.GetAdjacentTiles(work.LocalPos, true));

            SetPath(path);
            _pathingSystem.ShowPath(path.Id);
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