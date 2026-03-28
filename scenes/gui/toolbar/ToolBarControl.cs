using Godot;
using Catcophony.data.enums;

namespace Catcophony.scenes.gui.toolbar
{
    public partial class ToolBarControl : Control
    {
        [Signal]
        public delegate void SelectPressedEventHandler();

        [Signal]
        public delegate void MinePressedEventHandler();

        [Signal]
        public delegate void CutPressedEventHandler();

        [Signal]
        public delegate void HaulPressedEventHandler();

        [Signal]
        public delegate void BuildPressedEventHandler(int tileType);

        [Signal]
        public delegate void FarmPressedEventHandler();

        [Signal]
        public delegate void GatherPressedEventHandler();

        [Signal]
        public delegate void FishPressedEventHandler();

        [Signal]
        public delegate void CreateAreaSelectedEventHandler(int areaType);

        [Signal]
        public delegate void CancelPressedEventHandler();

        private ItemList _buildMenu;

        private Control _areaSelectMenu;

        public override void _Ready()
        {
            _buildMenu = GetNode<ItemList>("%ToolBarBuildMenu");
            _areaSelectMenu = GetNode<Control>("%ToolBarCreateAreaMenu");
        }

        private static TileType GetTileType(int index)
        {
            switch (index)
            {
                case 0:
                    return TileType.WALL;
                case 1:
                    return TileType.THREE_CONNECT_WALL;
                case 2:
                    return TileType.CORNER_WALL;
                case 3:
                    return TileType.FOUR_CONNECT_WALL;
                case 4:
                    return TileType.STORAGE;
                default:
                    GD.Print($"Incorrect index {index} in ToolBarControl::GetTileType.");
                    return TileType.NONE;
            }
        }

        private void ShowBuildMenu()
        {
            _buildMenu.Visible = true;
        }

        private void ShowAreaSelectMenu()
        {
            _areaSelectMenu.Visible = true;
        }

        private void HideSubMenus()
        {
            _buildMenu.Visible = false;
            _areaSelectMenu.Visible = false;
        }

        private void OnSelectButtonPressed()
        {
            HideSubMenus();
            EmitSignal(SignalName.SelectPressed);
        }

        private void OnMineButtonPressed()
        {
            HideSubMenus();
            EmitSignal(SignalName.MinePressed);
        }

        private void OnCutButtonPressed()
        {
            HideSubMenus();
            EmitSignal(SignalName.CutPressed);
        }

        private void OnHaulButtonPressed()
        {
            HideSubMenus();
            EmitSignal(SignalName.HaulPressed);
        }

        private void OnBuildButtonPressed()
        {
            HideSubMenus();
            ShowBuildMenu();
        }

        private void OnBuildMenuItemSelected(int index)
        {
            HideSubMenus();
            var tileType = GetTileType(index);
            EmitSignal(SignalName.BuildPressed, (int)tileType);
        }

        private void OnFarmButtonPressed()
        {
            HideSubMenus();
            EmitSignal(SignalName.FarmPressed);
        }

        private void OnGatherButtonPressed()
        {
            HideSubMenus();
            EmitSignal(SignalName.GatherPressed);
        }

        private void OnCreateAreaButtonPressed()
        {
            HideSubMenus();
            ShowAreaSelectMenu();
        }

        private void OnHousingAreaSelected()
        {
            EmitSignal(SignalName.CreateAreaSelected, (int)AreaType.HOUSING);
        }

        private void OnPublicForumAreaSelected()
        {
            EmitSignal(SignalName.CreateAreaSelected, (int)AreaType.PUBLIC_FORUM);
        }

        private void OnStorageAreaSelected()
        {
            EmitSignal(SignalName.CreateAreaSelected, (int)AreaType.STORAGE);
        }

        private void OnFishButtonPressed()
        {
            HideSubMenus();
            EmitSignal(SignalName.FishPressed);
        }

        private void OnCancelButtonPressed()
        {
            HideSubMenus();
            EmitSignal(SignalName.CancelPressed);
        }
    }
}
