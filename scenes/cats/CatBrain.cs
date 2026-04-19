using Catcophony.core.enums;
using Catcophony.core.goap.goals;
using Catcophony.core.goap.interfaces;
using Godot;

namespace Catcophony.scenes.cats
{
    public partial class CatBrain
    {
        public static IGoal EvaluateGoal(CatModel catModel)
        {
            if (catModel.Thirst < 25)
            {
                return new WaterGoal();
            }

            return new WorkGoal();
        }

        public static IdleType EvaulateIdle(RandomNumberGenerator rng)
        {
            IdleType idleType;

            var number = rng.RandiRange(0, 100);

            if (number < 10)
            {
                idleType = IdleType.WANDER;
            }
            else if (number < 20)
            {
                idleType = IdleType.RETURN;
            }
            else
            {
                idleType = IdleType.IDLE;
            }

            return idleType;
        }
    }
}
