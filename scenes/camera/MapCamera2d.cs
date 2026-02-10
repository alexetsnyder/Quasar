using Godot;

public partial class MapCamera2d : Camera2D
{
	[Export]
	public float ZoomFactor { get; set; } = 1.25f;

	[Export]
	public float ZoomSpeed { get; set; } = 10.0f;

	private Vector2 _zoomTarget = Vector2.Zero;

	private Vector2 _dragStartMousePos = Vector2.Zero;

	private Vector2 _dragStartCameraPos = Vector2.Zero;

	private bool _isDragging = false;
	

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		_zoomTarget = Zoom;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		CameraZoom(delta);
		CameraPan();
	}

	public void CameraZoom(double delta)
	{
		if (Input.IsActionJustPressed("ZoomIn"))
		{
			_zoomTarget *= ZoomFactor;
		}

        if (Input.IsActionJustPressed("ZoomOut"))
        {
			_zoomTarget *= 1 / ZoomFactor;
        }

		Zoom = Zoom.Slerp(_zoomTarget, (float)(ZoomSpeed * delta));
    }

	public void CameraPan()
	{
		if (!_isDragging && Input.IsActionJustPressed("CameraPan"))
		{
			_dragStartMousePos = GetViewport().GetMousePosition();
			_dragStartCameraPos = Position;
			_isDragging = true;
		}

		if (_isDragging && Input.IsActionJustReleased("CameraPan"))
		{
			_isDragging = false;
		}

		if (_isDragging)
		{
			var currentMousePos = GetViewport().GetMousePosition();
			var moveVector = currentMousePos - _dragStartMousePos;
			Position = _dragStartCameraPos - moveVector * (1 / Zoom.X);
		}
	}
}
