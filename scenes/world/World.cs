using Godot;
using Quasar.data;
using Quasar.data.enums;
using Quasar.math;
using Quasar.scenes.cats;
using Quasar.scenes.time;
using System.Collections.Generic;
using System.Linq;

namespace Quasar.scenes.world
{
    public partial class World : Node2D
    {
        #region Exports

        [Export(PropertyHint.Range, "0,200")]
        public int Rows { get; set; } = 10;

        [Export(PropertyHint.Range, "0,200")]
        public int Cols { get; set; } = 10;

        [Export]
        public int CatSpeed { get; set; } = 10;

        [Export]
        public bool ShowGrid { get; set; } = false;

        [Export]
        public Color SelectionColor { get; set; } = new Color(1.0f, 0.0f, 0.0f, 1.0f);

        [Export]
        public Color PathColor { get; set; } = new Color(1.0f, 0.0f, 1.0f, 1.0f);

        #endregion

        #region Children

        private TileMapLayer _gridLayer;

        private TileMapLayer _worldLayer;

        private TileMapLayer _hideLayer;

        private TileMapLayer _selectLayer;

        private TileMapLayer _pathLayer;

        private ColorRect _selectionRect;

        #endregion

        #region Private Variables

        private SelectionState _selectionState = SelectionState.NONE;

        private WorldManager _worldManager = new();

        private Queue<Vector2> _path = [];

        private bool _isCatMoving = false;

        private Vector2 _nextCatPos = new();

        private bool _isSelecting = false;

        private Vector2 _selectionStart;

        private SimplexNoise _heightNoise;

        private AStarGrid2D _aStarGrid2d = new();

        private RandomNumberGenerator _rng = new();

        private WorldCell[,] _worldCellArray;

        private readonly List<Vector2I> _groundVariance = [AtlasCoordWorld.DIRT, AtlasCoordWorld.GRASS_01, 
                                                           AtlasCoordWorld.GRASS_02, AtlasCoordWorld.GRASS_03];

        private readonly List<Color> _colorVariance = [ColorConstants.RED, ColorConstants.GREEN, ColorConstants.GRASS_GREEN, 
                                                       ColorConstants.YELLOW, ColorConstants.ORANGE, ColorConstants.AMBER];

        #endregion

        #region Public Methods

        public override void _Ready()
        {
            _rng.Randomize();

            _gridLayer = GetNode<TileMapLayer>("GridLayer");
            _worldLayer = GetNode<TileMapLayer>("WorldLayer");
            _hideLayer = GetNode<TileMapLayer>("HideLayer");
            _selectLayer = GetNode<TileMapLayer>("SelectLayer");
            _pathLayer = GetNode<TileMapLayer>("PathLayer");
            _selectionRect = GetNode<ColorRect>("SelectionRect");
            _worldManager.Register(GetNode<Cat>("Cat"), new(0, 0));
            _heightNoise = new SimplexNoise(_rng.RandiRange(int.MinValue, int.MaxValue));

            _worldCellArray = new WorldCell[Rows, Cols];

            GenerateWorld();
            FillMap();
            SetUpAStar();
            PlaceCat();

            _gridLayer.Visible = ShowGrid;
        }

        public override void _Process(double delta)
        {
            if (_isSelecting)
            {
                var currentMousePos = GetGlobalMousePosition();
                var newSelectionRect = new Rect2(_selectionStart, currentMousePos - _selectionStart).Abs();
                _selectionRect.Position = newSelectionRect.Position;
                _selectionRect.Size = newSelectionRect.Size;
                SelectArea(SelectionState.SELECTING);
            }

            MoveCat(TimeSystem.Instance.TicksPerSecond * delta);
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
                            SelectArea(SelectionState.SELECTING);
                        }
                    }
                }
                else if (inputEventMouseButton.ButtonIndex == MouseButton.Left)
                {
                    if (@event.IsPressed())
                    {
                        var cat = GetCat();
                        if (cat != null)
                        {
                            FindPath(cat.Position, GetLocalMousePosition());
                        } 
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

        public string GetTileTypeStr(Vector2 localPos)
        {
            var cellCoord = _worldLayer.LocalToMap(localPos);
            var atlasCoord = _worldLayer.GetCellAtlasCoords(cellCoord);

            return AtlasCoordWorld.GetTileStrReflection(atlasCoord);
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

        public void SetSelectionState(SelectionState selectionState)
        {
            _selectionState = selectionState;
        }

        #endregion

        #region Private Methods

        private void GenerateWorld()
        {
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Cols; j++)
                {
                    var noiseVal = _heightNoise.GetNoise(j, i);
                    var atlasCoord = GetAtlasCoord(noiseVal);
                    var modulate = GetTileColor(atlasCoord, out int colorIndex);
                    _worldCellArray[i, j] = new(atlasCoord, modulate, colorIndex);
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
                        var atlasCoord = AtlasCoordWorld.SOLID_WALL;
                        var modulate = GetTileColor(atlasCoord, out int colorIndex);
                        _worldCellArray[i, j] = new(atlasCoord, modulate, colorIndex);
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

        private void FillMap()
        {
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Cols; j++)
                {
                    var cellCoord = new Vector2I(i, j);
                    var worldCell = _worldCellArray[i, j];

                    SetCell(_worldLayer, cellCoord, 0, worldCell.AtlasCoord, worldCell.AlternateTile, worldCell.Modulate);
                    SetCell(_gridLayer, cellCoord, 1, new(0, 0), 0, ColorConstants.GREY);
                }
            }
        }

        private Vector2I GetAtlasCoord(Vector2I cellCoord)
        {
            var worldCell = GetWorldCell(cellCoord);

            if (worldCell == null)
            {
                return AtlasCoordWorld.SOLID;
            }

            return worldCell.AtlasCoord;
        }

        private bool IsSolid(Vector2I cellCoord)
        {
            var atlasCoord = GetAtlasCoord(cellCoord);
            return (atlasCoord == AtlasCoordWorld.SOLID || atlasCoord == AtlasCoordWorld.SOLID_WALL);
        }

        private static void SetCell(TileMapLayer tileMapLayer, Vector2I cellCoord, int sourceId = -1, Vector2I? atlasCoord = null, int alternateTile = 0, Color? modulate = null)
        {
            tileMapLayer.SetCell(cellCoord, sourceId,  atlasCoord, alternateTile);

            if (modulate.HasValue)
            {
                var tileData = tileMapLayer.GetCellTileData(cellCoord);
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
            return (atlasCoord == AtlasCoordWorld.SOLID || 
                    atlasCoord == AtlasCoordWorld.SOLID_WALL || 
                    atlasCoord == AtlasCoordWorld.WATER);
        }

        private void PlaceCat()
        {
            var cat = GetCat();

            if (cat != null)
            {
                var tileSize = _worldLayer.TileSet.TileSize;
                cat.Scale = new(tileSize.X / cat.Width, tileSize.Y / cat.Height);

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

                cat.Position = new(localPos.X, localPos.Y);
                _worldManager.UpdateCellCoord(cat.ID, selectedCellCoord);
                HideCell(selectedCellCoord);
            } 
        }

        private void MoveCat(double delta)
        {
            var cat = GetCat();

            if (cat == null)
            {
                return;
            }

            if (!_isCatMoving && _path.Count > 0)
            {
                _isCatMoving = true;

                var lastCatPos = _worldManager.GetCellCoord(cat.ID);
                if (lastCatPos != null)
                {
                    ShowCell(lastCatPos.Value);
                }

                var cellLocalPos = _path.Dequeue();
                HideCell(_worldLayer.LocalToMap(cellLocalPos));
                _nextCatPos = new(cellLocalPos.X + cat.Width / 2.0f, cellLocalPos.Y + cat.Height / 2.0f);
            }

            if (_isCatMoving)
            {
                cat.Position = cat.Position.Lerp(_nextCatPos, (float)(delta * CatSpeed));

                if (cat.Position.IsEqualApprox(_nextCatPos))
                {
                    _isCatMoving = false;
                    _worldManager.UpdateCellCoord(cat.ID, _worldLayer.LocalToMap(_nextCatPos));
                }
            }
        }

        private Cat GetCat()
        {
            var cat = _worldManager.GetAllGameObjects().FirstOrDefault();

            return (cat != null) ? cat as Cat : null;
        }

        private void HideCell(Vector2I cellCoord)
        {
            if (IsInBounds(cellCoord))
            {
                SetCell(_hideLayer, cellCoord, 0, new(0, 0));
            }
        }

        private void ShowCell(Vector2I cellCoord)
        {
            if (IsInBounds(cellCoord))
            {
                SetCell(_hideLayer, cellCoord);
            }
        }

        private WorldCell GetWorldCell(Vector2I cellCoord)
        {
            if (!IsInBounds(cellCoord))
            {
                return null;
            }

            return _worldCellArray[cellCoord.X, cellCoord.Y];
        }

        private bool IsInBounds(Vector2I cellCoord)
        {
            if (cellCoord.X < 0 || cellCoord.Y < 0 ||
                cellCoord.X >= Rows || cellCoord.Y >= Cols)
            {
                return false;
            }

            return true;
        }

        private void SelectArea(SelectionState selectionState)
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

                    Vector2I atlasCoord = GetAtlasCoordForSelection(i, j, startingRow, endingRow, startingCol, endingCol);

                    SelectCell(_selectLayer, cellCoord, atlasCoord, SelectionColor);
                }
            }
        }

        private static Vector2I GetAtlasCoordForSelection(int i, int j, int startingRow, int endingRow, int startingCol, int endingCol)
        {
            Vector2I atlasCoord;

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

        private void FindPath(Vector2 startPos, Vector2 endPos)
        {
            _pathLayer.Clear();
            _path.Clear();

            var start = _worldLayer.LocalToMap(startPos);
            var end = _worldLayer.LocalToMap(endPos);

            var path = _aStarGrid2d.GetPointPath(start, end);

            foreach (var point in path)
            {
                _path.Enqueue(point);

                var cellCoord =  _worldLayer.LocalToMap(point);

                SelectCell(_pathLayer, cellCoord, new(0, 0), PathColor);
            }
        }

        private void SelectCell(TileMapLayer tileMapLayer, Vector2I cellCoord, Vector2I atlasCoord, Color modulate)
        {
            if (_worldLayer.GetCellSourceId(cellCoord) != -1)
            {
                SetCell(tileMapLayer, cellCoord, 0, atlasCoord, 0, modulate);
            }
        }

        private Vector2I GetAtlasCoord(float heightNoiseVal)
        {
            if (heightNoiseVal < 25.0f)
            {
                return AtlasCoordWorld.WATER;
            }
            else if (heightNoiseVal < 65.0f)
            {
                return RandomChoice(_groundVariance, out _);
            }
            else //(heightNoiseVal < 100.0f)
            {
                return AtlasCoordWorld.SOLID;
            }
        }

        private Color GetTileColor(Vector2I atlasCoord, out int colorIndex)
        {
            colorIndex = 0;
            if (atlasCoord == AtlasCoordWorld.WATER)
            {
                return ColorConstants.BLUE;
            }
            else if (atlasCoord == AtlasCoordWorld.DIRT ||
                     atlasCoord == AtlasCoordWorld.GRASS_01 ||
                     atlasCoord == AtlasCoordWorld.GRASS_02 ||
                     atlasCoord == AtlasCoordWorld.GRASS_03)
            {
                return RandomChoice(_colorVariance, out colorIndex);
            }
            else if (atlasCoord == AtlasCoordWorld.SOLID_WALL)
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

        #endregion
    }
}
