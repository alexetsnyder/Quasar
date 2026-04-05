using Catcophony.scenes.systems.items;
using Godot;

namespace Catcophony.core.actions.interfaces
{
    public interface IActor
    {
        public int Id { get; }

        public Item Item { get; set; }

        public void Drink();

        public void SetDestination(Vector2 localPos);

        public void SetAction(Action action);
    }
}