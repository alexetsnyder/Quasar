using Godot;

public partial class MapCamera2d : Camera2D
{
	[Export]
	public float ZoomFactor { get; set; } = 1.25f;

	[Export]
	public float ZoomSpeed { get; set; } = 10.0f;

	[Export(PropertyHint.Range, "1.0, 2.0")]
	public float ZoomInLimit { get; set; } = 2.0f;

    [Export(PropertyHint.Range, "0.0, 1.0")]
    public float ZoomOutLimit { get; set; } = 0.65f;

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

	private void CameraZoom(double delta)
	{
		if (Input.IsActionJustPressed("ZoomIn"))
		{
			var tempTarget = _zoomTarget * ZoomFactor;
			_zoomTarget.X = Mathf.Clamp(tempTarget.X, ZoomOutLimit, ZoomInLimit);
            _zoomTarget.Y = Mathf.Clamp(tempTarget.Y, ZoomOutLimit, ZoomInLimit);
        }

        if (Input.IsActionJustPressed("ZoomOut"))
        {
			var tempTarget = _zoomTarget * 1 / ZoomFactor;
            _zoomTarget.X = Mathf.Clamp(tempTarget.X, ZoomOutLimit, ZoomInLimit);
            _zoomTarget.Y = Mathf.Clamp(tempTarget.Y, ZoomOutLimit, ZoomInLimit);
        }

        Zoom = Zoom.Slerp(_zoomTarget, (float)(ZoomSpeed * delta));
    }

	private void CameraPan()
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
