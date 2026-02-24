using Godot;
using Godot.Collections;
using Quasar.data;
using Quasar.data.enums;
using Quasar.math;
using Quasar.scenes.work;
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
        public bool ShowGrid { get; set; } = false;

        [Export]
        public Color SelectionColor { get; set; } = new Color(1.0f, 0.0f, 0.0f, 1.0f);

        [Export]
        public Color PathColor { get; set; } = new Color(1.0f, 0.0f, 1.0f, 1.0f);

        #endregion

        #region Signals

        [Signal]
        public delegate void TileSelectedEventHandler(Vector2 tilePos);

        [Signal]
        public delegate void CreateWorkEventHandler(Array<Work> workArray);

        [Signal]
        public delegate void CancelWorkEventHandler(Array<Vector2> worldPosArray);

        #endregion

        #region Children

        private TileMapLayer _gridLayer;

        private TileMapLayer _worldLayer;

        private TileMapLayer _hideLayer;

        private TileMapLayer _selectLayer;

        private TileMapLayer _pathLayer;

        private TileMapLayer _selectingLayer;

        private ColorRect _selectionRect;

        #endregion

        #region Private Variables

        private SelectionState _selectionState = SelectionState.SINGLE;

        private bool _isWorking = false;

        private bool _isSelecting = false;

        private Vector2 _selectionStart;

        private SimplexNoise _heightNoise;

        private AStarGrid2D _aStarGrid2d = new();

        private RandomNumberGenerator _rng = new();

        private WorldCell[,] _worldCellArray;

        private readonly List<Vector2I> _groundVariance = [AtlasCoordWorld.GRASS_01, AtlasCoordWorld.GRASS_02, 
                                                           AtlasCoordWorld.GRASS_03, AtlasCoordWorld.GRASS_04];

        private readonly List<Color> _colorVariance = [ColorConstants.RED, ColorConstants.GREEN, ColorConstants.GRASS_GREEN, 
                                                       ColorConstants.YELLOW, ColorConstants.ORANGE, ColorConstants.AMBER];

        #region TileMapLayer SourceIds

        private int _gridLayerSourceId = 1;

        private int _worldLayerSourceId = 0;

        private int _hideLayerSourceId = 0;

        private int _selectingLayerSourceId = 0;

        private int _selectLayerSourceId = 0;

        private int _pathLayerSourceId = 0;

        #endregion

        #endregion

        #region Public Methods

        public override void _Ready()
        {
            _rng.Randomize();

            _gridLayer = GetNode<TileMapLayer>("GridLayer");
            _worldLayer = GetNode<TileMapLayer>("WorldLayer");
            _hideLayer = GetNode<TileMapLayer>("HideLayer");
            _selectingLayer = GetNode<TileMapLayer>("SelectingLayer");
            _selectLayer = GetNode<TileMapLayer>("SelectLayer");
            _pathLayer = GetNode<TileMapLayer>("PathLayer");
            _selectionRect = GetNode<ColorRect>("SelectionRect");

            _heightNoise = new SimplexNoise(_rng.RandiRange(int.MinValue, int.MaxValue));
            _worldCellArray = new WorldCell[Rows, Cols];

            GenerateWorld();
            FillMap();
            SetUpAStar();

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
                        switch (_selectionState)
                        {
                            case SelectionState.SINGLE:
                                var mousePos = GetGlobalMousePosition();
                                if (!_isWorking)
                                {
                                    EmitSignal(SignalName.TileSelected, mousePos);
                                }
                                break;
                            case SelectionState.DIGGING:
                            case SelectionState.FARMING:
                            case SelectionState.CANCEL:
                                _isSelecting = true;
                                _selectionStart = GetGlobalMousePosition();
                                _selectionRect.Position = _selectionStart;
                                _selectionRect.Size = new Vector2();
                                break;
                            case SelectionState.BUILDING:
                                break;
                            default:
                                GD.Print($"Selection State {_selectionState} set incorrectly.");
                                break;
                        }
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

        public Vector2? PlaceCat()
        {
            var maxConnnectedArea = Math.MaxConnectedArea(GetAllPoints(), (v) => IsInBounds(v) && !IsImpassable(v));

            Vector2I center = new(Rows / 2, Cols / 2);

            var cellCoord = Math.MinDistanceToPoint(maxConnnectedArea, center);

            if (cellCoord != null)
            {
                HideCell(cellCoord.Value);
                return _worldLayer.MapToLocal(cellCoord.Value);
            }

            return null;
        }

        public List<Vector2> FindPath(Vector2 startPos, Vector2 endPos)
        {
            var start = _worldLayer.LocalToMap(startPos);
            var end = _worldLayer.LocalToMap(endPos);

            return [.._aStarGrid2d.GetPointPath(start, end)];
        }

        public void ShowPath(List<Vector2> path)
        {
            ClearPath();

            foreach (var point in path)
            {
                var cellCoord = _worldLayer.LocalToMap(point);

                SelectCell(_pathLayer, cellCoord, _pathLayerSourceId, new(0, 0), PathColor);
            }

        }

        public void ClearPath()
        {
            _pathLayer.Clear();
        }

        public void HideTile(Vector2 localPos)
        {
            HideCell(_worldLayer.LocalToMap(localPos));
        }

        public void ShowTile(Vector2 localPos)
        {
            ShowCell(_worldLayer.LocalToMap(localPos));
        }

        public List<Vector2> GetAdjacentTiles(Vector2 localPos, bool includeDiagonals = false)
        {
            return [..GetAdjacentCells(_worldLayer.LocalToMap(localPos), includeDiagonals).Select(a => _worldLayer.MapToLocal(a))];
        }

        public void Dig(Vector2 localPos)
        {
            var cellCoord = _worldLayer.LocalToMap(localPos);

            if (IsSolid(cellCoord) && _selectLayer.GetCellSourceId(cellCoord) != -1)
            {

                UpdateWorldTile(TileType.DIRT, cellCoord);
                SetCell(_selectLayer, cellCoord);
                _aStarGrid2d.SetPointSolid(cellCoord, false);

                foreach (var adjCell in GetAdjacentCells(cellCoord, true))
                {
                    if (IsSolid(adjCell))
                    {
                        SetWall(adjCell);
                    }
                }
            }
        }

        #endregion

        #region Private Methods

        private void UpdateWorldTile(TileType tileType, Vector2I cellCoord)
        {
            var atlasCoord = GetAtlasCoord(tileType);
            var modulate = GetTileColor(tileType, out int alternativeTile);

            _worldCellArray[cellCoord.X, cellCoord.Y] = new(tileType, atlasCoord, modulate, alternativeTile);

            SetCell(_worldLayer, cellCoord, _worldLayerSourceId, atlasCoord, alternativeTile, modulate);
        }

        private void SetWall(Vector2I cellCoord)
        {
            UpdateWorldTile(TileType.SOLID_WALL, cellCoord);
        }

        private void GenerateWorld()
        {
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Cols; j++)
                {
                    var noiseVal = _heightNoise.GetNoise(j, i);
                    var tileType = GetTileType(noiseVal);
                    var atlasCoord = GetAtlasCoord(tileType);
                    var modulate = GetTileColor(tileType, out int alternativeTile);
                    _worldCellArray[i, j] = new(tileType, atlasCoord, modulate, alternativeTile);
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
                        SetWall(cellCoord);
                    }
                }
            }
        }

        private bool IsEdge(Vector2I cellCoord)
        {
            if (IsSolid(cellCoord))
            {
                foreach (var adjCellCoord in GetAdjacentCells(cellCoord, true))
                {
                    if (!IsSolid(adjCellCoord))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private static List<Vector2I> GetAdjacentCells(Vector2I cellCoord, bool includeDiagonals = false)
        {
            List<Vector2I> adjDirs = [];

            if (includeDiagonals)
            {
                adjDirs.AddRange([new(1, 0), new(1, 1),
                                  new(1, -1), new(0, 1),
                                  new(-1, 1), new(0, -1),
                                  new(-1, -1), new(-1, 0)]);
            }
            else
            {
                adjDirs.AddRange([new(1, 0), new(0, 1),
                                  new(-1, 0), new(0, -1)]);
            }

            return [.. adjDirs.Select(n => cellCoord + n)];
        }

        private void FillMap()
        {
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Cols; j++)
                {
                    var cellCoord = new Vector2I(i, j);
                    var worldCell = _worldCellArray[i, j];

                    SetCell(_worldLayer, cellCoord, _worldLayerSourceId, worldCell.AtlasCoord, worldCell.AlternateTile, worldCell.Modulate);
                    SetCell(_gridLayer, cellCoord, _gridLayerSourceId, new(0, 0), 0, ColorConstants.GREY);
                }
            }
        }

        private bool IsSolid(Vector2I tileCoord)
        {
            var worldCell = GetWorldCell(tileCoord);
            if (worldCell == null)
            {
                return true;
            }

            return (worldCell.TileType == TileType.SOLID || worldCell.TileType == TileType.SOLID_WALL);
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
            _aStarGrid2d.DiagonalMode = AStarGrid2D.DiagonalModeEnum.Always;
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
            var worldCell = GetWorldCell(cellCoord);
            if (worldCell == null)
            {
                return true;
            }

            return (worldCell.TileType == TileType.SOLID ||
                    worldCell.TileType == TileType.SOLID_WALL ||
                    worldCell.TileType == TileType.WATER);
        }

        public List<Vector2I> GetAllPoints()
        {
            List<Vector2I> allPoints = [];

            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Cols; j++)
                {
                    allPoints.Add(new(i, j));
                }
            }

            return allPoints;
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

        private void SetSelectingArea()
        {
            _selectingLayer.Clear();

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

                    if (_selectionState == SelectionState.CANCEL)
                    {
                        SelectCell(_selectingLayer, cellCoord, _selectingLayerSourceId, AtlasCoordSelection.CANCEL, ColorConstants.WARNING_RED);
                    }
                    else
                    {
                        var atlasCoord = GetAtlasCoordForSelection(i, j, startingRow, endingRow, startingCol, endingCol);
                        SelectCell(_selectingLayer, cellCoord, _selectingLayerSourceId, atlasCoord, SelectionColor);
                    }  
                }
            }
        }

        private void SelectArea()
        {
            _selectingLayer.Clear();

            var tileSize = _worldLayer.TileSet.TileSize;
            var left = _selectionRect.Position.X;
            var right = left + _selectionRect.Size.X;
            var top = _selectionRect.Position.Y;
            var bottom = top + _selectionRect.Size.Y;

            var startingCol = Mathf.FloorToInt(left / tileSize.X);
            var endingCol = Mathf.CeilToInt(right / tileSize.X);
            var startingRow = Mathf.FloorToInt(top / tileSize.Y);
            var endingRow = Mathf.CeilToInt(bottom / tileSize.Y);

            if (_selectionState == SelectionState.DIGGING)
            {
                Array<Work> workList = [];
                for (int i = startingRow; i < endingRow; i++)
                {
                    for (int j = startingCol; j < endingCol; j++)
                    {
                        var cellCoord = new Vector2I(j, i);
                        if (IsSolid(cellCoord) && _selectLayer.GetCellSourceId(cellCoord) == -1)
                        {
                            SelectCell(_selectLayer, cellCoord, _selectLayerSourceId, AtlasCoordSelection.DIG, ColorConstants.GREY);
                            workList.Add(new("DIGGING", WorkType.DIGGING, _worldLayer.MapToLocal(cellCoord)));
                        }
                    }
                }

                EmitSignal(SignalName.CreateWork, workList);
            }
            else if (_selectionState == SelectionState.CANCEL)
            {
                Array<Vector2> worldPosList = [];

                for (int i = startingRow; i < endingRow; i++)
                {
                    for (int j = startingCol; j < endingCol; j++)
                    {
                        var cellCoord = new Vector2I(j, i);

                        if (_selectLayer.GetCellSourceId(cellCoord) != -1)
                        {
                            worldPosList.Add(_worldLayer.MapToLocal(cellCoord));
                            SetCell(_selectLayer, cellCoord);
                        }
                    }
                } 

                EmitSignal(SignalName.CancelWork, worldPosList);
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

        private void SelectCell(TileMapLayer tileMapLayer, Vector2I cellCoord, int sourceId, Vector2I atlasCoord, Color? modulate = null)
        {
            if (_worldLayer.GetCellSourceId(cellCoord) != -1)
            {
                SetCell(tileMapLayer, cellCoord, sourceId, atlasCoord, 0, modulate);
            }
        }

        private TileType GetTileType(float heightNoiseVal)
        {
            if (heightNoiseVal < 25.0f)
            {
                return TileType.WATER;
            }
            else if (heightNoiseVal < 65.0f)
            {
                return TileType.GRASS;
            }
            else //(heightNoiseVal < 100.0f)
            {
                return TileType.SOLID;
            }
        }

        private Vector2I GetAtlasCoord(TileType tileType)
        {
            switch (tileType)
            {
                case TileType.WATER:
                    return AtlasCoordWorld.WATER;
                case TileType.GRASS:
                    return RandomChoice(_groundVariance, out _);
                case TileType.DIRT:
                    return AtlasCoordWorld.DIRT;
                case TileType.SOLID_WALL:
                    return AtlasCoordWorld.SOLID_WALL;
                case TileType.SOLID:
                    return AtlasCoordWorld.SOLID;
                default:
                    return AtlasCoordWorld.SOLID;
            }
        }

        /// <summary>
        /// Returns the modulate color of tile given tile type.
        /// </summary>
        /// <param name="tileType"></param>
        /// <param name="alternativeTile">
        /// List Index matches alternative tiles in TileSet.
        /// </param>
        /// <returns> Return Tile Color </returns>
        private Color GetTileColor(TileType tileType, out int alternativeTile)
        {
            alternativeTile = 0; 
            switch (tileType)
            {
                case TileType.WATER:
                    return ColorConstants.BLUE;
                case TileType.GRASS:
                    return RandomChoice(_colorVariance, out alternativeTile);
                case TileType.DIRT:
                    alternativeTile = 1;
                    return ColorConstants.BURNT_ORANGE;
                case TileType.SOLID_WALL:
                    return ColorConstants.WALL_PURPLE;
                case TileType.SOLID:
                    return ColorConstants.BLACK;
                default:
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
