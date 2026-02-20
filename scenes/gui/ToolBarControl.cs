using Godot;

public partial class ToolBarControl : MarginContainer
{
	[Signal]
	public delegate void DigPressedEventHandler();

	[Signal]
	public delegate void SelectPressedEventHandler();

	[Signal]
	public delegate void CancelPressedEventHandler();

	private Button _digButton;

	private Button _selectButton;

	public override void _Ready()
	{
		_selectButton = GetNode<Button>("ToolBarContainer/SelectButton");
		_digButton = GetNode<Button>("ToolBarContainer/DigButton");
	}

	public override void _Process(double delta)
	{
	}

    private void OnSelectButtonPressed()
    {
        EmitSignal(SignalName.SelectPressed);
    }

    private void OnDigButtonPressed()
	{
        EmitSignal(SignalName.DigPressed);
	}

	public void OnCancelButtonPressed()
	{
		EmitSignal(SignalName.CancelPressed);
	}
}
