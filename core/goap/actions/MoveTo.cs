using Quasar.core.blackboard;
using Quasar.core.common;
using Quasar.core.naming;
using System.Collections.Generic;

namespace Quasar.core.goap.actions
{
    public partial class MoveTo
    {
        private Blackboard LinkToBlackboard;

        public readonly Dictionary<FastName, bool> _preconditions = new()
        {
            { Constants.Names.HasPath, true },
        };

        public readonly Dictionary<FastName, bool> _effects = new()
        {
            { Constants.Names.IsAdjToWork, true },
        };

        public void LinkBlackboard(Blackboard blackboard)
        {
            LinkToBlackboard = blackboard;
        }

        public bool CheckPreconditions()
        {
            foreach (var key in _preconditions.Keys)
            {
                if (LinkToBlackboard.TryGetBool(key, out bool value))
                {
                    if (value != _preconditions[key])
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        public void Excecute()
        {
            foreach (var effect in _effects)
            {
                LinkToBlackboard.Set(effect.Key, effect.Value);
            }
        }
    }
}
