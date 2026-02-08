using Godot;

public partial class World : Node2D
{
    [Export(PropertyHint.Range, "0,200")]
    public int Rows { get; set; } = 10;

    [Export(PropertyHint.Range, "0,200")]
    public int Cols { get; set; } = 10;

    private TileMapLayer _mapLayer;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        _mapLayer = GetNode<TileMapLayer>("MapLayer");

        FillMap();
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
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

                GD.Print($"{_mapLayer.GetCellSourceId(new Vector2I(i, j))}");
            }
        }
    }
}
