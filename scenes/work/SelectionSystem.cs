using Godot;
using Quasar.data;
using Quasar.data.enums;
using Quasar.scenes.common.interfaces;

namespace Quasar.scenes.work
{
    public partial class SelectionSystem : Node2D
    {
        [Export]
        public SelectionState SelectionState { get; set; } = SelectionState.NONE;

        [Export]
        public Color SelectionColor { get; set; } = new Color(1.0f, 0.0f, 0.0f, 1.0f);


        private ColorRect _selectionRect;

        private IMultiColorTileMapLayer _selectedTileMapLayer;

        private IMultiColorTileMapLayer _selectingTileMapLayer;

        private bool _isSelecting = false;

        private Vector2 _selectionStart;

        public override void _Ready()
        {
            _selectionRect = GetNode<ColorRect>("SelectionRect");
            _selectedTileMapLayer = GetNode<IMultiColorTileMapLayer>("SelectedTileMapLayer");
            _selectingTileMapLayer = GetNode<IMultiColorTileMapLayer>("SelectingTileMapLayer");
        }

        public override void _Process(double delta)
        {
            if (_isSelecting)
            {
                var currentMousePos = GetGlobalMousePosition();
                var newSelectionRect = new Rect2(_selectionStart, currentMousePos - _selectionStart).Abs();
                _selectionRect.Position = newSelectionRect.Position;
                _selectionRect.Size = newSelectionRect.Size;
                SetSelectingArea();
            }
        }

        public override void _UnhandledInput(InputEvent @event)
        {
            if (@event.IsActionPressed("Quit"))
            {
                GetTree().Quit();
            }
            else if (@event is InputEventMouseButton inputEventMouseButton)
            {
                if (inputEventMouseButton.ButtonIndex == MouseButton.Left)
                {
                    if (@event.IsPressed())
                    {
                        _isSelecting = true;
                        _selectionStart = GetGlobalMousePosition();
                        _selectionRect.Position = _selectionStart;
                        _selectionRect.Size = new Vector2();
                    }
                    else
                    {
                        if (_isSelecting)
                        {
                            _isSelecting = false;
                            _selectionRect.Visible = false;
                            SetSelectedArea();
                        }
                    }
                }
            }
        }

        private void SetSelectingArea()
        {
            _selectingTileMapLayer.Clear();

            var selection = GetMapSelection(_selectingTileMapLayer);

            for (int i = selection.Position.X; i < selection.End.X; i++)
            {
                for (int j = selection.Position.Y; j < selection.End.Y; j++)
                {
                    var coords = new Vector2I(j, i);
                    var atlasCoords = GetAtlasCoordsForSelecting(i, j, selection);
                    var color = GetColorForSelecting();

                    SelectCell(_selectingTileMapLayer, coords, atlasCoords, color);
                }
            }

        }

        private Vector2I? GetAtlasCoordsForSelecting(int i, int j, Rect2I selection)
        {
            switch(SelectionState)
            {
                case SelectionState.CANCEL:
                    return AtlasCoordSelection.CANCEL;
                case SelectionState.MINING:
                case SelectionState.BUILDING:
                case SelectionState.FARMING:
                case SelectionState.FISHING:
                    return GetAtlasCoordForSelection(i, j, selection);
                default:
                    GD.Print("Incorrect SelectionState in GetAtlasCoordsForSelecting.");
                    return null;
            }
        }

        private static Vector2I GetAtlasCoordForSelection(int i, int j, Rect2I selection)
        {
            Vector2I atlasCoord;

            var startingRow = selection.Position.X;
            var startingCol = selection.Position.Y;
            var endingRow = selection.End.X;
            var endingCol = selection.End.Y;
            
            if (i == startingRow && j == startingCol)
            {
                atlasCoord = AtlasCoordSelection.LEFT_TOP_SELECTION;
            }
            else if (i == startingRow && j == endingCol - 1)
            {
                atlasCoord = AtlasCoordSelection.RIGHT_TOP_SELECTION;
            }
            else if (i == endingRow - 1 && j == startingCol)
            {
                atlasCoord = AtlasCoordSelection.LEFT_BOTTOM_SELECTION;
            }
            else if (i == endingRow - 1 && j == endingCol - 1)
            {
                atlasCoord = AtlasCoordSelection.RIGHT_BOTTOM_SELECTION;
            }
            else if (i == startingRow)
            {
                atlasCoord = AtlasCoordSelection.TOP_SELECTION;
            }
            else if (i == endingRow - 1)
            {
                atlasCoord = AtlasCoordSelection.BOTTOM_SELECTION;
            }
            else if (j == startingCol)
            {
                atlasCoord = AtlasCoordSelection.LEFT_SELECTION;
            }
            else if (j == endingCol - 1)
            {
                atlasCoord = AtlasCoordSelection.RIGHT_SELECTION;
            }
            else
            {
                atlasCoord = AtlasCoordSelection.MIDDLE_SELECTION;
            }

            return atlasCoord;
        }

        private Color? GetColorForSelecting()
        {
            switch (SelectionState)
            {
                case SelectionState.CANCEL:
                    return ColorConstants.WARNING_RED;
                case SelectionState.MINING:
                case SelectionState.BUILDING:
                case SelectionState.FARMING:
                case SelectionState.FISHING:
                    return SelectionColor;
                default:
                    GD.Print("Incorrect SelectionState in GetColorForSelecting.");
                    return null;
            }
        }

        private void SetSelectedArea()
        {
            _selectingTileMapLayer.Clear();
        }

        private Rect2I GetMapSelection(IMultiColorTileMapLayer tileMapLayer)
        {
            var tileSize = tileMapLayer.TileSize;
            var left = _selectionRect.Position.X;
            var right = left + _selectionRect.Size.X;
            var top = _selectionRect.Position.Y;
            var bottom = top + _selectionRect.Size.Y;

            var startingCol = Mathf.FloorToInt(left / tileSize.X);
            var endingCol = Mathf.CeilToInt(right / tileSize.X);
            var startingRow = Mathf.FloorToInt(top / tileSize.Y);
            var endingRow = Mathf.CeilToInt(bottom / tileSize.Y);

            return new(startingRow, startingCol, new(endingRow - startingRow, endingCol - startingCol));
        }

        private void SelectCell(IMultiColorTileMapLayer tileMapLayer, Vector2I coords, Vector2I? atlasCoords = null, Color? color = null)
        {
            //Check if inbounds
            tileMapLayer.SetCell(coords, atlasCoords, color);
        }
    }
}
