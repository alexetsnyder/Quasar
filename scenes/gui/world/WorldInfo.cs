using Catcophony.scenes.gui.items;
using Catcophony.scenes.systems.items;
using Catcophony.system;
using Godot;

namespace Catcophony.scenes.gui.world
{
    public partial class WorldInfo : Control
    {
        private Label _coordsLabelValue;

        private Label _localPosLabelValue;

        private Label _workLabelValue;

        private Label _tileTypeLabelValue;

        private Label _regionTypeLableValue;

        private Label _itemsLabelValue;

        private GridContainer _itemsGridContainer;

        public override void _Ready()
        {
            _coordsLabelValue = GetNode<Label>("%CoordsLabelValue");
            _localPosLabelValue = GetNode<Label>("%LocalPosLabelValue");
            _workLabelValue = GetNode<Label>("%WorkLabelValue");
            _tileTypeLabelValue = GetNode<Label>("%TileTypeLabelValue");
            _regionTypeLableValue = GetNode<Label>("%RegionTypeLabelValue");
            _itemsLabelValue = GetNode<Label>("%ItemsLabelValue");
            _itemsGridContainer = GetNode<GridContainer>("%ItemsGridContainer");
        }

        public void SetInfo(WorldInfoData data)
        {
            _coordsLabelValue.Text = data.Coords.ToString();
            _localPosLabelValue.Text = data.LocalPos.ToString();
            _workLabelValue.Text = data.WorkType.ToString();
            _tileTypeLabelValue.Text = data.TileType.ToString();
            _regionTypeLableValue.Text = data.RegionType.ToString();

            _itemsLabelValue.Visible = data.Items.Count == 0;

            ClearItems();

            foreach (var item in data.Items)
            {
                Add(item);
            }
        }

        private void Add(Item item)
        {
            var slot = GlobalSystem.Instance.InstantiateScene<InventorySlot>("res://scenes/gui/items/inventory_slot.tscn");
            if (slot != null)
            {
                _itemsGridContainer.AddChild(slot);

                slot.Add(item);
            }
        }

        public void ClearItems()
        {
            foreach (var child in _itemsGridContainer.GetChildren())
            {
                if (child is InventorySlot slot)
                {
                    _itemsGridContainer.RemoveChild(slot);
                    slot.QueueFree();
                }
            }
        }

        private void OnCloseButtonPressed()
        {
            ClearItems();
            this.Visible = false;
        }
    }
}
