using Godot;
using Godot.Collections;
using Quasar.data;
using Quasar.data.enums;
using Quasar.scenes.camera;
using Quasar.scenes.cats;
using Quasar.scenes.gui;
using Quasar.scenes.map;
using Quasar.scenes.work;
using Quasar.scenes.world;
using System.Collections.Generic;
using System.Linq;

namespace Quasar.scenes
{
    public partial class Main : Node2D
    {
        private Map _map;

        private World _world;

        private MapCamera2d _camera;

        private CanvasLayer _debugGUI;

        private CanvasLayer _gui;

        private BasicLabelDisplay _tileTypeDisplay;

        private BasicLabelDisplay _tileColorDisplay;

        private CharacterDisplay _characterDisplay;

        private Vector2 _prevCameraZoom;

        private Vector2 _prevCameraPos;

        private Cat _cat;

        private List<Work> _workList = [];

        public override void _Ready()
        {
            _debugGUI = GetNode<CanvasLayer>("DebugGUI");
            _gui = GetNode<CanvasLayer>("GUI");
            _map = GetNode<Map>("Map");
            _world = GetNode<World>("World");
            _camera = GetNode<MapCamera2d>("MapCamera2D");
            _tileTypeDisplay = GetNode<BasicLabelDisplay>("DebugGUI/TileTypeDisplay");
            _tileColorDisplay = GetNode<BasicLabelDisplay>("DebugGUI/TileColorDisplay");

            _characterDisplay = InstantiateScene<CharacterDisplay>("res://scenes/gui/character_display.tscn");
            if (_characterDisplay != null)
            {
                _gui.AddChild(_characterDisplay);
                _characterDisplay.Visible = false;
            }

            _cat = InstantiateScene<Cat>("res://scenes/cats/cat.tscn");
            if (_cat != null)
            {
                AddChild(_cat);
                var catPos = _world.PlaceCat();
                if (catPos.HasValue)
                {
                    _cat.Position = catPos.Value;
                }
                _cat.Speed = 8;
                _cat.CatClickedOn += OnCatClickedOn;
                _cat.MovedOne += OnCatMovedOne;
                _cat.PathComplete += OnCatPathComplete;
                _cat.CatWork += OnCatWork;
            }

            _map.SetProcessUnhandledInput(false);

            _camera.Position = new Vector2(_camera.WorldSize.X / 2.0f, _camera.WorldSize.Y / 2.0f);

            _prevCameraZoom = _camera.Zoom;
            _prevCameraPos = _camera.Position;
        }

        public override void _Process(double delta)
        {
            SetTyleTypeLabel();
            SetTyleColorLabel();

            CheckForWork();
        }

        public override void _Input(InputEvent @event)
        {
            if (@event.IsActionPressed("Quit"))
            {
                GetTree().Quit();
            }
            else if (@event.IsActionPressed("Map"))
            {
                ToggleMap();
            }
            else if (@event.IsActionPressed("DebugUI"))
            {
                _debugGUI.Visible = !_debugGUI.Visible;
            }
        }

        public void SetTyleTypeLabel()
        {
            var mousePos = GetLocalMousePosition();

            string tileTypeStr;
            Color? tileColor;
            if (_map.Visible)
            {
                tileTypeStr = _map.GetTileTypeStr(mousePos);
                tileColor = _map.GetTileColor(mousePos);
            }
            else
            {
                tileTypeStr = _world.GetTileTypeStr(mousePos);
                tileColor = _world.GetTileColor(mousePos);
            }

            _tileTypeDisplay.SetLabelText(tileTypeStr);
            _tileTypeDisplay.SetLabelColor((tileColor == null) ? ColorConstants.WHITE : tileColor.Value);
        }

        public void SetTyleColorLabel()
        {
            var mousePos = GetLocalMousePosition();

            string tileColorStr;
            Color? tileColor;
            if (_map.Visible)
            {
                tileColorStr = _map.GetTileColorStr(mousePos);
                tileColor = _map.GetTileColor(mousePos);
            }
            else
            {
                tileColorStr = _world.GetTileColorStr(mousePos);
                tileColor = _world.GetTileColor(mousePos);
            }

            _tileColorDisplay.SetLabelText(tileColorStr);
            _tileColorDisplay.SetLabelColor((tileColor == null) ? ColorConstants.WHITE : tileColor.Value);
        }

        private void ToggleMap()
        {
            var prevZoom = _prevCameraZoom;
            var prevPos = _prevCameraPos;

            _prevCameraZoom = _camera.Zoom;
            _prevCameraPos = _camera.Position;

            _camera.UpdateZoom(prevZoom);
            _camera.Position = prevPos;

            if (!_map.Visible)
            {
                _map.Visible = true;
                _map.SetProcessUnhandledInput(true);
                _world.Visible = false;
                _world.SetProcessUnhandledInput(false);
                _cat.Visible = false;
            }
            else
            {
                _map.Visible = false;
                _map.SetProcessUnhandledInput(false);
                _world.Visible = true;
                _world.SetProcessUnhandledInput(true);
                _cat.Visible = true;
            }

            SetTyleTypeLabel();
            SetTyleColorLabel();
        }

        private static T InstantiateScene<T>(string path)  where T : class    
        {
            var sceneResource = ResourceLoader.Load<PackedScene>(path);
            if (sceneResource == null)
            {
                return default;
            }
            else 
            {
                return sceneResource.Instantiate<T>();
            }
        }

        private void CheckForWork()
        {
            if (_workList.Count > 0 && _cat.CanWork())
            {
                var path = ShortestPath(out Vector2? workPos);
                if (workPos.HasValue)
                {
                    StartWork(WorkType.DIGGING, workPos.Value, path);
                }
            }
        }

        private List<Vector2> ShortestPath(out Vector2? workPos)
        {
            List<Vector2> shortestPath = [];
            int minPathCount = int.MaxValue;
            workPos = null;

            foreach (var worldPos in _workList.Select(w => w.WorldPos))
            {
                foreach (var adjPos in _world.GetAdjacentTiles(worldPos, true))
                {
                    if (_cat.Position.IsEqualApprox(adjPos))
                    {
                        workPos = worldPos;
                        return [];
                    }
                    
                    var path = _world.FindPath(_cat.Position, adjPos);

                    if (path.Count == 0)
                    {
                        continue;
                    }

                    if (path.Count < minPathCount)
                    {
                        workPos = worldPos;
                        minPathCount = path.Count;
                        shortestPath = path;
                    }
                }
            }

            return shortestPath;
        }

        private void StartWork(WorkType workType, Vector2 workPos, List<Vector2> path)
        {
            _cat.SetPath(path);
            _world.ShowPath(path);
            _cat.SetWork(workType, workPos);
        }

        private void OnToolBarSelectPressed()
        {
            _world.SetSelectionState(SelectionState.SINGLE);
        }

        private void OnToolBarDigPressed()
        {
            _world.SetSelectionState(SelectionState.DIGGING);
        }

        private void OnToolBarBuildPressed()
        {
            _world.SetSelectionState(SelectionState.BUILDING);
        }

        private void OnToolBarFarmPressed()
        {
            _world.SetSelectionState(SelectionState.FARMING);
        }

        private void OnToolBarCancelPressed()
        {
            _world.SetSelectionState(SelectionState.CANCEL);
        }

        private void OnCatClickedOn(Cat cat)
        {
            _characterDisplay.FillUI(cat.CatData);
            _characterDisplay.Visible = true;
        }

        private void OnWorldTileSelected(Vector2 tileSelected)
        {
            if (!_cat.IsWorking)
            {
                var path = _world.FindPath(_cat.Position, tileSelected);
                _world.ShowPath(path);
                GD.Print($"Path Count: {path.Count}");
                _cat.SetPath(path);
            } 
        }

        private void OnCatMovedOne(Vector2 lastPos, Vector2 newPos)
        {
            _world.ShowTile(lastPos);
            _world.HideTile(newPos);
        }

        private void OnCatPathComplete()
        {
            _world.ClearPath();
        }

        private void OnWorldCreateWork(Array<Work> workArray)
        {
            _workList.AddRange(workArray);
            GD.Print($"WorkList: {_workList.Count}");
        }

        private void OnWorldCancelWork(Array<Vector2> worldPosArray)
        {
            List<Work> removeList = [];

            foreach (var pos in worldPosArray)
            {
                foreach (var work in _workList)
                {
                    if (work.WorldPos == pos)
                    {
                        removeList.Add(work);
                        break;
                    }
                }
            }

            foreach (var work in removeList)
            {
                _workList.Remove(work);
            }

            GD.Print($"WorkList: {_workList.Count}");
        }

        private void OnCatWork(Cat cat, Vector2 workPos)
        {
            _world.Dig(workPos);
            cat.CompleteWork();
            var work = _workList.First(w =>  w.WorldPos == workPos);
            _workList.Remove(work);
        }
    }
}
