using Godot;
using Quasar.scenes.camera;
using Quasar.scenes.map;
using Quasar.scenes.world;

public partial class Main : Node2D
{
	private Map _map;

	private World _world;

	private MapCamera2d _camera;

	private Label _tileTypeLabel;

	private Label _tileColorLabel;

	private Vector2 _prevCameraZoom;

	private Vector2 _prevCameraPos;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_map = GetNode<Map>("Map");
		_world = GetNode<World>("World");
		_camera = GetNode<MapCamera2d>("MapCamera2D");
		_tileTypeLabel = GetNode<Label>("GUI/TileTypeMargin/TileType");
		_tileColorLabel = GetNode<Label>("GUI/TileColorMargin/TileColor");

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
    }

	public void SetTyleTypeLabel()
	{
        var mousePos = GetLocalMousePosition();

        string tyleType;
        if (_map.Visible)
        {
            tyleType = _map.GetTileType(mousePos);
        }
        else
        {
            tyleType = _world.GetTileType(mousePos);
        }

        _tileTypeLabel.Text = tyleType;
    }

    public void SetTyleColorLabel()
    {
        var mousePos = GetLocalMousePosition();

        string tyleColor;
        if (_map.Visible)
        {
            tyleColor = _map.GetTileColor(mousePos);
        }
        else
        {
            tyleColor = _world.GetTileColor(mousePos);
        }

        _tileColorLabel.Text = tyleColor;
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
