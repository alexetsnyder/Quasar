using Godot;
using Quasar.scenes.camera;
using Quasar.scenes.map;
using Quasar.scenes.world;

public partial class Main : Node2D
{
	private Map _map;

	private World _world;

	private MapCamera2d _camera;

	private CanvasLayer _debugGUI;

	private Label _tileTypeLabel;

	private Label _tileColorLabel;

	private Vector2 _prevCameraZoom;

	private Vector2 _prevCameraPos;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_debugGUI = GetNode<CanvasLayer>("DebugGUI");
		_map = GetNode<Map>("Map");
		_world = GetNode<World>("World");
		_camera = GetNode<MapCamera2d>("MapCamera2D");
		_tileTypeLabel = GetNode<Label>("DebugGUI/TileTypeMargin/TileType");
		_tileColorLabel = GetNode<Label>("DebugGUI/TileColorMargin/TileColor");

		_camera.Position = new Vector2(_camera.WorldSize.X / 2.0f, _camera.WorldSize.Y / 2.0f);

		_prevCameraZoom = _camera.Zoom;
		_prevCameraPos = _camera.Position;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
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
		else if (@event.IsActionPressed("FindCat") && _world.Visible)
		{
			_camera.Position = _world.CatPosition;
		}
    }

	public void SetTyleTypeLabel()
	{
        var mousePos = GetLocalMousePosition();

        string tileType;
		Color tileColor;
        if (_map.Visible)
        {
            tileType = _map.GetTileTypeStr(mousePos);
			tileColor = _map.GetTileColor(mousePos);
        }
        else
        {
            tileType = _world.GetTileTypeStr(mousePos);
			tileColor = _world.GetTileColor(mousePos);
        }

        _tileTypeLabel.Text = tileType;
		_tileTypeLabel.AddThemeColorOverride("font_color", tileColor);
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

        _tileColorLabel.Text = tileColorStr;
		_tileColorLabel.AddThemeColorOverride("font_color", tileColor);
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
			_world.Visible = false;
		}
		else
		{
            _map.Visible = false;
            _world.Visible = true;
        }

		SetTyleTypeLabel();
		SetTyleColorLabel();
	}
}
