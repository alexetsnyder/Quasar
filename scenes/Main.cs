using Godot;
using Quasar.scenes.camera;
using Quasar.scenes.map;
using Quasar.scenes.world;
using Quasar.scenes.gui;
using Quasar.data.enums;
using Quasar.scenes.cats;

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

        public override void _Ready()
        {
            _debugGUI = GetNode<CanvasLayer>("DebugGUI");
            _gui = GetNode<CanvasLayer>("GUI");
            _map = GetNode<Map>("Map");
            _world = GetNode<World>("World");
            _camera = GetNode<MapCamera2d>("MapCamera2D");
            _tileTypeDisplay = GetNode<BasicLabelDisplay>("DebugGUI/TileTypeDisplay");
            _tileColorDisplay = GetNode<BasicLabelDisplay>("DebugGUI/TileColorDisplay");

            var sceneResource = ResourceLoader.Load<PackedScene>("res://scenes/gui/character_display.tscn");
            if (sceneResource != null)
            {
                _characterDisplay = sceneResource.Instantiate<CharacterDisplay>();
                _gui.AddChild(_characterDisplay);
                _characterDisplay.Visible = false;
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
            Color tileColor;
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
            _tileTypeDisplay.SetLabelColor(tileColor);
        }

        public void SetTyleColorLabel()
        {
            var mousePos = GetLocalMousePosition();

            string tileColorStr;
            Color tileColor;
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
            _tileColorDisplay.SetLabelColor(tileColor);
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
            }
            else
            {
                _map.Visible = false;
                _map.SetProcessUnhandledInput(false);
                _world.Visible = true;
                _world.SetProcessUnhandledInput(true);
            }

            SetTyleTypeLabel();
            SetTyleColorLabel();
        }

        private void OnToolBarDigPressed()
        {
            _world.SetSelectionState(SelectionState.DIGGING);
        }

        private void OnToolBarSelectPressed()
        {
            _world.SetSelectionState(SelectionState.SINGLE);
        }

        private void OnToolBarCancelPressed()
        {
            _world.SetSelectionState(SelectionState.CANCEL);
        }

        public void OnCatClickedOn(Cat cat)
        {
            _characterDisplay.FillUI(cat.CatData);
            _characterDisplay.Visible = true;
        }
    }
}
