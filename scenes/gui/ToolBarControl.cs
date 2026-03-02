using Godot;
using Quasar.data.enums;

public partial class ToolBarControl : Control
{
    [Signal]
    public delegate void SelectPressedEventHandler();

    [Signal]
	public delegate void MinePressedEventHandler();

	[Signal]
	public delegate void BuildPressedEventHandler(int tileType);

	[Signal]
	public delegate void FarmPressedEventHandler();

	[Signal]
	public delegate void FishPressedEventHandler();

    [Signal]
    public delegate void CancelPressedEventHandler();

    private ItemList _buildMenu;

	public override void _Ready()
	{
		_buildMenu = GetNode<ItemList>("BuildMenu");
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
                GD.Print($"");
                return TileType.NONE;
        }
    }

    private void OnSelectButtonPressed()
    {
        _buildMenu.Visible = false;
        EmitSignal(SignalName.SelectPressed);
    }

    private void OnMineButtonPressed()
	{
        _buildMenu.Visible = false;
        EmitSignal(SignalName.MinePressed);
	}

	private void OnBuildButtonPressed()
	{
		_buildMenu.Visible = true;
	}

    private void OnBuildMenuItemSelected(int index)
    {
        _buildMenu.Visible = false;
        var tileType = GetTileType(index);
        EmitSignal(SignalName.BuildPressed, (int)tileType);
    }

    private void OnFarmButtonPressed()
	{
        _buildMenu.Visible = false;
        EmitSignal(SignalName.FarmPressed);
	}

	private void OnFishButtonPressed()
	{
        _buildMenu.Visible = false;
        EmitSignal(SignalName.FishPressed);
	}

    private void OnCancelButtonPressed()
    {
        _buildMenu.Visible = false;
        EmitSignal(SignalName.CancelPressed);
    }
}
