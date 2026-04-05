using Catcophony.core.actions.interfaces;

namespace Catcophony.core.actions.commands
{
    public partial class DrinkingCommand() : ICommand
    {
        public void Execute(IActor actor = null)
        {
            actor?.Drink();
        }
    }
}