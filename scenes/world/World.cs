using Godot;
using Quasar.data;
using Quasar.math;

namespace Quasar.scenes.world
{
    public partial class World : Node2D
    {
        [Export(PropertyHint.Range, "0,200")]
        public int Rows { get; set; } = 10;

        [Export(PropertyHint.Range, "0,200")]
        public int Cols { get; set; } = 10;

        [Signal]
        public delegate void TileSelectedEventHandler();

        private TileMapLayer _mapLayer;

        private TileMapLayer _selectLayer;

        private ColorRect _selectionRect;

        private bool _isSelecting = false;

        private Vector2 _selectionStart;

        private SimplexNoise _noise;

        private AStarGrid2D _aStarGrid2d = new();

        private Vector2I _selectedCell;

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            _mapLayer = GetNode<TileMapLayer>("MapLayer");
            _selectLayer = GetNode<TileMapLayer>("SelectLayer");
            _selectionRect = GetNode<ColorRect>("SelectionRect");
            _noise = new SimplexNoise();

            FillMap();

            _aStarGrid2d.Region = new Rect2I(0, 0, Rows + 1, Cols + 1);
            _aStarGrid2d.CellSize = new Vector2(16.0f, 16.0f);
            _aStarGrid2d.DefaultComputeHeuristic = AStarGrid2D.Heuristic.Manhattan;
            _aStarGrid2d.DefaultEstimateHeuristic = AStarGrid2D.Heuristic.Manhattan;
            _aStarGrid2d.DiagonalMode = AStarGrid2D.DiagonalModeEnum.Never;
            _aStarGrid2d.Update();
        }

        // Called every frame. 'delta' is the elapsed time since the previous frame.
        public override void _Process(double delta)
        {
            if (_isSelecting)
            {
                var currentMousePos = GetGlobalMousePosition();
                var newSelectionRect = new Rect2(_selectionStart, currentMousePos - _selectionStart).Abs();
                _selectionRect.Position = newSelectionRect.Position;
                _selectionRect.Size = newSelectionRect.Size;
            }
        }

        public override void _UnhandledInput(InputEvent @event)
        {
            if (@event is InputEventMouseButton inputEventMouseButton)
            {
                if (inputEventMouseButton.ButtonIndex == MouseButton.Right)
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
                            SelectArea();
                        }
                    }
                }
                else if (inputEventMouseButton.ButtonIndex == MouseButton.Left)
                {
                    if (@event.IsPressed())
                    {
                        SelectCell();

                        EmitSignal(SignalName.TileSelected);
                    }
                }
            }
            else if (@event is InputEventMouseMotion && _isSelecting)
            {
                if (_selectionRect.Size.X >= 1.0f && _selectionRect.Size.Y >= 1.0f)
                {
                    _selectionRect.Visible = true;
                }
                else
                {
                    _selectionRect.Visible = false;
                }
            }
        }

        private void FillMap()
        {
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Cols; j++)
                {
                    var coord = new Vector2I(i, j);

                    var color = ColorConstants.GREEN;
                    var atlasCoords = AtlasTileCoords.GRASS;
                    var noiseVal = _noise.GetNoise(i, j) * Math.SigmoidFallOffMapCircular(j, i, Cols, Rows);
                    GetAtlasCoordsAndColor(noiseVal, ref atlasCoords, ref color);

                    _mapLayer.SetCell(coord, 0, atlasCoords);

                    var tileData = _mapLayer.GetCellTileData(coord);
                    if (tileData != null)
                    {
                        tileData.Modulate = color; 
                    }
                }
            }
        }

        private void SelectArea()
        {
            _selectLayer.Clear();

            var tileSize = _mapLayer.TileSet.TileSize;
            var left = _selectionRect.Position.X;
            var right = left + _selectionRect.Size.X;
            var top = _selectionRect.Position.Y;
            var bottom = top + _selectionRect.Size.Y;

            var startingCol = Mathf.FloorToInt(left / tileSize.X);
            var endingCol = Mathf.CeilToInt(right / tileSize.X);
            var startingRow = Mathf.FloorToInt(top / tileSize.Y);
            var endingRow = Mathf.CeilToInt(bottom / tileSize.Y);

            for (int i = startingRow; i < endingRow; i++)
            {
                for (int j = startingCol; j < endingCol; j++)
                {
                    var cellCoord = new Vector2I(j, i);

                    if (_mapLayer.GetCellSourceId(cellCoord) != -1)
                    {
                        var atlasCoord = _mapLayer.GetCellAtlasCoords(cellCoord);

                        _selectLayer.SetCell(cellCoord, 0, atlasCoord);

                        var tileData = _selectLayer.GetCellTileData(cellCoord);

                        if (tileData != null)
                        {
                            tileData.Modulate = new Color(1.0f, 0.0f, 0.0f, 1.0f);
                        }
                    }
                }
            }
        }

        private void SelectCell()
        {
            _selectLayer.Clear();

            var tileSize = _mapLayer.TileSet.TileSize;
            var mousePos = GetLocalMousePosition();
            var col = Mathf.FloorToInt(mousePos.X / tileSize.X);
            var row = Mathf.FloorToInt(mousePos.Y / tileSize.Y);
            var cellCoord = new Vector2I(col, row);

            if (_mapLayer.GetCellSourceId(cellCoord) != -1)
            {
                _selectedCell = new(cellCoord.X, cellCoord.Y);

                var atlasCoord = _mapLayer.GetCellAtlasCoords(cellCoord);

                _selectLayer.SetCell(cellCoord, 0, atlasCoord);

                var tileData = _selectLayer.GetCellTileData(cellCoord);

                if (tileData != null)
                {
                    tileData.Modulate = new Color(1.0f, 0.0f, 0.0f, 1.0f);
                }
            }
        }

        public void FindPath(Vector2 startPos)
        {
            var start = _mapLayer.LocalToMap(startPos);
            var end = _selectedCell;

            var path = _aStarGrid2d.GetPointPath(start, end);

            foreach (var point in path)
            {
                var cellCoord =  _mapLayer.LocalToMap(point);

                var atlasCoord = _mapLayer.GetCellAtlasCoords(cellCoord);

                _selectLayer.SetCell(cellCoord, 0, atlasCoord);

                var tileData = _selectLayer.GetCellTileData(cellCoord);

                if (tileData != null)
                {
                    tileData.Modulate = new Color(1.0f, 0.0f, 1.0f, 1.0f);
                }
            }
        }

        private void GetAtlasCoordsAndColor(float noiseVal, ref Vector2I atlasCoord, ref Color cellColor)
        {
            if (noiseVal < 25.0f)
            {
                atlasCoord = AtlasTileCoords.WATER;
                cellColor = ColorConstants.BLUE;
            }
            else if (noiseVal < 35.0f)
            {
                atlasCoord = AtlasTileCoords.GRASS;
                cellColor = ColorConstants.GRASS_GREEN;
            }
            else if (noiseVal < 40.0f)
            {
                atlasCoord = AtlasTileCoords.TREE;
                cellColor = ColorConstants.FOREST_GREEN; 
            }
            else if (noiseVal < 60.0f)
            {
                atlasCoord = AtlasTileCoords.HILL;
                cellColor = ColorConstants.EMERALD_GREEN;
            }
            else //(noiseVal < 100.0f
            {
                atlasCoord = AtlasTileCoords.MOUNTAIN;
                cellColor = ColorConstants.GREY;
            }
        }
    }
}
