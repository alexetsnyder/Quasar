using Godot;
using Quasar.scenes.common.interfaces;
using System.Collections.Generic;


namespace Quasar.scenes.work
{
    public partial class PathingSystem : Node2D
    {
        [Export]
        public Color PathColor { get; set; } = new Color(1.0f, 0.0f, 1.0f, 1.0f);

        [Export]
        public Node World { get; set; }

        private int _nextId = 0;

        private IMultiColorTileMapLayer _pathingTileMapLayer;

        private Dictionary<int, Path> _paths = [];

        private AStarGrid2D _aStarGrid2d = new();

        private Vector2I _atlasCoords = Vector2I.Zero;

        public override void _Ready()
        {
            _pathingTileMapLayer = GetNode<IMultiColorTileMapLayer>("PathingTileMapLayer");

            SetUpAStar();
        }

        public Path FindPath(Vector2 startPos, Vector2 endPos)
        {
            var start = _pathingTileMapLayer.LocalToMap(startPos);
            var end = _pathingTileMapLayer.LocalToMap(endPos);

            var points = _aStarGrid2d.GetPointPath(start, end);

            Queue<Vector2> pointQueue = [];

            foreach ( var point in points )
            {
                pointQueue.Enqueue(point);
            }

            _paths.Add(_nextId, new(_nextId, pointQueue));

            return _paths[_nextId++];
        }

        public void ShowPath(int id)
        {
            if (_paths.TryGetValue(id, out Path path))
            {
                foreach (var point in path.Points)
                {
                    SelectCell(point, _atlasCoords, PathColor);
                }
            } 
        }

        public void RemovePath(int id)
        {
            if (_paths.TryGetValue(id, out Path path))
            {
                foreach (var point in path.Points)
                {
                    SelectCell(point);
                }

                _paths.Remove(id);
            }
        }

        public void SetPointSolid(Vector2 localPos, bool solid = true)
        {
            _aStarGrid2d.SetPointSolid(_pathingTileMapLayer.LocalToMap(localPos), solid);
        }

        private void SelectCell(Vector2 localPos, Vector2I? atlasCoords = null, Color? color = null)
        {
            var coords = _pathingTileMapLayer.LocalToMap(localPos);
            _pathingTileMapLayer.SetCell(coords, atlasCoords, color);
        }

        private void SetUpAStar()
        {
            if (World is IWorld world)
            {
                _aStarGrid2d.Region = new Rect2I(0, 0, world.Rows + 1, world.Cols + 1);
                _aStarGrid2d.CellSize = _pathingTileMapLayer.TileSize;
                _aStarGrid2d.DefaultComputeHeuristic = AStarGrid2D.Heuristic.Manhattan;
                _aStarGrid2d.DefaultEstimateHeuristic = AStarGrid2D.Heuristic.Manhattan;
                _aStarGrid2d.DiagonalMode = AStarGrid2D.DiagonalModeEnum.Always;
                _aStarGrid2d.Update();

                foreach (var coords in world.GetAllPoints())
                {
                    if (world.IsImpassable(coords))
                    {
                        _aStarGrid2d.SetPointSolid(coords);
                    }
                }
            }
        }
    }
}
