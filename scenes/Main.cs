using Godot;
using Quasar.scenes.world;

public partial class Main : Node2D
{
	private World _world;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_world = GetNode<World>("World");
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
