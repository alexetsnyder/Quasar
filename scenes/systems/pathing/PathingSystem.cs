using Catcophony.scenes.common.interfaces;
using Catcophony.system;
using Godot;
using System.Collections.Generic;

namespace Catcophony.scenes.systems.pathing
{
    public partial class PathingSystem : Node2D, IPathingSystem
    {
        [Export]
        public Color PathColor { get; set; } = new Color(1.0f, 0.0f, 1.0f, 1.0f);

        [Export]
        public Node WorldNode { get; set; }

        private IWorld _world;

        private int _nextId = 0;

        private IMultiColorTileMapLayer _pathingTileMapLayer;

        private readonly Dictionary<int, Path> _paths = [];

        private AStarGrid2D _aStarGrid2d = new();

        private Vector2I _atlasCoords = Vector2I.Zero;

        private readonly Dictionary<Vector2I, int> _pathReferences = [];

        public override void _Ready()
        {
            _pathingTileMapLayer = GetNode<IMultiColorTileMapLayer>("PathingTileMapLayer");

            GlobalSystem.Instance.LoadInterface<IWorld>(WorldNode, out _world);

            SetUpAStar();
        }

        public bool HasPath(Vector2 fromPos, Vector2 toPos)
        {
            if (fromPos.IsEqualApprox(toPos))
            {
                return true;
            }

            var pathQueue = FindPath(fromPos, toPos);

            if (pathQueue != null)
            {
                return true;
            }

            return false;
        }

        public Vector2? NearestAdjacentPoint(Vector2 fromPos, List<Vector2> toPosList)
        {
            Vector2? minPoint = null;
            int minPathCount = int.MaxValue;

            foreach (var toPos in toPosList)
            {
                if (fromPos.IsEqualApprox(toPos))
                {
                    return toPos;
                }

                foreach (var adjPos in _world.GetAdjacentTiles(toPos))
                {
                    if (fromPos.IsEqualApprox(adjPos))
                    {
                        return toPos;
                    }

                    var path = FindPath(fromPos, adjPos);

                    if (path != null && path.Count < minPathCount)
                    {
                        minPathCount = path.Count;
                        minPoint = toPos;
                    }
                }
            }

            return minPoint;
        }

        public Path ShortestPath(Vector2 startPos, List<Vector2> toPosList)
        {
            Queue<Vector2> points = null;
            int minPathCount = int.MaxValue;

            foreach (var toPos in toPosList)
            {
                if (startPos.IsEqualApprox(toPos))
                {
                    return new Path(-1, []);
                }

                var path = FindPath(startPos, toPos);

                if (path != null && path.Count < minPathCount)
                {
                    points = path;
                    minPathCount = path.Count;
                }
            }

            if (points != null)
            {
                var id = AddPath(points);

                return _paths[id];
            }

            return null;
        }

        private Queue<Vector2> FindPath(Vector2 startPos, Vector2 endPos)
        {
            var start = _pathingTileMapLayer.LocalToMap(startPos);
            var end = _pathingTileMapLayer.LocalToMap(endPos);

            var points = _aStarGrid2d.GetPointPath(start, end);

            if (points.Length > 0)
            {
                Queue<Vector2> pointQueue = [];

                foreach (var point in points)
                {
                    pointQueue.Enqueue(point);
                }

                return pointQueue;
            }

            return null;
        }

        public void ShowPath(int id)
        {
            if (_paths.TryGetValue(id, out Path path))
            {
                path.IsShown = true;
                foreach (var point in path.Points)
                {
                    AddPathReference(point);
                    SelectCell(point, _atlasCoords, PathColor);
                }
            } 
        }

        public int AddPath(Queue<Vector2> pointQueue)
        {
            _paths.Add(_nextId, new Path(_nextId, pointQueue));

            return _nextId++;
        }

        public void RemovePath(int id)
        {
            if (_paths.TryGetValue(id, out Path path))
            {
                foreach (var point in path.Points)
                {
                    if (path.IsShown)
                    {
                        RemovePathReference(point);
                    }                

                    if (IsTileSelected(point))
                    {    
                        if (GetPathReferences(point) == 0)
                        {
                            SelectCell(point);
                        }
                    }
                }

                _paths.Remove(id);
            }
        }

        public void SetPointSolid(Vector2 localPos, bool solid = true)
        {
            _aStarGrid2d.SetPointSolid(_pathingTileMapLayer.LocalToMap(localPos), solid);
        }

        private bool IsTileSelected(Vector2 localPos)
        {
            var coords = _pathingTileMapLayer.LocalToMap(localPos);

            return _pathingTileMapLayer.GetCellSourceId(coords) != -1;
        }

        private int GetPathReferences(Vector2 localPos)
        {
            var coords = _pathingTileMapLayer.LocalToMap(localPos);

            if (_pathReferences.TryGetValue(coords, out var count))
            {
                return count;
            }

            return 0;
        }

        private void AddPathReference(Vector2 localPos)
        {
            var coords = _pathingTileMapLayer.LocalToMap(localPos);

            if (_pathReferences.TryGetValue(coords, out int value))
            {
                _pathReferences[coords] = ++value;
            }
            else
            {
                _pathReferences.Add(coords, 1);
            }
        }

        private void RemovePathReference(Vector2 localPos)
        {
            var coords = _pathingTileMapLayer.LocalToMap(localPos);

            if (_pathReferences.TryGetValue(coords, out int value))
            {
                _pathReferences[coords] = --value;
            }
        }

        private void SelectCell(Vector2 localPos, Vector2I? atlasCoords = null, Color? color = null)
        {
            var coords = _pathingTileMapLayer.LocalToMap(localPos);
            _pathingTileMapLayer.SetCell(coords, atlasCoords, color);
        }

        private void SetUpAStar()
        {
            _aStarGrid2d.Region = new Rect2I(0, 0, _world.Rows + 1, _world.Cols + 1);
            _aStarGrid2d.CellSize = _pathingTileMapLayer.TileSize;
            _aStarGrid2d.DefaultComputeHeuristic = AStarGrid2D.Heuristic.Manhattan;
            _aStarGrid2d.DefaultEstimateHeuristic = AStarGrid2D.Heuristic.Manhattan;
            _aStarGrid2d.DiagonalMode = AStarGrid2D.DiagonalModeEnum.Always;
            _aStarGrid2d.Update();

            foreach (var coords in _world.GetAllPoints())
            {
                if (_world.IsImpassable(coords))
                {
                    _aStarGrid2d.SetPointSolid(coords);
                }
            }
        }
    }
}
