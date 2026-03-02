using Godot;
using Quasar.data;
using Quasar.data.enums;
using Quasar.scenes.camera;
using Quasar.scenes.cats;
using Quasar.scenes.gui;
using Quasar.scenes.map;
using Quasar.scenes.work;
using Quasar.scenes.world;
using Quasar.system;
using System.Collections.Generic;

namespace Quasar.scenes
{
    public partial class Main : Node2D
    {
        private Map _map;

        private World _world;

        private MapCamera2d _camera;

        private SelectionSystem _selectionSystem;

        private PathingSystem _pathingSystem;

        private WorkSystem _workSystem;

        private CanvasLayer _debugGUI;

        private CanvasLayer _gui;

        private BasicLabelDisplay _tileTypeDisplay;

        private BasicLabelDisplay _tileColorDisplay;

        private ToolBarControl _toolBarControl;

        private CharacterDisplay _characterDisplay;

        private Vector2 _prevCameraZoom;

        private Vector2 _prevCameraPos;

        private List<Cat> _cats = [];

        private Cat _selectedCat = null;

        private readonly List<CatData> _catDataList = [
            new("Fern", "Black Shorthair Cat", "Playful", 100, WorkType.MINING),
            new("Fig", "Black Shorthair Cat", "Sad", 100, WorkType.BUILDING),
            new("Pepper", "Longhair Cat", "Wary", 100, WorkType.FARMING),
            new("New Year", "Russian Blue Cat", "Curious", 100, WorkType.FISHING),
        ];

        private BuildingType _currentBuildable = BuildingType.NONE;

        public override void _Ready()
        {
            _debugGUI = GetNode<CanvasLayer>("DebugGUI");
            _gui = GetNode<CanvasLayer>("GUI");
            _map = GetNode<Map>("Map");
            _world = GetNode<World>("World");
            _selectionSystem = GetNode<SelectionSystem>("SelectionSystem");
            _pathingSystem = GetNode<PathingSystem>("PathingSystem");
            _workSystem = GetNode<WorkSystem>("WorkSystem");
            _camera = GetNode<MapCamera2d>("MapCamera2D");
            _tileTypeDisplay = GetNode<BasicLabelDisplay>("DebugGUI/TileTypeDisplay");
            _tileColorDisplay = GetNode<BasicLabelDisplay>("DebugGUI/TileColorDisplay");
            _toolBarControl = GetNode<ToolBarControl>("GUI/ToolBar");

            _characterDisplay = InstantiateScene<CharacterDisplay>("res://scenes/gui/character_display.tscn");
            if (_characterDisplay != null)
            {
                _gui.AddChild(_characterDisplay);
                _characterDisplay.Visible = false;
            }

            CreateCats();

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
                GlobalSystem.Instance.Quit();
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

            _map.Visible = !_map.Visible;
            _map.SetProcessUnhandledInput(_map.Visible);

            ToggleWorld();

            SetTyleTypeLabel();
            SetTyleColorLabel();
        }

        private void ToggleWorld()
        {
            _world.Visible = !_world.Visible;
            _world.SetProcessUnhandledInput(_world.Visible);
            _selectionSystem.Visible = _world.Visible;
            _selectionSystem.SetProcessUnhandledInput(_world.Visible);
            _pathingSystem.Visible = _world.Visible;
            _cats.ForEach(c => c.Visible = _world.Visible);

            _toolBarControl.Visible = _world.Visible;
        }

        private void CreateCats()
        {
            int n = 4;
            var spawnPoints = _world.GetSpawnPoints(_world.Center, n);

            var spawnPointsStr = "";
            foreach (var spawnPoint in spawnPoints)
            {
                spawnPointsStr += spawnPoint.ToString();
            }

            if (spawnPoints.Count == n)
            {
                for (int i = 0; i < n; i++)
                {
                    var cat = InstantiateScene<Cat>("res://scenes/cats/cat.tscn");
                    if (cat != null)
                    {
                        AddChild(cat);

                        var catPos = spawnPoints[i];
                        cat.Position = catPos;
                        _world.PlaceItem(catPos);

                        cat.Speed = 8;
                        cat.SetCatData(_catDataList[i]);
                        WireCatEvents(cat);

                        _cats.Add(cat);
                    }
                }

            }
        }

        private void WireCatEvents(Cat cat)
        {
            cat.CatClickedOn += OnCatClickedOn;
            cat.MovedOne += OnCatMovedOne;
            cat.PathComplete += OnCatPathComplete;
            cat.CatWork += OnCatWork;
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
            foreach (var cat in _cats)
            {
                if (cat.CanWork())
                {
                    var workTuple = _workSystem.CheckForWork(cat);
                    if (workTuple != null)
                    {
                        StartWork(cat, workTuple.Item1, workTuple.Item2);
                    }
                }
            }
        }

        private void StartWork(Cat cat, Work work, Path path)
        {
            _pathingSystem.ShowPath(path.Id);
            cat.SetWork(work, path);
        }

        private void CancelCatWork(Vector2 workPos)
        {
            foreach (var cat in _cats)
            {
                if (!cat.CanWork() && cat.CatData.WorkPos == workPos)
                {
                    GD.Print("Cancel from CancelCatWork");
                    cat.CompleteWork();
                    break;
                }
            }
        }

        private void RemoveWork(List<Vector2> worldPosList)
        {
            _workSystem.RemoveWork(worldPosList);
        }

        private WorkType GetWorkType(SelectionState selectionState)
        {
            switch (selectionState)
            {
                case SelectionState.MINING:
                    return WorkType.MINING;
                case SelectionState.BUILDING:
                    return WorkType.BUILDING;
                case SelectionState.FARMING:
                    return WorkType.FARMING;
                case SelectionState.FISHING:
                    return WorkType.FISHING;
                default:
                    GD.Print("Incorrect SelectionState in GetWorkType");
                    return WorkType.NONE;
            }
        }

        private void OnToolBarSelectPressed()
        {
            _selectionSystem.SelectionState = SelectionState.SINGLE;
        }

        private void OnToolBarMinePressed()
        {
            _selectionSystem.SelectionState = SelectionState.MINING;
        }

        private void OnToolBarBuildPressed(int buildableType)
        {
            _currentBuildable = (BuildingType)buildableType;
            _selectionSystem.SelectionState = SelectionState.BUILDING;
        }

        private void OnToolBarFarmPressed()
        {
            _selectionSystem.SelectionState = SelectionState.FARMING;
        }

        private void OnToolBarFishPressed()
        {
            _selectionSystem.SelectionState = SelectionState.FISHING;
        }

        private void OnToolBarCancelPressed()
        {
            _selectionSystem.SelectionState = SelectionState.CANCEL;
        }

        private void OnSelectionCreated(Selection selection)
        {
            switch (selection.SelectionState)
            {
                case SelectionState.MINING:
                case SelectionState.BUILDING:
                case SelectionState.FARMING:
                case SelectionState.FISHING:
                    var workType = GetWorkType(selection.SelectionState);
                    _workSystem.CreateWork(workType, selection.Points);
                    break;
                case SelectionState.CANCEL:
                    RemoveWork(selection.Points);
                    break;
                default:
                    GD.Print("Incorrect SelectionState in OnSelectionCreated");
                    break;
            }
        }

        private void OnTileSelected(Vector2 localPos)
        {
            if (_selectedCat != null &&
                _selectedCat.CanWork() &&
                !_world.TileOccupied(localPos))
            {
                if (_selectedCat.IsMoving())
                {
                    _pathingSystem.RemovePath(_selectedCat.GetCurrentPath().Id);
                }

                var path = _pathingSystem.FindPath(_selectedCat.Position, localPos);
                _pathingSystem.ShowPath(path.Id);
                _selectedCat.SetPath(path);
            } 
        }

        private void OnCatClickedOn(Cat cat)
        {
            _selectedCat = cat;
            _characterDisplay.SetCatData(cat.CatData);
            _characterDisplay.Visible = true;
        }

        private void OnCatMovedOne(Vector2 lastPos, Vector2 newPos)
        {
            _world.PlaceItem(newPos, lastPos);
        }

        private void OnCatPathComplete(Path path)
        {
            _pathingSystem.RemovePath(path.Id);
        }

        private void OnCatWork(Cat cat, Vector2 worldPos)
        {
            var work = _workSystem.GetWork(worldPos);
            cat.CompleteWork();

            if (work != null)
            {
                _world.Work(work.WorkType, worldPos, _currentBuildable);
                _selectionSystem.Deselect(worldPos);

                switch (work.WorkType)
                {
                    case WorkType.MINING:
                        _pathingSystem.SetPointSolid(worldPos, false);
                        break;
                    case WorkType.BUILDING:
                        _pathingSystem.SetPointSolid(worldPos);
                        break;
                }

                _workSystem.RemoveWork(work);
            }
            else
            {
                GD.Print("Cancel from OnCatWork");
            }
        }
    }
}
