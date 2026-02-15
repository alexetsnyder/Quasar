using Godot;
using Quasar.scenes.camera;

public partial class Main : Node2D
{
	private MapCamera2d _camera;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_camera = GetNode<MapCamera2d>("MapCamera2D");

		_camera.Position = new Vector2(_camera.WorldSize.X / 2.0f, _camera.WorldSize.Y / 2.0f);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("Quit"))
		{
			GetTree().Quit();
		}
    }
}
