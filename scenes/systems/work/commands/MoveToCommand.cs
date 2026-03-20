using Quasar.scenes.cats;
using Quasar.scenes.common.interfaces;
using Quasar.scenes.systems.pathing;

namespace Quasar.scenes.systems.work.commands
{
    public partial class MoveToCommand(Path path) : ICommand
    {
        private readonly Path _path = path;

        public void Execute(Cat cat = null)
        {
            if (cat != null)
            {
                cat.SetPath(_path);
            }
        }
    }
}