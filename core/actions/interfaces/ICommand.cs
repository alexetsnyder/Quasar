namespace Catcophony.core.actions.interfaces
{
    public interface ICommand
    {
        public void Execute(IActor actor = null);
    }
}