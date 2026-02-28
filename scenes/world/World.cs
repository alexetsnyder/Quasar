using Godot;
using Quasar.data;
using Quasar.data.enums;
using Quasar.math;
using Quasar.scenes.common.interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Quasar.scenes.world
{
    public partial class World : Node2D, IWorld
    {
        #region Exports

        [Export(PropertyHint.Range, "0,200")]
        public int Rows { get; set; } = 10;

        [Export(PropertyHint.Range, "0,200")]
        public int Cols { get; set; } = 10;

        [Export]
        public bool ShowGrid { get; set; } = false;

        [Export]
        public Color PathColor { get; set; } = new Color(1.0f, 0.0f, 1.0f, 1.0f);

        #endregion

        #region Signals

        #endregion

        #region Getters

        public Vector2 Center 
        { 
            get => _worldTileMapLayer.MapToLocal(new(Rows / 2, Cols / 2)); 
        }

        #endregion

        #region Children

        private IMultiColorTileMapLayer _gridTileMapLayer;

        private IMultiColorTileMapLayer _worldTileMapLayer;

        private IMultiColorTileMapLayer _pathTileMapLayer;

        #endregion

        #region Private Variables

        private SimplexNoise _heightNoise;

        private AStarGrid2D _aStarGrid2d = new();

        private RandomNumberGenerator _rng = new();

        private WorldCell[,] _worldCellArray;

        private readonly List<Vector2I> _groundVariance = [AtlasCoordWorld.GRASS_01, AtlasCoordWorld.GRASS_02, 
                                                           AtlasCoordWorld.GRASS_03, AtlasCoordWorld.GRASS_04];

        private readonly List<Color> _colorVariance = [ColorConstants.RED, ColorConstants.GREEN, ColorConstants.GRASS_GREEN, 
                                                       ColorConstants.YELLOW, ColorConstants.ORANGE, ColorConstants.AMBER];

        #endregion

        #region Public Methods

        public override void _Ready()
        {
            _gridTileMapLayer = GetNode<IMultiColorTileMapLayer>("GridTileMapLayer");
            _worldTileMapLayer = GetNode<IMultiColorTileMapLayer>("WorldTileMapLayer");
            _pathTileMapLayer = GetNode<IMultiColorTileMapLayer>("PathTileMapLayer");

            _heightNoise = new SimplexNoise(_rng.RandiRange(int.MinValue, int.MaxValue));
            _worldCellArray = new WorldCell[Rows, Cols];

            GenerateWorld();
            FillMap();
            SetUpAStar();

            _gridTileMapLayer.Visible = ShowGrid;
        }

        public string GetTileTypeStr(Vector2 localPos)
        {
            var coords = _worldTileMapLayer.LocalToMap(localPos);
            var worldCell = GetWorldCell(coords);

            if (worldCell == null)
            {
                return "NONE";
            }

            return AtlasCoordWorld.GetTileStrReflection(worldCell.AtlasCoords);
        }

        public string GetTileColorStr(Vector2 localPos)
        {
            var color = GetTileColor(localPos);

            if (color == null)
            {
                return "NONE";
            }

            return ColorConstants.GetColorStrReflection(color.Value);
        }

        public Color? GetTileColor(Vector2 localPos)
        {
            var coords = _worldTileMapLayer.LocalToMap(localPos);
            var worldCell = GetWorldCell(coords);

            if (worldCell == null)
            {
                return null;
            }

            return worldCell.Color;
        }

        /// <summary>
        /// Get spawn points in the largest connected area and closest to point.
        /// </summary>
        /// <param name="localPoint"></param>
        /// <returns>
        /// List of position in the world to spawn.
        /// </returns>
        public List<Vector2> GetSpawnPoints(Vector2 localPoint, int n = 1)
        {
            var toPoint = _worldTileMapLayer.LocalToMap(localPoint);
            var allPoints = GetAllPoints();
            var maxConnnectedArea = Math.MaxConnectedArea(allPoints, (v) => !IsImpassable(v));

            var coordsSpawnPoints = Math.MinDistanceToPoint(maxConnnectedArea, toPoint, n);

            List<Vector2> localSpawnPoints = [];

            foreach (var spawnPoint in coordsSpawnPoints)
            {
                if (spawnPoint != null)
                {
                    localSpawnPoints.Add(_worldTileMapLayer.MapToLocal(spawnPoint.Value));
                }
            }

            return localSpawnPoints; 
        }

        public List<Vector2> FindPath(Vector2 startPos, Vector2 endPos)
        {
            var start = _worldTileMapLayer.LocalToMap(startPos);
            var end = _worldTileMapLayer.LocalToMap(endPos);

            return [.._aStarGrid2d.GetPointPath(start, end)];
        }

        public void ShowPath(List<Vector2> path)
        {
            ClearPath();

            foreach (var point in path)
            {
                var coords = _worldTileMapLayer.LocalToMap(point);

                SelectCell(_pathTileMapLayer, coords, new(0, 0), PathColor);
            }

        }

        public void ClearPath()
        {
            _pathTileMapLayer.Clear();
        }

        public void PlaceItem(Vector2 newPos, Vector2? oldPos = null)
        {
            if (oldPos != null)
            {
                ShowCell(_worldTileMapLayer.LocalToMap(oldPos.Value));
            }

            HideCell(_worldTileMapLayer.LocalToMap(newPos));
        }

        public List<Vector2> GetAdjacentTiles(Vector2 localPos, bool includeDiagonals = false)
        {
            return [..GetAdjacentCells(_worldTileMapLayer.LocalToMap(localPos), includeDiagonals).Select(a => _worldTileMapLayer.MapToLocal(a))];
        }

        public void Work(WorkType workType, Vector2 localPos, BuildingType? buildingType = null)
        {
            switch(workType)
            {
                case WorkType.MINING:
                    Mine(localPos);
                    break;
                case WorkType.BUILDING:
                    Build(localPos, buildingType.Value);
                    break;
                case WorkType.FARMING:
                    Till(localPos);
                    break;
                case WorkType.FISHING:
                    Fish(localPos);
                    break;
                default:
                    GD.Print("Work not Implimented Yet in World.Work");
                    break;
            }
        }

        public void Mine(Vector2 localPos)
        {
            var coords = _worldTileMapLayer.LocalToMap(localPos);

            if (IsSolid(coords))
            {
                UpdateWorldTile(TileType.DIRT, coords);

                foreach (var adjCell in GetAdjacentCells(coords, true))
                {
                    if (IsSolid(adjCell))
                    {
                        SetNaturalWall(adjCell);
                    }
                }
            }
        }

        public void Build(Vector2 localPos, BuildingType buildingType)
        {
            var coords = _worldTileMapLayer.LocalToMap(localPos);

            if (!IsImpassable(coords))
            {
                var atlasCoords = GetAtlasCoords(buildingType);
                var color = GetCellColor(TileType.WALL);

                _worldCellArray[coords.X, coords.Y] = new(TileType.WALL, atlasCoords, color);

                SetCell(_worldTileMapLayer, coords, atlasCoords, color);
                _aStarGrid2d.SetPointSolid(coords, true);

            }
        }

        public void Till(Vector2 localPos)
        {
            var coords = _worldTileMapLayer.LocalToMap(localPos);

            if (!IsImpassable(coords))
            {
                UpdateWorldTile(TileType.TILLED, coords, false);
            }
        }

        public void Fish(Vector2 localPos)
        {
            var coords = _worldTileMapLayer.LocalToMap(localPos);

            if (IsWater(coords))
            {

            }
        }

        public bool IsSolid(Vector2I coords)
        {
            var worldCell = GetWorldCell(coords);
            if (worldCell == null)
            {
                return true;
            }

            return (worldCell.TileType == TileType.SOLID || worldCell.TileType == TileType.NATURAL_WALL || worldCell.TileType == TileType.WALL);
        }

        public bool IsWater(Vector2I coords)
        {
            var worldCell = GetWorldCell(coords);
            if (worldCell == null)
            {
                return false;
            }

            return (worldCell.TileType == TileType.WATER);
        }

        public bool IsImpassable(Vector2I coords)
        {
            var worldCell = GetWorldCell(coords);
            if (worldCell == null)
            {
                return true;
            }

            return (worldCell.TileType == TileType.SOLID ||
                    worldCell.TileType == TileType.NATURAL_WALL ||
                    worldCell.TileType == TileType.WALL ||
                    worldCell.TileType == TileType.WATER);
        }

        public bool IsInBounds(Vector2I coords)
        {
            if (coords.X < 0 || coords.Y < 0 ||
                coords.X >= Rows || coords.Y >= Cols)
            {
                return false;
            }

            return true;
        }

        public bool TileOccupied(Vector2 localPos)
        {
            var coords = _worldTileMapLayer.LocalToMap(localPos);
            if (_worldTileMapLayer.GetCellSourceId(coords) == -1)
            {
                return true;
            }

            return false;
        }

        #endregion

        #region Private Methods

        private void UpdateWorldTile(TileType tileType, Vector2I coords, bool isSolid = false)
        {
            var atlasCoords = GetAtlasCoords(tileType);
            var color = GetCellColor(tileType);

            _worldCellArray[coords.X, coords.Y] = new(tileType, atlasCoords, color);

            SetCell(_worldTileMapLayer, coords, atlasCoords, color);
            _aStarGrid2d.SetPointSolid(coords, isSolid);
        }

        private void SetNaturalWall(Vector2I cellCoord)
        {
            UpdateWorldTile(TileType.NATURAL_WALL, cellCoord, true);
        }

        private void GenerateWorld()
        {
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Cols; j++)
                {
                    var noiseVal = _heightNoise.GetNoise(j, i);
                    var tileType = GetTileType(noiseVal);
                    var atlasCoords = GetAtlasCoords(tileType);
                    var color = GetCellColor(tileType);
                    _worldCellArray[i, j] = new(tileType, atlasCoords, color);
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
                    var coords = new Vector2I(i, j);

                    if (IsEdge(coords))
                    {
                        _worldCellArray[i, j] = new(TileType.NATURAL_WALL, AtlasCoordWorld.SOLID_WALL, ColorConstants.WALL_PURPLE);
                    }
                }
            }
        }

        private bool IsEdge(Vector2I coords)
        {
            if (IsSolid(coords))
            {
                foreach (var adjCoords in GetAdjacentCells(coords, true))
                {
                    if (!IsSolid(adjCoords))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private static List<Vector2I> GetAdjacentCells(Vector2I coords, bool includeDiagonals = false)
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

            return [.. adjDirs.Select(n => coords + n)];
        }

        private void FillMap()
        {
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Cols; j++)
                {
                    var coords = new Vector2I(i, j);
                    var worldCell = _worldCellArray[i, j];

                    SetCell(_worldTileMapLayer, coords, worldCell.AtlasCoords, worldCell.Color);
                    SetCell(_gridTileMapLayer, coords, new(0, 0), ColorConstants.GREY);
                }
            }
        }

        private static void SetCell(IMultiColorTileMapLayer tileMapLayer, Vector2I coords, Vector2I? atlasCoords = null, Color? color = null)
        {
            tileMapLayer.SetCell(coords, atlasCoords, color);
        }

        private void SelectCell(IMultiColorTileMapLayer tileMapLayer, Vector2I coords, Vector2I? atlasCoords = null, Color? color = null)
        {
            if (_worldTileMapLayer.GetCellSourceId(coords) != -1)
            {
                SetCell(tileMapLayer, coords, atlasCoords, color);
            }
        }

        private void SetUpAStar()
        {
            _aStarGrid2d.Region = new Rect2I(0, 0, Rows + 1, Cols + 1);
            _aStarGrid2d.CellSize = _worldTileMapLayer.TileSize;
            _aStarGrid2d.DefaultComputeHeuristic = AStarGrid2D.Heuristic.Manhattan;
            _aStarGrid2d.DefaultEstimateHeuristic = AStarGrid2D.Heuristic.Manhattan;
            _aStarGrid2d.DiagonalMode = AStarGrid2D.DiagonalModeEnum.Always;
            _aStarGrid2d.Update();

            foreach (var coords in _worldTileMapLayer.GetUsedCellsById())
            {
                if (IsImpassable(coords))
                {
                    _aStarGrid2d.SetPointSolid(coords);
                }
            }
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

        private void HideCell(Vector2I coords)
        {
            if (IsInBounds(coords))
            {
                SetCell(_worldTileMapLayer, coords);
            }
        }

        private void ShowCell(Vector2I cellCoord)
        {
            var worldCell = GetWorldCell(cellCoord);

            if (worldCell != null)
            {
                SetCell(_worldTileMapLayer, cellCoord, worldCell.AtlasCoords, worldCell.Color);
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

        private static TileType GetTileType(float heightNoiseVal)
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

        private Vector2I GetAtlasCoords(TileType tileType)
        {
            switch (tileType)
            {
                case TileType.WATER:
                    return AtlasCoordWorld.WATER;
                case TileType.GRASS:
                    return Random.RandomChoice<Vector2I>(_rng, _groundVariance);
                case TileType.DIRT:
                    return AtlasCoordWorld.DIRT;
                case TileType.WALL:
                    return AtlasCoordWorld.WALL;
                case TileType.TILLED:
                    return AtlasCoordWorld.DIRT;
                case TileType.NATURAL_WALL:
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
        private Color GetCellColor(TileType tileType)
        {
            switch (tileType)
            {
                case TileType.WATER:
                    return ColorConstants.BLUE;
                case TileType.GRASS:
                    return Random.RandomChoice<Color>(_rng, _colorVariance);
                case TileType.DIRT:
                    return ColorConstants.BURNT_ORANGE;
                case TileType.WALL:
                    return ColorConstants.GREY;
                case TileType.TILLED:
                    return ColorConstants.LAVENDER;
                case TileType.NATURAL_WALL:
                    return ColorConstants.WALL_PURPLE;
                case TileType.SOLID:
                    return ColorConstants.BLACK;
                default:
                    return ColorConstants.BLACK;
            }
        }

        private Vector2I GetAtlasCoords(BuildingType buildingType)
        {
            switch (buildingType)
            {
                case BuildingType.VERTICAL_WALL:
                    return new(10, 11);
                case BuildingType.THREEWAY_CONNECT_UP_WALL:
                    return new(10, 12);
                case BuildingType.HORIZONTAL_WALL:
                    return new(13, 12);
                case BuildingType.LEFTTOP_CORNER_WALL:
                    return new(9, 12);
                default:
                    return new(2, 11);
            }
        }

        #endregion
    }
}
