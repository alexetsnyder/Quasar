using Godot;
using Quasar.data;
using System;
using System.Collections.Generic;

namespace Quasar.scenes.demos
{
    public partial class MultiColorTileMapDemo : TileMapLayer
    {
        private Texture2D _atlasTexture;

        private readonly Dictionary<Color, int> _tileSetSources = [];

        private int _nextSourceId = 0;

        private Color _currentColor = ColorConstants.WHITE;

        private readonly RandomNumberGenerator _rng = new();

        private readonly int _alternateTile = 0;


        public override void _Ready()
        {
            _atlasTexture = ResourceLoader.Load<Texture2D>("res://assets/textures/Haowan_Curses_1440x450.png");

            SetTileSetSource();
        }

        public override void _UnhandledInput(InputEvent @event)
        {
            if (@event.IsActionPressed("Quit"))
            {
                GetTree().Quit();
            }
            if (@event is InputEventMouseButton inputEventMouseButton)
            {
                if (inputEventMouseButton.ButtonIndex == MouseButton.Left)
                {
                    if (@event.IsPressed())
                    {
                        var localPos = GetGlobalMousePosition();
                        var atlasCoord = RandomChoice<Vector2I>(GetAllAtlasCoords(new(288, 288)));
                        SetCell(LocalToMap(localPos), _tileSetSources[_currentColor], atlasCoord);
                    }
                }
            }
        }

        private void SetTileSetSource()
        {
            if (_tileSetSources.TryAdd(_currentColor, _nextSourceId))
            {
                TileSetAtlasSource atlasSource = new();
                atlasSource.Texture = _atlasTexture;
                atlasSource.TextureRegionSize = new Vector2I(18, 18);

                CreateAllTiles(atlasSource);

                TileSet.AddSource(atlasSource, _nextSourceId++);

            }
        }

        private void CreateAllTiles(TileSetAtlasSource atlasSource)
        {
            var atlasSize = _atlasTexture.GetSize();

            foreach (var atlasCoords in GetAllAtlasCoords(atlasSize))
            {
                atlasSource.CreateTile(atlasCoords, new(1, 1));
                var tileData = atlasSource.GetTileData(atlasCoords, _alternateTile);
                if (tileData != null)
                {
                    tileData.Modulate = _currentColor;
                }
            }
        }

        private List<Vector2I> GetAllAtlasCoords(Vector2 atlasSize)
        {
            List<Vector2I> atlasCoords = [];

            for (int i = 0; i < atlasSize.X / 18; i++)
            {
                for (int j = 0; j < atlasSize.Y / 18; j++)
                {
                    atlasCoords.Add(new(i, j));
                }
            }

            return atlasCoords;
        }

        private T RandomChoice<T>(List<T> alts)
        {
            if (alts.Count <= 0)
            {
                return default;
            }

            return alts[_rng.RandiRange(0, alts.Count - 1)];
        }

        private void OnLineEditTextSubmitted(string newText)
        {
            Color? newColor = null;
            try
            {
                newColor = new(newText);
            }
            catch (ArgumentOutOfRangeException)
            {
                GD.Print($"Couldn't parse {newText} into Color");
            }

            if (newColor.HasValue)
            {
                _currentColor = newColor.Value;
                SetTileSetSource();
            }
        }
    }
}
