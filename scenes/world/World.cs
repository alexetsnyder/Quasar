using Godot;
using Quasar.data;
using Quasar.math;
using Quasar.scenes.cats;
using System.Collections.Generic;

namespace Quasar.scenes.world
{
    public partial class World : Node2D
    {
        #region Variables

        [Export(PropertyHint.Range, "0,200")]
        public int Rows { get; set; } = 10;

        [Export(PropertyHint.Range, "0,200")]
        public int Cols { get; set; } = 10;

        [Export]
        public Color SelectionColor { get; set; } = new Color(1.0f, 0.0f, 0.0f, 1.0f);

        [Export]
        public Color PathColor { get; set; } = new Color(1.0f, 0.0f, 1.0f, 1.0f);

        private TileMapLayer _worldLayer;

        private TileMapLayer _selectLayer;

        private ColorRect _selectionRect;

        private Cat _cat;

        private bool _isSelecting = false;

        private Vector2 _selectionStart;

        private SimplexNoise _heightNoise;

        private AStarGrid2D _aStarGrid2d = new();

        private RandomNumberGenerator _rng = new();

        private Vector2I[,] _worldArray;

        private readonly List<Vector2I> _groundVariance = [AtlasTileCoords.DIRT, AtlasTileCoords.GRASS_01, 
                                                           AtlasTileCoords.GRASS_02, AtlasTileCoords.GRASS_03];

        private readonly List<Color> _colorVariance = [ColorConstants.RED, ColorConstants.GREEN, ColorConstants.GRASS_GREEN, 
                                                       ColorConstants.YELLOW, ColorConstants.ORANGE, ColorConstants.AMBER];

        #endregion

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            _rng.Randomize();

            _worldLayer = GetNode<TileMapLayer>("WorldLayer");
            _selectLayer = GetNode<TileMapLayer>("SelectLayer");
            _selectionRect = GetNode<ColorRect>("SelectionRect");
            _cat = GetNode<Cat>("Cat");
            _heightNoise = new SimplexNoise(_rng.RandiRange(int.MinValue, int.MaxValue));

            _worldArray = new Vector2I[Rows, Cols];

            GenerateWorld();
            FillMap();
            SetUpAStar();
            PlaceCat();
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
            if (Visible)
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
                            FindPath(_cat.Position, GetLocalMousePosition());
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
        }

        public string GetTileTypeStr(Vector2 localPos)
        {
            var cellCoord = _worldLayer.LocalToMap(localPos);
            var atlasCoord = _worldLayer.GetCellAtlasCoords(cellCoord);

            return AtlasTileCoords.GetTileStrReflection(atlasCoord);
        }

        public string GetTileColorStr(Vector2 localPos)
        {
            var color = GetTileColor(localPos);

            if (color == ColorConstants.WHITE)
            {
                return "NONE";
            }

            return ColorConstants.GetColorStrReflection(color);
        }

        public Color GetTileColor(Vector2 localPos)
        {
            var cellCoord = _worldLayer.LocalToMap(localPos);

            if (_worldLayer.GetCellSourceId(cellCoord) != -1)
            {
                var tileData = _worldLayer.GetCellTileData(cellCoord);
                return tileData.Modulate;
            }

            return ColorConstants.WHITE;
        }

        private void GenerateWorld()
        {
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Cols; j++)
                {
                    var noiseVal = _heightNoise.GetNoise(j, i);
                    _worldArray[i, j] = GetAtlasCoords(noiseVal);
                }
            }

            CheckForEdges();
        }

        private void CheckForEdges()
        {
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Cols; j++)
                {
                    var cellCoord = new Vector2I(i, j);
                    if (IsEdge(cellCoord))
                    {
                        _worldArray[i, j] = AtlasTileCoords.SOLID_WALL;
                    }
                }
            }
        }

        private void FillMap()
        {
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Cols; j++)
                {
                    var cellCoord = new Vector2I(i, j);
                    var atlasCoord = _worldArray[i, j];
                    var modulate = GetTileColor(atlasCoord, out int colorIndex);

                    SetCell(cellCoord, atlasCoord, modulate, colorIndex);

                    var tileData = _worldLayer.GetCellTileData(cellCoord);
                    if (tileData != null)
                    {
                        tileData.Modulate = modulate;
                    }
                }
            }
        }

        private bool IsEdge(Vector2I cellCoord)
        {
            List<Vector2I> neighbors = [new(1, 0), new(1, 1),
                                        new(1, -1), new(0, 1),
                                        new(-1, 1), new(0, -1),
                                        new(-1, -1), new(-1, 0)];

            if (IsSolid(cellCoord))
            {
                foreach (var neighbor in neighbors)
                {
                    var neighborCellCoord = cellCoord + neighbor;
                    if (!IsSolid(neighborCellCoord))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private Vector2I GetAtlasCoord(Vector2I cellCoord)
        {
            if (cellCoord.X < 0 || cellCoord.Y < 0 ||
                cellCoord.X >= Rows || cellCoord.Y >= Cols)
            {
                return AtlasTileCoords.SOLID;
            }

            return _worldArray[cellCoord.X, cellCoord.Y];
        }

        private bool IsSolid(Vector2I cellCoord)
        {
            var atlasCoord = GetAtlasCoord(cellCoord);
            return (atlasCoord == AtlasTileCoords.SOLID || atlasCoord == AtlasTileCoords.SOLID_WALL);
        }

        private void SetCell(Vector2I cellCoord, Vector2I? atlasCoord = null, Color? modulate = null, int alternateTile = 0)
        {
            _worldLayer.SetCell(cellCoord, 0, atlasCoord, alternateTile);

            if (modulate != null)
            {
                var tileData = _worldLayer.GetCellTileData(cellCoord);
                if (tileData != null)
                {
                    tileData.Modulate = modulate.Value;
                }
            }   
        }

        private void SetUpAStar()
        {
            _aStarGrid2d.Region = new Rect2I(0, 0, Rows + 1, Cols + 1);
            _aStarGrid2d.CellSize = _worldLayer.TileSet.TileSize;
            _aStarGrid2d.DefaultComputeHeuristic = AStarGrid2D.Heuristic.Manhattan;
            _aStarGrid2d.DefaultEstimateHeuristic = AStarGrid2D.Heuristic.Manhattan;
            _aStarGrid2d.DiagonalMode = AStarGrid2D.DiagonalModeEnum.Never;
            _aStarGrid2d.Update();

            foreach (var cellCoord in _worldLayer.GetUsedCellsById())
            {
                if (IsImpassable(cellCoord))
                {
                    _aStarGrid2d.SetPointSolid(cellCoord);
                }
            }
        }

        private bool IsImpassable(Vector2I cellCoord)
        {
            var atlasCoord = _worldLayer.GetCellAtlasCoords(cellCoord);
            return (atlasCoord == AtlasTileCoords.SOLID || 
                    atlasCoord == AtlasTileCoords.SOLID_WALL || 
                    atlasCoord == AtlasTileCoords.WATER);
        }

        private void PlaceCat()
        {
            var tileSize = _worldLayer.TileSet.TileSize;
            _cat.Scale = new(tileSize.X / _cat.Width, tileSize.Y / _cat.Height);

            List<Vector2I> possibleCells = [];

            foreach (var cellCoord in _worldLayer.GetUsedCellsById())
            {
                if (!IsImpassable(cellCoord))
                {
                    possibleCells.Add(cellCoord);
                }
            }

            var selectedCellCoord = RandomChoice(possibleCells, out _);
            var localPos = _worldLayer.MapToLocal(selectedCellCoord);

            _cat.Position = new(localPos.X - 1.0f, localPos.Y - 1.0f);
            _worldLayer.SetCell(selectedCellCoord);
        }

        private void SelectArea()
        {
            _selectLayer.Clear();

            var tileSize = _worldLayer.TileSet.TileSize;
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

                    SelectCell(cellCoord, SelectionColor);
                }
            }
        }

        private void FindPath(Vector2 startPos, Vector2 endPos)
        {
            _selectLayer.Clear();

            var start = _worldLayer.LocalToMap(startPos);
            var end = _worldLayer.LocalToMap(endPos);

            var path = _aStarGrid2d.GetPointPath(start, end);

            foreach (var point in path)
            {
                var cellCoord =  _worldLayer.LocalToMap(point);

                SelectCell(cellCoord, PathColor);
            }
        }

        private void SelectCell(Vector2I cellCoord, Color modulate)
        {
            if (_worldLayer.GetCellSourceId(cellCoord) != -1)
            {
                var atlasCoord = _worldLayer.GetCellAtlasCoords(cellCoord);

                _selectLayer.SetCell(cellCoord, 0, atlasCoord);

                var tileData = _selectLayer.GetCellTileData(cellCoord);

                if (tileData != null)
                {
                    tileData.Modulate = modulate;
                }
            }
        }

        private Vector2I GetAtlasCoords(float heightNoiseVal)
        {
            if (heightNoiseVal < 25.0f)
            {
                return AtlasTileCoords.WATER;
            }
            else if (heightNoiseVal < 65.0f)
            {
                return RandomChoice(_groundVariance, out _);
            }
            else //(heightNoiseVal < 100.0f)
            {
                return AtlasTileCoords.SOLID;
            }
        }

        private Color GetTileColor(Vector2I atlasCoord, out int colorIndex)
        {
            colorIndex = 0;
            if (atlasCoord == AtlasTileCoords.WATER)
            {
                return ColorConstants.BLUE;
            }
            else if (atlasCoord == AtlasTileCoords.DIRT ||
                     atlasCoord == AtlasTileCoords.GRASS_01 ||
                     atlasCoord == AtlasTileCoords.GRASS_02 ||
                     atlasCoord == AtlasTileCoords.GRASS_03)
            {
                return RandomChoice(_colorVariance, out colorIndex);
            }
            else if (atlasCoord == AtlasTileCoords.SOLID_WALL)
            {
                return ColorConstants.WALL_PURPLE;
            }
            else //(atlasCoord == AtlasTileCoords.SOLID)
            {
                return ColorConstants.BLACK;
            }
        }

        private T RandomChoice<T>(List<T> alts, out int index)
        { 
            if (alts.Count <= 0)
            {
                index = 0;
                return default;
            }

            index = _rng.RandiRange(0, alts.Count - 1);
            return alts[index];
        }
    }
}
