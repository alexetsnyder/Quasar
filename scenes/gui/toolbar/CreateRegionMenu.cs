using Godot;

namespace Catcophony.scenes.gui.toolbar
{
    public partial class CreateRegionMenu : Control
    {
        [Signal]
        public delegate void PublicForumRegionSelectedEventHandler();

        [Signal]
        public delegate void HousingRegionSelectedEventHandler();

        [Signal]
        public delegate void StorageRegionSelectedEventHandler();

        private void OnPublicForumRegionButtonPressed()
        {
            EmitSignal(SignalName.PublicForumRegionSelected);
        }

        private void OnHousingRegionButtonPressed()
        {
            EmitSignal(SignalName.HousingRegionSelected);
        }

        private void OnStorageRegionButtonPressed()
        {
            EmitSignal(SignalName.StorageRegionSelected);
        }
    }
}