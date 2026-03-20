using Godot;
using Quasar.data;
using Quasar.data.enums;
using Quasar.scenes.common.interfaces;
using Quasar.system;

namespace Quasar.scenes.systems.selection
{
    public partial class SelectionSystem : Node2D, ISelectionSystem
    {
        [Export]
        public WorkType WorkType { get; set; } = WorkType.NONE;

        [Export]
        public Color SelectionColor { get; set; } = new Color(1.0f, 0.0f, 0.0f, 1.0f);

        [Export]
        public Node WorldNode { get; set; }

        [Signal]
        public delegate void TileSelectedEventHandler(Vector2 localPos);

        [Signal]
        public delegate void SelectionCreatedEventHandler(Selection selection);

        private IWorld _world;

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

            GlobalSystem.Instance.LoadInterface<IWorld>(WorldNode, out _world);
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
            if (@event is InputEventMouseButton inputEventMouseButton)
            {
                if (inputEventMouseButton.ButtonIndex == MouseButton.Left)
                {
                    if (@event.IsPressed())
                    {
                        if (WorkType == WorkType.NONE)
                        {
                            EmitSignal(SignalName.TileSelected, GetGlobalMousePosition());
                        }
                        else if (WorkType != WorkType.NONE)
                        {
                            _isSelecting = true;
                            _selectionStart = GetGlobalMousePosition();
                            _selectionRect.Position = _selectionStart;
                            _selectionRect.Size = new Vector2();
                        } 
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

        public void Deselect(Selection selection)
        {
            foreach (var point in selection.Points)
            {
                Deselect(point);
            }
        }

        public void Deselect(Vector2 localPos)
        {
            var coords = _selectedTileMapLayer.LocalToMap(localPos);

            if (_selectedTileMapLayer.GetCellSourceId(coords) != -1)
            {
                SelectCell(_selectedTileMapLayer, coords);
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
            switch(WorkType)
            {
                case WorkType.CANCEL:
                    return AtlasConstants.GetAtlasCoords(TileType.CANCEL);
                case WorkType.MINING:
                case WorkType.BUILDING:
                case WorkType.FARMING:
                case WorkType.FISHING:
                case WorkType.WOOD_CUTTING:
                case WorkType.HAULING:
                case WorkType.GATHERING:
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
                atlasCoord = AtlasConstants.GetAtlasCoords(TileType.SELECTION, (int)SelectionIndex.LEFT_TOP);
            }
            else if (i == startingRow && j == endingCol - 1)
            {
                atlasCoord = AtlasConstants.GetAtlasCoords(TileType.SELECTION, (int)SelectionIndex.RIGHT_TOP);
            }
            else if (i == endingRow - 1 && j == startingCol)
            {
                atlasCoord = AtlasConstants.GetAtlasCoords(TileType.SELECTION, (int)SelectionIndex.LEFT_BOTTOM);
            }
            else if (i == endingRow - 1 && j == endingCol - 1)
            {
                atlasCoord = AtlasConstants.GetAtlasCoords(TileType.SELECTION, (int)SelectionIndex.RIGHT_BOTTOM);
            }
            else if (i == startingRow)
            {
                atlasCoord = AtlasConstants.GetAtlasCoords(TileType.SELECTION, (int)SelectionIndex.TOP);
            }
            else if (i == endingRow - 1)
            {
                atlasCoord = AtlasConstants.GetAtlasCoords(TileType.SELECTION, (int)SelectionIndex.BOTTOM);
            }
            else if (j == startingCol)
            {
                atlasCoord = AtlasConstants.GetAtlasCoords(TileType.SELECTION, (int)SelectionIndex.LEFT);
            }
            else if (j == endingCol - 1)
            {
                atlasCoord = AtlasConstants.GetAtlasCoords(TileType.SELECTION, (int)SelectionIndex.RIGHT);
            }
            else
            {
                atlasCoord = AtlasConstants.GetAtlasCoords(TileType.SELECTION, (int)SelectionIndex.MIDDLE);
            }

            return atlasCoord;
        }

        private Color? GetColorForSelecting()
        {
            switch (WorkType)
            {
                case WorkType.CANCEL:
                    return AtlasConstants.GetColor(TileType.CANCEL);
                case WorkType.MINING:
                case WorkType.BUILDING:
                case WorkType.FARMING:
                case WorkType.FISHING:
                case WorkType.WOOD_CUTTING:
                case WorkType.HAULING:
                case WorkType.GATHERING:
                    return SelectionColor;
                default:
                    GD.Print("Incorrect SelectionState in GetColorForSelecting.");
                    return null;
            }
        }

        private void SetSelectedArea()
        {
            _selectingTileMapLayer.Clear();

            System.Func<Vector2I, bool> filter;

            switch (WorkType)
            {
                case WorkType.MINING:
                    filter = (c) => _world.IsMineable(c) && _selectedTileMapLayer.GetCellSourceId(c) == -1;
                    break;
                case WorkType.WOOD_CUTTING:
                    filter = (c) => _world.IsTree(c) && _selectedTileMapLayer.GetCellSourceId(c) == -1;
                    break;
                case WorkType.HAULING:
                    filter = (c) => _world.HasItemsToHaul(c) && _selectedTileMapLayer.GetCellSourceId(c) == -1;
                    break;
                case WorkType.BUILDING:
                case WorkType.FARMING:
                    filter = (c) => !_world.IsImpassable(c) && _selectedTileMapLayer.GetCellSourceId(c) == -1;
                    break;
                case WorkType.GATHERING:
                    filter = (c) => _world.IsGatherable(c) && _selectedTileMapLayer.GetCellSourceId(c) == -1;
                    break;
                case WorkType.FISHING:
                    filter = (c) => _world.IsWater(c) && _selectedTileMapLayer.GetCellSourceId(c) == -1;
                    break;
                case WorkType.CANCEL:
                    filter = (c) => _selectedTileMapLayer.GetCellSourceId(c) != -1;
                    break;
                default:
                    GD.Print("Incorrect SelectionState in SetSelectedArea");
                    filter = (c) => true;
                    break;
            }

            EmitSignal(SignalName.SelectionCreated, GetSelection(filter));
        }

        private Selection GetSelection(System.Func<Vector2I, bool> filter)
        {
            var mapSelection = GetMapSelection(_selectingTileMapLayer);

            var atlasCoords = GetAtlasCoordsForSelected(WorkType);
            var color = GetCellColorForSelected(WorkType);

            Selection selection = new(WorkType, []);

            for (int i = mapSelection.Position.X; i < mapSelection.End.X; i++)
            {
                for (int j = mapSelection.Position.Y; j < mapSelection.End.Y; j++)
                {
                    var coords = new Vector2I(j, i);
                    if (filter(coords))
                    {
                        SelectCell(_selectedTileMapLayer, coords, atlasCoords, color);
                        selection.Points.Add(_selectedTileMapLayer.MapToLocal(coords));
                    }
                }
            }

            return selection;
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
            if (_world.IsInBounds(coords))
            {
                tileMapLayer.SetCell(coords, atlasCoords, color);
            } 
        }

        private static Vector2I? GetAtlasCoordsForSelected(WorkType workType)
        {
            switch (workType)
            {
                case WorkType.MINING:
                    return AtlasConstants.GetAtlasCoords(TileType.MINE);
                case WorkType.WOOD_CUTTING:
                    return AtlasConstants.GetAtlasCoords(TileType.CUT);
                case WorkType.HAULING:
                    return AtlasConstants.GetAtlasCoords(TileType.HAUL);
                case WorkType.BUILDING:
                    return AtlasConstants.GetAtlasCoords(TileType.BUILD);
                case WorkType.FARMING:
                    return AtlasConstants.GetAtlasCoords(TileType.TILL);
                case WorkType.GATHERING:
                    return AtlasConstants.GetAtlasCoords(TileType.GATHER);
                case WorkType.FISHING:
                    return AtlasConstants.GetAtlasCoords(TileType.FISH);
                case WorkType.CANCEL:
                    return null;
                default:
                    GD.Print("Incorrect WorkType in GetAtlasCoords(workType).");
                    return null;

            }
        }

        private static Color? GetCellColorForSelected(WorkType workType)
        {
            switch (workType)
            {
                case WorkType.MINING:
                    return AtlasConstants.GetColor(TileType.MINE);
                case WorkType.WOOD_CUTTING:
                    return AtlasConstants.GetColor(TileType.CUT);
                case WorkType.HAULING:
                    return AtlasConstants.GetColor(TileType.HAUL);
                case WorkType.BUILDING:
                    return AtlasConstants.GetColor(TileType.BUILD);
                case WorkType.FARMING:
                    return AtlasConstants.GetColor(TileType.TILL);
                case WorkType.GATHERING:
                    return AtlasConstants.GetColor(TileType.GATHER);
                case WorkType.FISHING:
                    return AtlasConstants.GetColor(TileType.FISH);
                case WorkType.CANCEL:
                    return null;
                default:
                    GD.Print("Incorrect WorkType in GetCellColor(workType).");
                    return null;
            }
        }
    }
}
