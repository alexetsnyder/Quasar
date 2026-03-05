using Godot;
using Quasar.data.enums;
using Quasar.scenes.cats;
using Quasar.scenes.common.interfaces;
using Quasar.scenes.systems.building;
using Quasar.scenes.systems.items;
using Quasar.scenes.systems.pathing;
using Quasar.system;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Quasar.scenes.systems.work
{
    [GlobalClass]
    public partial class WorkSystem : Node
    {
        [Export]
        public Node PathingSystemNode { get; set; }

        [Export]
        public Node WorldNode { get; set; }

        private IPathingSystem _pathingSystem;

        private IWorld _world;

        private int _nextId = 0;

        private readonly Dictionary<WorkType, Dictionary<int, Work>> _allWork = [];

        public override void _Ready()
        {
            GlobalSystem.Instance.LoadInterface<IPathingSystem>(PathingSystemNode, out  _pathingSystem);
            GlobalSystem.Instance.LoadInterface<IWorld>(WorldNode, out _world);
        }

        public Work GetWork(Vector2 worldPos)
        {
            var workTuple = FindWorkFromPos(worldPos);
            if (workTuple != null)
            {
                return _allWork[workTuple.Item1][workTuple.Item2];
            }

            return null;
        }

        public void CreateWork(WorkType workType, List<Vector2> worldPosList, bool isAssigned = false, Buildable buildable = null, Item item = null)
        {
            _allWork.TryAdd(workType, []);
            
            foreach (var worldPos in worldPosList)
            {
                _allWork[workType].Add(_nextId, new(_nextId++, workType, worldPos, isAssigned, buildable, item));
            }
        }

        public Work CreateWork(WorkType workType, Vector2 worldPos, bool isAssigned = false, Buildable buildable = null, Item item = null)
        {
            _allWork.TryAdd(workType, []);

            _allWork[workType].Add(_nextId, new(_nextId, workType, worldPos, isAssigned, buildable, item));

            return _allWork[workType][_nextId++];
        }

        public void RemoveWork(Work work)
        {
            _allWork[work.WorkType].Remove(work.WorkId);
        }

        public void RemoveWork(List<Vector2> worldPosList)
        {
            List<Tuple<WorkType, int>> removeList = [];

            foreach (var worldPos in worldPosList)
            {
                var work = FindWorkFromPos(worldPos);
                if (work != null)
                {
                    removeList.Add(work);
                }
            }

            foreach (var work in removeList)
            {
                _allWork[work.Item1].Remove(work.Item2);
            }
        }

        private Tuple<WorkType, int> FindWorkFromPos(Vector2 worldPos)
        {
            foreach (var workDict in _allWork.Values)
            {
                foreach (var work in workDict.Values)
                {
                    if (work.WorldPos == worldPos)
                    {
                        return new(work.WorkType, work.WorkId);
                    }
                }
            }

            return null;
        }

        public Tuple<Work, Path> CheckForWork(Cat cat)
        {
            var workType = cat.CatData.WorkType;

            if (_allWork.TryGetValue(workType, out Dictionary<int, Work> workDict))
            {
                if (workDict.Count > 0)
                {
                    var shortestPath = ShortestPath([.. workDict.Values.Where(w => !w.IsAssigned)], cat, out Work work);

                    if (work != null)
                    {
                        work.IsAssigned = true;
                        return new(work, shortestPath);
                    }
                }
            }

            return null;
        }

        private Path ShortestPath(List<Work> workList, Cat cat, out Work work)
        {
            Path shortestPath = null;
            int minPathCount = int.MaxValue;
            work = null;

            foreach (var pWork in workList)
            {
                var adjPosList = _world.GetAdjacentTiles(pWork.WorldPos, true);

                var path = _pathingSystem.ShortestPath(cat.Position, adjPosList);

                if (path == null)
                {
                    continue; 
                }

                if (path.Id == -1)
                {
                    work = pWork;
                    return path;
                }

                if (path.Points.Count < minPathCount)
                {
                    if (shortestPath != null)
                    {
                        _pathingSystem.RemovePath(shortestPath.Id);
                    }

                    work = pWork;
                    shortestPath = path;
                    minPathCount = path.Points.Count;
                }
            }

            return shortestPath;
        }
    }
}
