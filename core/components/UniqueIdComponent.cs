namespace Catcophony.core.components
{
    public partial class UniqueIdComponent
    {
        private int _nextId = 0;

        public int NextId {  get { return _nextId++; } }
    }
}