using System;
using System.Collections.Generic;

namespace Quasar.core.naming
{
    public partial class NameManager
    {
        #region Singleton

        public static NameManager Instance { get => Nested.Instance; }

        private NameManager()
        {

        }

        private class Nested
        {
            static Nested()
            {

            }

            internal static readonly NameManager Instance = new NameManager();
        }

        #endregion

        private Dictionary<UInt32, string> _names = [];

        private UInt32 _nextId = 1;

        private static readonly object _lock = new();

        public UInt32 CreateOrFetchNameId(string name)
        {
            lock (_lock)
            {
                foreach (var kvp in _names)
                {
                    if (kvp.Value.Equals(name))
                    {
                        return kvp.Key;
                    }
                }

                var nameId = _nextId;
                _names.Add(_nextId++, name);
                return nameId;
            } 
        }

        public string FetchName(UInt32 nameId)
        {
            lock (_lock)
            {
                if (_names.TryGetValue(nameId, out var name))
                {
                    return name; 
                }

                return "## MISSING NAME ##";
            }
        }

    }
}