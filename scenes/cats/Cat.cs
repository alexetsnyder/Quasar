using Catcophony.core.actions;
using Catcophony.core.actions.interfaces;
using Catcophony.core.enums;
using Catcophony.core.goap;
using Catcophony.core.goap.interfaces;
using Catcophony.scenes.common.interfaces;
using Catcophony.scenes.systems.items;
using Catcophony.scenes.systems.pathing;
using Catcophony.scenes.time;
using Godot;
using System.Collections.Generic;
using System.Linq;

namespace Catcophony.scenes.cats
{
    public partial class Cat : Node2D, IGameObject, IAgent, IActor
    {
        #region Exports

        [Export]
        public int Speed { get; set; } = 10;

        #endregion

        #region Signals

        [Signal]
        public delegate void CatClickedOnEventHandler(Cat cat);

        [Signal]
        public delegate void MovedOneEventHandler(Vector2 lastPos, Vector2 newPos);

        [Signal]
        public delegate void PathCompleteEventHandler(Path path);

        [Signal]
        public delegate void CatActionEventHandler(Cat cat, int actionId);

        #endregion

        #region Public Variables

        public int Id { get; set; }

        public CatModel CatModel { get; private set; }

        public ActionType ActionType { get => CatModel.ActionType; }

        public bool IsActing { get; private set; } = false;

        public Item Item { get; set; } = null;

        public float Width { get => _catSprite.GetRect().Size.X; }

        public float Height { get => _catSprite.GetRect().Size.Y; }

        #endregion

        #region Private Variables

        private IGoal _currentGoal;

        private Action _currentAction;

        private IWorld _world;

        private IActionManager _actionManager;

        private IPathingSystem _pathingSystem;

        private TextureProgressBar _actionProgress;

        private Sprite2D _catSprite;

        private bool _isMoving = false;

        private Path _movePath = null;

        private readonly Queue<Vector2> _movePathQueue = [];

        private Vector2 _lastPos = new();

        private Vector2 _nextPos = new();

        private double ElapsedWorkTime = 0.0;

        private IPlanner _planner;

        private Plan _currentPlan;

        private Vector2 _spawnPoint;

        private RandomNumberGenerator _rng = new();

        private double _idleCount = 0;

        private readonly int _idleTicks = 20;

        #endregion

        #region Public Methods

        public override void _Ready()
        {
            _catSprite = GetNode<Sprite2D>("%CatSprite");
            _actionProgress = GetNode<TextureProgressBar>("%ActionProgress");
            _actionProgress.Visible = false;
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
            else if (IsActing)
            {
                Act(TimeSystem.Instance.TicksPerSecond * delta);
            }
            else
            {
                Plan();

                if (_currentPlan != null && _currentPlan.Actions.Count > 0)
                {
                    var goapAction = _currentPlan.Actions.Dequeue();
                    var action = goapAction.GetAction();
                    action ??= _actionManager.RegisterAction(goapAction);

                    if (action != null)
                    {
                        SetAction(action);
                    }
                    else
                    {
                        _currentPlan = null;
                    }
                }
                else
                {
                    _idleCount += TimeSystem.Instance.TicksPerSecond * delta;

                    if ((int)_idleCount >= _idleTicks)
                    {
                        _idleCount = 0;

                        var idleType = CatBrain.EvaulateIdle(_rng);

                        switch (idleType)
                        {
                            case IdleType.WANDER:
                                Wander();
                                break;
                            case IdleType.RETURN:
                                ReturnToSpawn();
                                break;
                            case IdleType.IDLE:
                                break;
                            default:
                                GD.Print($"Incorrect IdleType in Cat::_Process.");
                                break;
                        }
                    }  
                }
            }
        }

        public void Plan()
        {
            _currentGoal = CatBrain.EvaluateGoal(CatModel);

            if (_currentGoal != null && (_currentPlan == null || _currentPlan.Actions.Count == 0))
            {
                _currentPlan = _planner.Plan(this, _currentGoal);
                if (_currentPlan != null && _currentPlan.Actions.Count > 0)
                {
                    if (!_actionManager.AssignActions([.. _currentPlan.Actions.Select(p => p.GetAction()).Where(p => p != null)]))
                    {
                        _currentPlan = null;
                    }
                }
            }
        }

        public void SetCatModel(CatModel data)
        {
            CatModel = data;
        }

        public void SetSpawn(Vector2 spawn)
        {
            _spawnPoint = spawn;
            Position = spawn;
        }

        public void SetDeps(IWorld world, IActionManager actionManager, IPathingSystem pathingSystem, IPlanner planner)
        {
            _world = world;
            _actionManager = actionManager;
            _pathingSystem = pathingSystem;
            _planner = planner;
        }

        public bool IsMoving()
        {
            return (_isMoving || _movePathQueue.Count > 0);
        }

        public void CompleteAction()
        {
            CatModel.Thirst -= 10;

            EmitSignal(SignalName.CatAction, this, _currentAction.Id);

            _actionProgress.Visible = false;
            IsActing = false;
            CatModel.ActionPos = null;
        }

        public void Drink()
        {
            CatModel.Thirst = 100;
            GD.Print($"{CatModel.Name} drank water!");
        }

        public void SetDestination(Vector2 localPos)
        {
            var path = _pathingSystem.ShortestPath(Position, _world.GetAdjacentTiles(localPos));

            if (path != null && path.Points.Count > 0)
            {
                SetPath(path);
            }
        }

        public void SetAction(Action action)
        {
            CatModel.ActionPos = action.LocalPos;
            _actionProgress.Visible = true;
            _currentAction = action;
            IsActing = true;
        }

        #endregion

        #region Private Methods

        private void Wander()
        {
            var point = _world.TryGetWanderingPoint(tries: 10);

            if (point != null && !Position.IsEqualApprox(point.Value))
            {
                var path = _pathingSystem.FindPath(Position, point.Value);

                if (path != null)
                {
                    SetPath(path);
                }
            }
        }

        private void ReturnToSpawn()
        {
            if (!Position.IsEqualApprox(_spawnPoint))
            {
                var path = _pathingSystem.FindPath(Position, _spawnPoint);

                if (path != null)
                {
                    SetPath(path);
                }
            }
        }

        private void SetPath(Path path)
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

        private void Act(double delta)
        {
            ElapsedWorkTime += delta;

            _actionProgress.Value = ElapsedWorkTime * _currentAction.Ticks; // 10;

            if (ElapsedWorkTime >= _currentAction.Ticks)
            {
                CompleteAction();
                if (_currentAction.Ticks > 0)
                {
                    ElapsedWorkTime %= _currentAction.Ticks;
                }
                else
                {
                    ElapsedWorkTime = 0;
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

        #endregion
    }
}