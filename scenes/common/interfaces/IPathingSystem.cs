using Godot;
using Catcophony.scenes.systems.pathing;
using System.Collections.Generic;

namespace Catcophony.scenes.common.interfaces
{
    public interface IPathingSystem
    {
        public bool HasPath(Vector2 fromPos, Vector2 toPos);

        public Vector2? NearestAdjacentPoint(Vector2 fromPos, List<Vector2> toPosList);

        public Path ShortestPath(Vector2 startPos, List<Vector2> toPosList);

        public Path FindPath(Vector2 fromPos, Vector2 toPos);

        public void ShowPath(int id);

        public void RemovePath(int id);

        public void SetPointSolid(Vector2 localPos, bool solid = true);
    }
}