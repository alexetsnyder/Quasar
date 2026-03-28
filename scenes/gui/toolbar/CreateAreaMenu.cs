using Godot;

namespace Catcophony.scenes.gui.toolbar
{
    public partial class CreateAreaMenu : Control
    {
        [Signal]
        public delegate void PublicForumAreaSelectedEventHandler();

        [Signal]
        public delegate void HousingAreaSelectedEventHandler();

        [Signal]
        public delegate void StorageAreaSelectedEventHandler();

        private void OnPublicForumAreaButtonPressed()
        {
            EmitSignal(SignalName.PublicForumAreaSelected);
        }

        private void OnHousingAreaButtonPressed()
        {
            EmitSignal(SignalName.HousingAreaSelected);
        }

        private void OnStorageAreaButtonPressed()
        {
            EmitSignal(SignalName.StorageAreaSelected);
        }
    }
}