using System;
using System.Collections.Generic;

namespace Quasar.core.naming
{
    public partial class FastName : IEquatable<FastName>, IComparable<FastName>
    {
        public static readonly FastName None = new();

        private readonly UInt32 _nameId;

        private FastName()
        {
            _nameId = 0;
        }

        public FastName(string name)
        {
            _nameId = NameManager.Instance.CreateOrFetchNameId(name);
        }

        public override string ToString()
        {
            if (this == None)
            {
                return "## NONE ##";
            }

            return NameManager.Instance.FetchName(_nameId);
        }

        public bool Equals(FastName other)
        {
            if (other == null)
            {
                return false;
            }

            return _nameId == other._nameId;
        }

        public int CompareTo(FastName other)
        {
            return _nameId.CompareTo(other._nameId);
        }

        public override bool Equals(object obj)
        {
            if (obj is FastName fName)
            {
                return _nameId == fName._nameId;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return _nameId.GetHashCode();
        }

        public static bool operator ==(FastName left, FastName right)
        {
            return EqualityComparer<FastName>.Default.Equals(left, right);
        }

        public static bool operator !=(FastName left, FastName right)
        {
            return !(left == right);
        }

        public static implicit operator bool(FastName other)
        {
            return other != None;
        }
    }
}

