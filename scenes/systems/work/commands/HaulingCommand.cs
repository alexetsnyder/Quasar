using Godot;
using Quasar.scenes.cats;
using Quasar.scenes.common.interfaces;

namespace Quasar.scenes.systems.work.commands
{
    public partial class HaulingCommand(IWorld world, IItemSystem itemSystem, ISelectionSystem selectionSystem, Vector2 localPos) : ICommand
    {
        private readonly IWorld _world = world;

        private readonly IItemSystem _itemSystem = itemSystem;

        private readonly ISelectionSystem _selectionSystem = selectionSystem;

        private readonly Vector2 _localPos = localPos;

        public void Execute(Cat cat = null)
        {
            if (cat == null)
            {
                return;
            }

            if (cat.Item == null)
            {
                var items = _itemSystem.GetItems(_localPos);
                if (items.Count > 0)
                {
                    cat.Item = items[0];
                    _itemSystem.PickUpItem(cat.Id, cat.Item);

                    if (items.Count <= 0)
                    {
                        _selectionSystem.Deselect(_localPos);
                    }
                }
            }
            else
            {
                var storageId = _world.GetWorldCellId(_localPos);
                if (storageId == -1)
                {
                    _itemSystem.PlaceItem(cat.Id, cat.Item, _localPos);
                }
                else
                {
                    _itemSystem.StoreItem(cat.Id, storageId, cat.Item, _localPos);
                }
                
                cat.Item = null;
            } 
        }
    }
}