using Godot;

public partial class World : Node2D
{
    [Export(PropertyHint.Range, "0,200")]
    public int Rows { get; set; } = 10;

    [Export(PropertyHint.Range, "0,200")]
    public int Cols { get; set; } = 10;

    private TileMapLayer _mapLayer;

    private TileMapLayer _selectLayer;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        _mapLayer = GetNode<TileMapLayer>("MapLayer");
        _selectLayer = GetNode<TileMapLayer>("SelectLayer");

        FillMap();
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("LeftMouseClick"))
            //@event is InputEventMouseButton inputEventMouseButton &&
            //inputEventMouseButton.ButtonIndex == MouseButton.Left)
        {
            var mousePos = GetLocalMousePosition();
            var cellCoord = _selectLayer.LocalToMap(mousePos);
            var atlasCoord = new Vector2I(7, 15);

            _selectLayer.SetCell(cellCoord, 0, atlasCoord);

            var tileData = _selectLayer.GetCellTileData(cellCoord);

            if (tileData != null)
            {
                tileData.Modulate = new Color(1.0f, 0.0f, 0.0f, 1.0f);
            }
        }

    }

    private void FillMap()
    {
        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < Cols; j++)
            {
                var coord = new Vector2I(i, j);
                var atlasCoord = new Vector2I(7, 15);

                _mapLayer.SetCell(coord, 0, atlasCoord);

                var tileData = _mapLayer.GetCellTileData(coord);
                if (tileData != null)
                {
                    tileData.Modulate = new Color(0.0f, 1.0f, 0.0f, 1.0f);
                }
            }
        }
    }
}
