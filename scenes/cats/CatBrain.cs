using Catcophony.core.goap.goals;
using Catcophony.core.goap.interfaces;

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
    }
}
