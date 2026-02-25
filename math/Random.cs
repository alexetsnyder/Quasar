using Godot;
using System.Collections.Generic;

namespace Quasar.math
{
    public static partial class Random
    {
        public static T RandomChoice<T>(RandomNumberGenerator rng, List<T> alts)
        {
            if (alts.Count <= 0)
            {
                return default;
            }

            return alts[rng.RandiRange(0, alts.Count - 1)];
        }
    }
}