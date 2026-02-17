using Godot;

namespace Quasar.scenes.camera
{
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

        [Export]
        public Vector2 WorldSize { get; set; } = new Vector2(2688.0f, 2688.0f);

        [Export]
        public Vector4 WorldMargins { get; set; } = new Vector4(20.0f, 20.0f, 20.0f, 20.0f);

        private Vector2 _zoomTarget = Vector2.Zero;

        private Vector2 _dragStartMousePos = Vector2.Zero;

        private Vector2 _dragStartCameraPos = Vector2.Zero;

        private bool _isDragging = false;

        public override void _Ready()
        {
            _zoomTarget = Zoom;
        }

        public override void _Process(double delta)
        {
            CameraZoom(delta);
            CameraPan();
        }

        public void UpdateZoom(Vector2 zoom)
        {
            Zoom = _zoomTarget = zoom;
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
            ClampCameraPos(Position);
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
                ClampCameraPos(_dragStartCameraPos - moveVector * (1 / Zoom.X));
            }
        }

        private void ClampCameraPos(Vector2 cameraPos)
        {
            var cameraSize = GetViewportRect().Size / Zoom;
            var leftCameraPoint = cameraPos - cameraSize / 2.0f;
            var rightCameraPoint = cameraPos + cameraSize / 2.0f;

            if (leftCameraPoint.X < -WorldMargins.X)
            {
                cameraPos.X = -WorldMargins.X + cameraSize.X / 2.0f;
            }
            else if (rightCameraPoint.X > WorldSize.X + WorldMargins.Z)
            {
                cameraPos.X = WorldSize.X + WorldMargins.X - cameraSize.X / 2.0f;
            }

            if (leftCameraPoint.Y < -WorldMargins.Y)
            {
                cameraPos.Y = -WorldMargins.Y + cameraSize.Y / 2.0f;
            }
            else if (rightCameraPoint.Y > WorldSize.Y + WorldMargins.W)
            {
                cameraPos.Y = WorldSize.Y + WorldMargins.W - cameraSize.Y / 2.0f;
            }

            Position = cameraPos;
        }
    }
}
