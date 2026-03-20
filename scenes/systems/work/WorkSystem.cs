using Godot;
using Quasar.data.enums;
using Quasar.scenes.cats;
using Quasar.scenes.common.interfaces;
using Quasar.scenes.systems.pathing;
using Quasar.scenes.systems.work.commands;
using Quasar.system;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Quasar.scenes.systems.work
{
    [GlobalClass]
    public partial class WorkSystem : Node, IWorkSystem
    {
        [Export]
        public Node PathingSystemNode { get; set; }

        [Export]
        public Node WorldNode { get; set; }

        [Export]
        public CommandFactory CommandFactory { get; set; }

        private IPathingSystem _pathingSystem;

        private IWorld _world;

        private int _nextId = 0;

        private readonly Dictionary<WorkType, Dictionary<int, Work>> _allWork = [];

        private static readonly object _lock = new();

        public override void _Ready()
        {
            GlobalSystem.Instance.LoadInterface<IPathingSystem>(PathingSystemNode, out  _pathingSystem);
            GlobalSystem.Instance.LoadInterface<IWorld>(WorldNode, out _world);
        }

        public bool AssignWork(Work work)
        {
            lock (_lock)
            {
                if (!work.IsAssigned)
                {
                    if (_allWork.TryGetValue(work.WorkType, out var workDict))
                    {
                        if (workDict.TryGetValue(work.WorkId, out _))
                        {
                            work.IsAssigned = true;
                            return true;
                        }
                    }
                }
                
                return false;
            }
        }

        public Work GetWork(int workId)
        {
            foreach (var workDict in _allWork.Values)
            {
                if (workDict.TryGetValue(workId, out var work))
                {
                    return work;
                }
            }

            return null;
        }
        
        public int CreateWork(WorkType workType, Vector2 localPos)
        {
            _allWork.TryAdd(workType, []);

            Work work = null;

            var command = CommandFactory.BuildCommand(workType, localPos);
            if (command != null)
            {
                var adjPosList = _world.GetAdjacentTiles(localPos).Where(a => !_world.IsImpassable(a));

                work = new(_nextId, workType, localPos, command, adjPosList.Any() ? [.. adjPosList] : null);
            }

            if (work != null)
            {
                _allWork[workType].Add(_nextId, work);
                return _nextId++;
            }

            return -1;
        }

        public void UpdateWork()
        {
            foreach (var workDict in _allWork.Values)
            {
                foreach (var work in workDict.Values)
                {
                    var adjPosList = _world.GetAdjacentTiles(work.LocalPos).Where(a => !_world.IsImpassable(a));
                    work.AdjPos = adjPosList.Any() ? [.. adjPosList] : null;
                }
            }
        }

        //public void LinkWork(int workId1, int workId2)
        //{
        //    var work1 = GetWork(workId1);
        //    var work2 = GetWork(workId2);

        //    if (work1 != null && work2 != null)
        //    {
        //        work1.LinkedWorkId = workId2;
        //        work2.LinkedWorkId = workId1;

        //        work2.IsDependent = true;
        //    }
        //}

        public void RemoveWork(Work work)
        {
            _allWork[work.WorkType].Remove(work.WorkId);
        }

        public void RemoveWork(List<Vector2> localPosList)
        {
            List<Tuple<WorkType, int>> removeList = [];

            foreach (var worldPos in localPosList)
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

        private Tuple<WorkType, int> FindWorkFromPos(Vector2 localPos)
        {
            foreach (var workDict in _allWork.Values)
            {
                foreach (var work in workDict.Values)
                {
                    if (work.LocalPos == localPos)
                    {
                        return new(work.WorkType, work.WorkId);
                    }
                }
            }

            return null;
        }

        public List<Work> CheckForWork(WorkType workType)
        {
            List<Work> workList = [];

            if (_allWork.TryGetValue(workType, out var workDict))
            {
                foreach (var work in workDict.Values)
                {
                    if (!work.IsAssigned)
                    {
                        workList.Add(work);
                    } 
                }
            }

            return workList;
        }

        public Tuple<List<Work>, Path> CheckForWork(Cat cat, bool assign = true)
        {
            var workType = cat.CatData.WorkType;

            if (_allWork.TryGetValue(workType, out Dictionary<int, Work> workDict))
            {
                if (workDict.Count > 0)
                {
                    var shortestPath = ShortestPath([.. workDict.Values.Where(w => !w.IsAssigned)], cat, out Work work);

                    if (work != null)
                    {
                        work.IsAssigned = assign;
                        List<Work> workList = [work];

                        //if (work.LinkedWorkId != -1)
                        //{
                        //    var linkedWork = GetWork(work.LinkedWorkId);
                        //    if (linkedWork != null)
                        //    {
                        //        linkedWork.IsAssigned = assign;
                        //        workList.Add(linkedWork);
                        //    }
                        //}

                        return new(workList, shortestPath);
                    }
                }
            }

            return null;
        }

        public List<Work> GetWork(WorkType workType)
        {
            _allWork.TryGetValue(workType, out Dictionary<int, Work> workDict);
            return [.. workDict.Select(kv => kv.Value)];
        }

        private Path ShortestPath(List<Work> workList, Cat cat, out Work work)
        {
            Path shortestPath = null;
            int minPathCount = int.MaxValue;
            work = null;

            foreach (var pWork in workList)
            {
                var adjPosList = _world.GetAdjacentTiles(pWork.LocalPos, true);

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
