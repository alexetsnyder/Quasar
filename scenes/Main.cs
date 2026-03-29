using Godot;
using Catcophony.core.goap;
using Catcophony.data;
using Catcophony.data.enums;
using Catcophony.scenes.camera;
using Catcophony.scenes.cats;
using Catcophony.scenes.gui;
using Catcophony.scenes.gui.debug;
using Catcophony.scenes.gui.items;
using Catcophony.scenes.gui.toolbar;
using Catcophony.scenes.map;
using Catcophony.scenes.systems.building;
using Catcophony.scenes.systems.items;
using Catcophony.scenes.systems.pathing;
using Catcophony.scenes.systems.selection;
using Catcophony.scenes.systems.work;
using Catcophony.scenes.world;
using Catcophony.system;
using System.Collections.Generic;
using Catcophony.scenes.systems.regions;
using Catcophony.scenes.gui.world;

namespace Catcophony.scenes
{
    public partial class Main : Node2D
    {
        private Map _map;

        private World _world;

        private MapCamera2d _camera;

        private SelectionSystem _selectionSystem;

        private PathingSystem _pathingSystem;

        private BuildingSystem _buildingSystem;

        private WorkSystem _workSystem;

        private ItemSystem _itemSystem;

        private RegionSystem _regionSystem;

        private CanvasLayer _debugGUI;

        private CanvasLayer _gui;

        private BasicLabelDisplay _tileTypeDisplay;

        private BasicLabelDisplay _tileColorDisplay;

        private ToolBarControl _toolBarControl;

        private CharacterDisplay _characterDisplay;

        private InventoryControl _inventoryControl;

        private WorldInfo _worldInfoDisplay;

        private Vector2 _prevCameraZoom;

        private Vector2 _prevCameraPos;

        private readonly List<Cat> _cats = [];

        private Cat _selectedCat = null;

        private readonly List<CatData> _catDataList = [
            new("Fern", "Black Shorthair Cat", "Playful", 100, WorkType.MINING),
            new("Fig", "Black Shorthair Cat", "Sad", 100, WorkType.BUILDING),
            new("Pepper", "Longhair Cat", "Wary", 100, WorkType.FARMING),
            new("New Year", "Russian Blue Cat", "Curious", 100, WorkType.FISHING),
            new("Maslow", "Orange", "Timid", 100, WorkType.HAULING),
            new("Millo", "Orange", "Adventurous", 100, WorkType.WOOD_CUTTING),
            new("Inky", "Black", "Affectionate", 100, WorkType.GATHERING),
        ];

        public override void _Ready()
        {
            _debugGUI = GetNode<CanvasLayer>("DebugGUI");
            _gui = GetNode<CanvasLayer>("GUI");
            _map = GetNode<Map>("Map");
            _world = GetNode<World>("World");
            _selectionSystem = GetNode<SelectionSystem>("SelectionSystem");
            _pathingSystem = GetNode<PathingSystem>("PathingSystem");
            _buildingSystem = GetNode<BuildingSystem>("BuildingSystem");
            _workSystem = GetNode<WorkSystem>("WorkSystem");
            _itemSystem = GetNode<ItemSystem>("ItemSystem");
            _regionSystem = GetNode<RegionSystem>("RegionSystem");
            _camera = GetNode<MapCamera2d>("MapCamera2D");
            _tileTypeDisplay = GetNode<BasicLabelDisplay>("DebugGUI/TileTypeDisplay");
            _tileColorDisplay = GetNode<BasicLabelDisplay>("DebugGUI/TileColorDisplay");
            _toolBarControl = GetNode<ToolBarControl>("GUI/ToolBar");

            _characterDisplay = GlobalSystem.Instance.InstantiateScene<CharacterDisplay>("res://scenes/gui/character_display.tscn");
            if (_characterDisplay != null)
            {
                _gui.AddChild(_characterDisplay);
                _characterDisplay.Visible = false;
            }

            _inventoryControl = GlobalSystem.Instance.InstantiateScene<InventoryControl>("res://scenes/gui/items/inventory_control.tscn");
            if (_inventoryControl != null)
            {
                _gui.AddChild(_inventoryControl);
                _inventoryControl.Visible = false;
            }

            CreateCats();

            _worldInfoDisplay = GlobalSystem.Instance.InstantiateScene<WorldInfo>("res://scenes/gui/world/world_info.tscn");
            if (_worldInfoDisplay != null)
            {
                _gui.AddChild(_worldInfoDisplay);
                _worldInfoDisplay.Visible = false;
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
            else if (@event.IsActionPressed("SwitchBuildable"))
            {
                if (_selectionSystem.WorkType == WorkType.BUILDING)
                {
                    _buildingSystem.NextBuildable();
                }
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
            int n = _catDataList.Count;
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
                    var cat = GlobalSystem.Instance.InstantiateScene<Cat>("res://scenes/cats/cat.tscn");
                    if (cat != null)
                    {
                        AddChild(cat);

                        cat.Id = i;

                        var newPlanner = new Planner(_workSystem, _pathingSystem, _itemSystem);
                        cat.SetDeps(_world, _pathingSystem, newPlanner);

                        var catPos = spawnPoints[i];
                        cat.Position = catPos;
                        PlaceCat(catPos);

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

        private void PlaceCat(Vector2 newPos, Vector2? lastPos = null)
        {
            _world.PlaceItem(newPos, lastPos);
        }

        private void RemoveWork(List<Vector2> worldPosList)
        {
            _workSystem.RemoveWork(worldPosList);
        }

        private void OnToolBarSelectPressed()
        {
            _buildingSystem.Clear();
            _selectionSystem.WorkType = WorkType.NONE;
        }

        private void OnToolBarMinePressed()
        {
            _buildingSystem.Clear();
            _selectionSystem.WorkType = WorkType.MINING;
        }

        private void OnToolBarCutPressed()
        {
            _buildingSystem.Clear();
            _selectionSystem.WorkType = WorkType.WOOD_CUTTING;
        }

        private void OnToolBarHaulPressed()
        {
            _buildingSystem.Clear();
            _selectionSystem.WorkType = WorkType.HAULING;
        }

        private void OnToolBarBuildPressed(int tileType)
        {
            _buildingSystem.Clear();
            _buildingSystem.SetCurrent((TileType)tileType);
            _selectionSystem.WorkType = WorkType.BUILDING;
        }

        private void OnToolBarFarmPressed()
        {
            _buildingSystem.Clear();
            _selectionSystem.WorkType = WorkType.FARMING;
        }

        private void OnToolBarGatherPressed()
        {
            _buildingSystem.Clear();
            _selectionSystem.WorkType = WorkType.GATHERING;
        }

        private void OnToolBarFishPressed()
        {
            _buildingSystem.Clear();
            _selectionSystem.WorkType = WorkType.FISHING;
        }

        private void OnToolBarCreateRegionSelected(int regionType)
        {
            _buildingSystem.Clear();
            _selectionSystem.WorkType = WorkType.CREATE_REGION;
            _regionSystem.CurrentRegionType = (RegionType)(regionType);
        }

        private void OnToolBarCancelPressed()
        {
            _buildingSystem.Clear();
            _selectionSystem.WorkType = WorkType.CANCEL;
        }

        private void OnSelectionCreated(Selection selection)
        {
            switch (selection.WorkType)
            {
                case WorkType.MINING:
                case WorkType.WOOD_CUTTING:       
                case WorkType.BUILDING:
                case WorkType.FARMING:
                case WorkType.GATHERING:
                case WorkType.FISHING:
                    CreateWork(selection);
                    break;
                case WorkType.HAULING:
                    CreateHaulingWork(selection);
                    break;
                case WorkType.CREATE_REGION:
                    CreateRegion(selection);
                    break;
                case WorkType.CANCEL:
                    RemoveWork(selection.Points);
                    break;
                default:
                    GD.Print("Incorrect SelectionState in OnSelectionCreated");
                    break;
            }
        }

        private void CreateRegion(Selection selection)
        {
            _regionSystem.CreateRegion(selection);
        }

        private void CreateHaulingWork(Selection selection)
        {
            var allStorage = _world.AllStorage();
            if (allStorage.Count > 0)
            {
                foreach (var point in selection.Points)
                {
                    var items = _itemSystem.GetItems(point);
                    for (int i = 0; i < items.Count; i++)
                    {
                        var closestStoragePos = _pathingSystem.ShortestPointWithAdjacent(point, allStorage);
                        if (closestStoragePos != null)
                        {
                            _workSystem.CreateWork(WorkType.HAULING, closestStoragePos.Value);
                            _workSystem.CreateWork(WorkType.GET_ITEM, point);
                        }
                        else
                        {
                            _selectionSystem.Deselect(point);
                        }
                    }
                }
            }
            else
            {
                _selectionSystem.Deselect(selection);
            }

            //PlanWork();
        }

        private void CreateWork(Selection selection)
        {
            foreach (var point in selection.Points)
            {
                _workSystem.CreateWork(selection.WorkType, point);
            }

            //PlanWork();
        }

        private void PlanWork()
        {
            foreach (var cat in _cats)
            {
                if (cat.CanWork() && !cat.IsMoving())
                {
                    cat.Plan();
                }
            }
        }

        private void OnTileSelected(Vector2 localPos)
        {
            if (_world.IsSolid(localPos) && _world.GetWorldCellId(localPos) != -1)
            {
                var items = _itemSystem.GetStoredItems(_world.GetWorldCellId(localPos));
                
                if (items.Count > 0)
                {
                    _inventoryControl.ClearItems();

                    foreach (var item in items)
                    {
                        _inventoryControl.Add(item);
                    }

                    _inventoryControl.GlobalPosition = _camera.GetCanvasTransform() * localPos;
                    _inventoryControl.Visible = true;
                }
            } 
            else
            {
                var worldInfoData = new WorldInfoData()
                {
                    Coords = _world.GetCoords(localPos),
                    LocalPos = localPos,
                    WorkType = _workSystem.GetWorkType(localPos),
                    TileType = _world.GetTileType(localPos),
                    RegionType = _regionSystem.GetRegionType(localPos),
                    Items = _itemSystem.GetItems(localPos),
                };
                
                _worldInfoDisplay.SetInfo(worldInfoData);
                _worldInfoDisplay.Visible = true;
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
            PlaceCat(newPos, lastPos);
        }

        private void OnCatPathComplete(Path path)
        {
            _pathingSystem.RemovePath(path.Id);
        }

        private void OnCatWork(Cat cat, Work work)
        {
            if (_workSystem.GetWork(work.WorkId) != null)
            {
                work.Command.Execute(cat);

                _workSystem.RemoveWork(work);

                _workSystem.UpdateWork();
            }
        }
    }
}
