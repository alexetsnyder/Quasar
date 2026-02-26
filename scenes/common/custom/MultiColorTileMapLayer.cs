using Godot;
using Quasar.data;
using Quasar.scenes.common.interfaces;
using System.Collections.Generic;

namespace Quasar.scenes.custom
{
    [GlobalClass]
    public partial class MultiColorTileMapLayer : TileMapLayer, IMultiColorTileMapLayer
    {
        [Export]
        public string TexturePath { get; set; }

        [Export]
        public Vector2I TileSize { get; set; }

        private Texture2D _atlasTexture;

        private readonly Dictionary<Color, int> _tileSetSources = [];

        private int _nextSourceId = 0;

        private readonly int _alternateTile = 0;

        public override void _Ready()
        {
            _atlasTexture = ResourceLoader.Load<Texture2D>(TexturePath);
        }

        public void SetCell(Vector2I coords, Vector2I? atlasCoords = null, Color? color = null)
        {
            if (atlasCoords == null || color == null)
            {
                base.SetCell(coords);
                return;
            }

            if (_tileSetSources.TryAdd(color.Value, _nextSourceId))
            {
                var atlasSource = CreateTileSetAtlasSource(color.Value);
                TileSet.AddSource(atlasSource, _nextSourceId++);
            }

            SetCell(coords, _tileSetSources[color.Value], atlasCoords, _alternateTile);
        }

        private TileSetAtlasSource CreateTileSetAtlasSource(Color color)
        {
            TileSetAtlasSource atlasSource = new()
            {
                Texture = _atlasTexture,
                TextureRegionSize = TileSize
            };

            CreateTiles(atlasSource, color);

            return atlasSource;
        }

        private void CreateTiles(TileSetAtlasSource atlasSource, Color color)
        {
            var atlasSize = _atlasTexture.GetSize();

            foreach (var atlasCoords in GetAllAtlasCoords(atlasSize))
            {
                atlasSource.CreateTile(atlasCoords, new(1, 1));
                var tileData = atlasSource.GetTileData(atlasCoords, _alternateTile);
                if (tileData != null)
                {
                    tileData.Modulate = color;
                }
            }
        }

        private List<Vector2I> GetAllAtlasCoords(Vector2 atlasSize)
        {
            List<Vector2I> atlasCoords = [];

            for (int i = 0; i < atlasSize.X / TileSize.X; i++)
            {
                for (int j = 0; j < atlasSize.Y / TileSize.Y; j++)
                {
                    atlasCoords.Add(new(i, j));
                }
            }

            return atlasCoords;
        }
    }
}
