using Catcophony.core.actions.interfaces;
using Catcophony.scenes.common.interfaces;
using Godot;

namespace Catcophony.core.actions.commands
{
    public partial class HaulingCommand(IWorld world, IItemSystem itemSystem, ISelectionSystem selectionSystem, Vector2 localPos) : ICommand
    {
        private readonly IWorld _world = world;

        private readonly IItemSystem _itemSystem = itemSystem;

        private readonly ISelectionSystem _selectionSystem = selectionSystem;

        private readonly Vector2 _localPos = localPos;

        public void Execute(IActor actor = null)
        {
            if (actor == null)
            {
                return;
            }

            if (actor.Item == null)
            {
                var items = _itemSystem.GetItems(_localPos);
                if (items.Count > 0)
                {
                    actor.Item = items[0];
                    _itemSystem.PickUpItem(actor.Id, actor.Item);

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
                    _itemSystem.PlaceItem(actor.Id, actor.Item, _localPos);
                }
                else
                {
                    _itemSystem.StoreItem(actor.Id, storageId, actor.Item, _localPos);
                }
                
                actor.Item = null;
            } 
        }
    }
}