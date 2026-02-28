using Godot;

public partial class ToolBarControl : Control
{
    [Signal]
    public delegate void SelectPressedEventHandler();

    [Signal]
	public delegate void MinePressedEventHandler();

	[Signal]
	public delegate void BuildPressedEventHandler(int buildingType);

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
        EmitSignal(SignalName.BuildPressed, index + 1);
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
