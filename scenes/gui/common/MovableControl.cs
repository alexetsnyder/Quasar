using Godot;

namespace Catcophony.scenes.gui.common
{
    [GlobalClass]
    public partial class MovableControl : Control
    {
        private bool _isMoving = false;

        private Vector2 _prevMousePos;

        public override void _Ready()
        {
            this.GuiInput += OnGUIInput;
            MouseFilter = MouseFilterEnum.Stop;
        }

        public override void _Process(double delta)
        {
            if (!Input.IsMouseButtonPressed(MouseButton.Left) && _isMoving)
            {
                _isMoving = false;
            }
        }

        protected virtual void OnGUIInput(InputEvent @event)
        {
            if (@event is InputEventMouseButton inputEventMouseButton)
            {
                if (inputEventMouseButton.ButtonIndex == MouseButton.Left)
                {
                    if (@event.IsPressed())
                    {
                        _isMoving = true;
                        _prevMousePos = inputEventMouseButton.Position;
                    }
                    else
                    {
                        _isMoving = false;
                    }
                }
            }
            else if (@event is InputEventMouseMotion mouseEventMouseMotion)
            {
                if (_isMoving)
                {
                    var dv = mouseEventMouseMotion.Position - _prevMousePos;
                    Position += dv;
                }
            }
        }
    }
}
