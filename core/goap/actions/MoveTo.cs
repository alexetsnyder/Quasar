using Quasar.core.blackboard;
using Quasar.core.common;
using Quasar.core.goap.interfaces;
using Quasar.core.naming;
using System.Collections.Generic;

namespace Quasar.core.goap.actions
{
    public partial class MoveTo : IAction
    {
        public FastName Name { get; set; } = new("MoveToAction");

        public int Cost { get; set; } = 2;

        private readonly Dictionary<FastName, bool> _preconditions = new()
        {
            { Constants.Names.HasPath, true },
        };

        private readonly Dictionary<FastName, bool> _effects = new()
        {
            { Constants.Names.IsAdjToWork, true },
        };

        public Dictionary<FastName, bool> GetPreconds()
        {
            return _preconditions; 
        }

        public bool SatisfyGoal(KeyValuePair<FastName, bool> goal)
        {
            if (_effects.TryGetValue(goal.Key, out var value))
            {
                if (goal.Value == value)
                {
                    return true;
                }
            }

            return false;
        }

        public bool SatisfyPreconds(Blackboard blackboard)
        {
            foreach (var key in _preconditions.Keys)
            {
                if (blackboard.TryGetBool(key, out bool value))
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

        public void Excecute(Blackboard blackboard)
        {
            foreach (var effect in _effects)
            {
                blackboard.Set(effect.Key, effect.Value);
            }
        }
    }
}
