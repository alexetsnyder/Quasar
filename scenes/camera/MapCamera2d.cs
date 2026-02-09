using Godot;

public partial class MapCamera2d : Camera2D
{
	[Export(PropertyHint.Range, "0, 1000")]
	public float PanSpeed { get; set; } = 250.0f;

    [Export(PropertyHint.Range, "0, 1000")]
    public float PanMargin { get; set; } = 25.0f;

    [Export(PropertyHint.Range, "0, 1000")]
    public float DragInertia { get; set; } = 0.1f;

    [Export]
	public bool CanDrag { get; set; } = true;

	private Vector2 _panDir = Vector2.Zero;

    private Vector2 _dragMovement = Vector2.Zero;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    public override void _PhysicsProcess(double delta)
    {
        if (_dragMovement != Vector2.Zero)
        {
            _dragMovement *= (float)Mathf.Pow(DragInertia, delta); 
            
            if (_dragMovement.LengthSquared() < 0.01f)
            {
                SetPhysicsProcess(false);
            }
        }
    }
}
