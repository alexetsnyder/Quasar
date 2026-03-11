using Godot;
using Quasar.core.naming;
using System.Collections.Generic;

namespace Quasar.core.blackboard
{
    public partial class Blackboard
    {
        private readonly Dictionary<FastName, int> _intValues = [];

        private readonly Dictionary<FastName, float> _floatValues = [];

        private readonly Dictionary<FastName, bool> _boolValues = [];

        private readonly Dictionary<FastName, Vector2> _vector2Values = [];

        public void Set(FastName key, int value)
        {
            _intValues[key] = value;
        }

        public void Set(FastName key, float value)
        {
            _floatValues[key] = value;
        }

        public void Set(FastName key, bool value)
        {
            _boolValues[key] = value;
        }

        public void Set(FastName key, Vector2 value)
        {
            _vector2Values[key] = value;
        }

        public bool TryGetInt(FastName key, out int value)
        {
            return TryGet(key, _intValues, out value);
        }

        public bool TryGetFloat(FastName key, out float value)
        {
            return TryGet(key, _floatValues, out value);
        }

        public bool TryGetBool(FastName key, out bool value)
        {
            return TryGet(key, _boolValues, out value);
        }

        public bool TryGetVector2(FastName key, out Vector2 value)
        {
            return TryGet(key, _vector2Values, out value);
        }

        private bool TryGet<T>(FastName key, Dictionary<FastName, T> values, out T value)
        {
            return values.TryGetValue(key, out value);
        }
    }
}