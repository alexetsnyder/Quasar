using Godot;
using Quasar.data;
using Quasar.scenes.systems.items;

public partial class InventorySlot : Control
{
	[Export]
	public Texture2D TextureAtlas {  get; set; }

	private TextureRect _inventoryIcon;

	private Label _inventoryLabel;

    public override void _Ready()
    {
		_inventoryIcon = GetNode<TextureRect>("%InventoryIcon");
		_inventoryLabel = GetNode<Label>("%InventoryLabel");
    }

	public void Add(Item item)
	{
		_inventoryLabel.Text = item.TileType.ToString();

		var atlasCoords = AtlasConstants.GetAtlasCoords(item.TileType);
		var color = AtlasConstants.GetColor(item.TileType);

		AtlasTexture atlas = new();
		atlas.Atlas = TextureAtlas;
		atlas.Region = new(atlasCoords.X * 18.0f, atlasCoords.Y * 18.0f, 18.0f, 18.0f);

		_inventoryIcon.Texture = atlas;

		_inventoryIcon.Modulate = color;
	}
}
