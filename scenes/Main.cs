using Godot;
using Quasar.scenes.camera;
using Quasar.scenes.world;

public partial class Main : Node2D
{
	private World _world;

	private MapCamera2d _camera;

	private Label _tileLabel;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_world = GetNode<World>("World");
		_camera = GetNode<MapCamera2d>("MapCamera2D");
		_tileLabel = GetNode<Label>("GUI/TileLabelMargin/TileLabel");

		_camera.Position = new Vector2(_camera.WorldSize.X / 2.0f, _camera.WorldSize.Y / 2.0f);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		var mousePos = GetLocalMousePosition();
		_tileLabel.Text = _world.GetTileType(mousePos);
	}

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("Quit"))
		{
			GetTree().Quit();
		}
    }
}
