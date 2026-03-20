using Godot;
using Quasar.data;
using Quasar.data.enums;
using Quasar.math;
using Quasar.scenes.common.interfaces;
using Quasar.scenes.systems.building;
using Quasar.system;
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
        public Node ItemSystemNode { get; set; }

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

        #endregion

        #region Private Variables

        private int _nextId = 0;

        private IItemSystem _itemSystem;

        private SimplexNoise _heightNoise;

        private RandomNumberGenerator _rng = new();

        private WorldCell[,] _worldCellArray;

        #endregion

        #region Public Methods

        public override void _Ready()
        {
            GlobalSystem.Instance.LoadInterface<IItemSystem>(ItemSystemNode, out _itemSystem);

            _gridTileMapLayer = GetNode<IMultiColorTileMapLayer>("GridTileMapLayer");
            _worldTileMapLayer = GetNode<IMultiColorTileMapLayer>("WorldTileMapLayer");

            _heightNoise = new SimplexNoise(_rng.RandiRange(int.MinValue, int.MaxValue));
            _worldCellArray = new WorldCell[Rows, Cols];

            GenerateWorld();
            FillMap();

            _gridTileMapLayer.Visible = ShowGrid;
        }

        public TileType GetTileType(Vector2 localPos)
        {
            var coords = _worldTileMapLayer.LocalToMap(localPos);
            var worldCell = GetWorldCell(coords);

            if (worldCell == null)
            {
                return TileType.NONE;
            }

            return worldCell.TileType;
        }

        public string GetTileTypeStr(Vector2 localPos)
        {
            var tileType = GetTileType(localPos);

            return tileType.ToString();
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

        public int GetWorldCellId(Vector2 localPos)
        {
            var coords = _worldTileMapLayer.LocalToMap(localPos);
            var wordCell = GetWorldCell(coords);
            if (wordCell == null)
            {
                return -1;
            }

            return wordCell.Id;
        }

        public List<Vector2> AllStorage()
        {
            List<Vector2> storagePosList = [];

            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Cols; j++)
                {
                    if (_worldCellArray[i, j].TileType == TileType.STORAGE)
                    {
                        storagePosList.Add(_worldTileMapLayer.MapToLocal(new(i, j)));
                    }
                }
            }

            return storagePosList;
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

        public void Build(Vector2 localPos, Buildable buildable)
        {
            var coords = _worldTileMapLayer.LocalToMap(localPos);

            if (!IsImpassable(coords))
            {
                var tileType = buildable.TileType;
                var atlasCoords = buildable.AtlasCoords;
                var color = buildable.Color;

                _worldCellArray[coords.X, coords.Y] = new(tileType, atlasCoords, color);

                if (tileType == TileType.STORAGE)
                {
                    _worldCellArray[coords.X, coords.Y].Id = _nextId++;
                }

                SetCell(_worldTileMapLayer, coords, atlasCoords, color);
            }
        }

        public void Cut(Vector2 localPos)
        {
            var coords = _worldTileMapLayer.LocalToMap(localPos);

            if (IsTree(coords))
            {
                UpdateWorldTile(TileType.DIRT, coords);
            }
        }

        public void Till(Vector2 localPos)
        {
            var coords = _worldTileMapLayer.LocalToMap(localPos);

            if (!IsImpassable(coords))
            {
                UpdateWorldTile(TileType.TILLED, coords);
            }
        }

        public void Gather(Vector2 localPos)
        {
            var coords = _worldTileMapLayer.LocalToMap(localPos);

            if (IsGatherable(coords))
            {

            }
        }

        public void Fish(Vector2 localPos)
        {
            var coords = _worldTileMapLayer.LocalToMap(localPos);

            if (IsWater(coords))
            {

            }
        }

        public bool IsSolid(Vector2 localPos)
        {
            var coords = _worldTileMapLayer.LocalToMap(localPos);
            return IsSolid(coords);
        }

        public bool IsSolid(Vector2I coords)
        {
            var worldCell = GetWorldCell(coords);
            if (worldCell == null)
            {
                return true;
            }

            return (worldCell.TileType == TileType.SOLID || 
                    worldCell.TileType == TileType.NATURAL_WALL || 
                    worldCell.TileType == TileType.WALL ||
                    worldCell.TileType == TileType.CORNER_WALL ||
                    worldCell.TileType == TileType.THREE_CONNECT_WALL ||
                    worldCell.TileType == TileType.FOUR_CONNECT_WALL ||
                    worldCell.TileType == TileType.STORAGE ||
                    worldCell.TileType == TileType.TREE);
        }

        public bool IsMineable(Vector2I coords)
        {
            var worldCell = GetWorldCell(coords);
            if (worldCell == null)
            {
                return false;
            }

            return (worldCell.TileType == TileType.SOLID || worldCell.TileType == TileType.NATURAL_WALL);
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

        public bool IsImpassable(Vector2 localPos)
        {
            return IsImpassable(_worldTileMapLayer.LocalToMap(localPos));
        }

        public bool IsImpassable(Vector2I coords)
        {
            var worldCell = GetWorldCell(coords);
            if (worldCell == null)
            {
                return true;
            }

            return (IsSolid(coords) || IsWater(coords));
        }

        public bool IsTree(Vector2I coords)
        {
            var worldCell = GetWorldCell(coords);
            if (worldCell == null)
            {
                return true;
            }

            return (worldCell.TileType == TileType.TREE);
        }

        public bool IsGatherable(Vector2I coords)
        {
            var worldCell = GetWorldCell(coords);
            if (worldCell == null)
            {
                return true;
            }

            return (worldCell.TileType == TileType.GRASS);
        }

        public bool HasItemsToHaul(Vector2I coords)
        {  
            return !IsSolid(coords) && _itemSystem.GetItems(_worldTileMapLayer.MapToLocal(coords)).Count > 0;
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

        #endregion

        #region Private Methods

        private void UpdateWorldTile(TileType tileType, Vector2I coords)
        {
            var atlasCoords = GetAtlasCoords(tileType);
            var color = GetCellColor(tileType);

            _worldCellArray[coords.X, coords.Y] = new(tileType, atlasCoords, color);

            SetCell(_worldTileMapLayer, coords, atlasCoords, color);
        }

        private void SetNaturalWall(Vector2I cellCoord)
        {
            UpdateWorldTile(TileType.NATURAL_WALL, cellCoord);
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
            GenerateTrees();
        }

        private void GenerateTrees()
        {
            var tileSize = _worldTileMapLayer.TileSize;
            var points = Random.PoissonDiskSampling(_rng, 200.0f, 30, Cols * tileSize.X, Rows * tileSize.Y);

            foreach (var point in points)
            {
                var coords = _worldTileMapLayer.LocalToMap(point);

                if (!IsImpassable(coords))
                {
                    var tileType = TileType.TREE;
                    var atlasCoords = GetAtlasCoords(tileType);
                    var color = GetCellColor(tileType);

                    _worldCellArray[coords.X, coords.Y] = new(tileType, atlasCoords, color);
                }
            }
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
                        var tileType = TileType.NATURAL_WALL;
                        var atlasCoords = GetAtlasCoords(tileType);
                        var color = GetCellColor(tileType);

                        _worldCellArray[i, j] = new(tileType, atlasCoords, color);
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
            return Random.RandomChoice<Vector2I>(_rng, AtlasConstants.AtlasCoords[tileType]);
        }

        private Color GetCellColor(TileType tileType)
        {
            return Random.RandomChoice<Color>(_rng, AtlasConstants.Colors[tileType]);
        }

        #endregion
    }
}
