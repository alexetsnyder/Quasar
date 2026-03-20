using Godot;
using Quasar.scenes.systems.items;
using Quasar.scenes.systems.pathing;
using Quasar.scenes.systems.work;
using System.Collections.Generic;

namespace Quasar.core.blackboard
{
    public partial class Blackboard<T>
    {
        private readonly Dictionary<T, int> _intValues = [];

        private readonly Dictionary<T, float> _floatValues = [];

        private readonly Dictionary<T, bool> _boolValues = [];

        private readonly Dictionary<T, Vector2> _vector2Values = [];

        private readonly Dictionary<T, Work> _workValues = [];

        private readonly Dictionary<T, Path> _pathValues = [];

        private readonly Dictionary<T, Item> _itemValues = [];

        private readonly Dictionary<T, List<Work>> _workListValues = [];

        public Blackboard() { }
        
        public Blackboard(Blackboard<T> blackboard)
        {
            _intValues = new(blackboard._intValues);
            _floatValues = new(blackboard._floatValues);
            _boolValues = new(blackboard._boolValues);
            _vector2Values = new(blackboard._vector2Values);
            _pathValues = new(blackboard._pathValues);
            _itemValues = new(blackboard._itemValues);
            _workListValues = new(blackboard._workListValues);
        }

        public void Set(T key, int value)
        {
            _intValues[key] = value;
        }

        public void Set(T key, float value)
        {
            _floatValues[key] = value;
        }

        public void Set(T key, bool value)
        {
            _boolValues[key] = value;
        }

        public void Set(T key, Vector2 value)
        {
            _vector2Values[key] = value;
        }

        public void Set(T key, Work value)
        {
            _workValues[key] = value;
        }

        public void Set(T key, Path value)
        {
            _pathValues[key] = value;
        }

        public void Set(T key, Item value)
        {
            _itemValues[key] = value;
        }

        public void Set(T key, List<Work> value)
        {
            _workListValues[key] = value;
        }

        public bool TryGetInt(T key, out int value)
        {
            return TryGet(key, _intValues, out value);
        }

        public bool TryGetFloat(T key, out float value)
        {
            return TryGet(key, _floatValues, out value);
        }

        public bool TryGetBool(T key, out bool value)
        {
            return TryGet(key, _boolValues, out value);
        }

        public bool TryGetVector2(T key, out Vector2 value)
        {
            return TryGet(key, _vector2Values, out value);
        }

        public bool TryGetWork(T key, out Work value)
        {
            return TryGet(key, _workValues, out value);
        }

        public bool TryGetPath(T key, out Path value)
        {
            return TryGet(key, _pathValues,  out value);
        }

        public bool TryGetItem(T key, out Item value)
        {
            return TryGet(key, _itemValues, out value);
        }

        public bool TryGetWorkList(T key, out List<Work> value)
        {
            return TryGet(key, _workListValues, out value);
        }

        private bool TryGet<U>(T key, Dictionary<T, U> values, out U value)
        {
            return values.TryGetValue(key, out value);
        }
    }
}