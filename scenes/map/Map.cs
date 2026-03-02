using Godot;
using Quasar.data;
using Quasar.data.enums;
using Quasar.math;
using System.Collections.Generic;

namespace Quasar.scenes.map
{
    public partial class Map : Node2D
    {
        #region Variables

        [Export(PropertyHint.Range, "0,200")]
        public int Rows { get; set; } = 10;

        [Export(PropertyHint.Range, "0,200")]
        public int Cols { get; set; } = 10;

        [Export]
        public Color SelectionColor { get; set; } = new Color(1.0f, 0.0f, 0.0f, 1.0f);

        [Export]
        public Color PathColor { get; set; } = new Color(1.0f, 0.0f, 1.0f, 1.0f);

        private TileMapLayer _mapLayer;

        private TileMapLayer _selectLayer;

        private ColorRect _selectionRect;

        private bool _isSelecting = false;

        private Vector2 _selectionStart;

        private SimplexNoise _heightNoise;

        private SimplexNoise _varianceNoise;

        private RandomNumberGenerator _rng = new();

        #endregion

        public override void _Ready()
        {
            _mapLayer = GetNode<TileMapLayer>("MapLayer");
            _selectLayer = GetNode<TileMapLayer>("SelectLayer");
            _selectionRect = GetNode<ColorRect>("SelectionRect");
            _heightNoise = new SimplexNoise(_rng.RandiRange(int.MinValue, int.MaxValue));
            _varianceNoise = new SimplexNoise(_rng.RandiRange(int.MinValue, int.MaxValue));

            FillMap();
        }

        public override void _Process(double delta)
        {
            if (_isSelecting)
            {
                var currentMousePos = GetGlobalMousePosition();
                var newSelectionRect = new Rect2(_selectionStart, currentMousePos - _selectionStart).Abs();
                _selectionRect.Position = newSelectionRect.Position;
                _selectionRect.Size = newSelectionRect.Size;
            }
        }

        public override void _UnhandledInput(InputEvent @event)
        {
            if (@event is InputEventMouseButton inputEventMouseButton)
            {
                if (inputEventMouseButton.ButtonIndex == MouseButton.Right)
                {
                    if (@event.IsPressed())
                    {
                        _isSelecting = true;
                        _selectionStart = GetGlobalMousePosition();
                        _selectionRect.Position = _selectionStart;
                        _selectionRect.Size = new Vector2();
                    }
                    else
                    {
                        if (_isSelecting)
                        {
                            _isSelecting = false;
                            _selectionRect.Visible = false;
                            SelectArea();
                        }
                    }
                }
            }
            else if (@event is InputEventMouseMotion && _isSelecting)
            {
                if (_selectionRect.Size.X >= 1.0f && _selectionRect.Size.Y >= 1.0f)
                {
                    _selectionRect.Visible = true;
                }
                else
                {
                    _selectionRect.Visible = false;
                }
            }
        }

        public string GetTileTypeStr(Vector2 localPos)
        {
            var cellCoord = _mapLayer.LocalToMap(localPos);
            var atlasCoord = _mapLayer.GetCellAtlasCoords(cellCoord);

            return "Not Implemented";
        }

        public string GetTileColorStr(Vector2 localPos)
        {
            var color = GetTileColor(localPos);

            if (color == ColorConstants.WHITE)
            {
                return "NONE";
            }

            return ColorConstants.GetColorStrReflection(color);
        }

        public Color GetTileColor(Vector2 localPos)
        {
            var cellCoord = _mapLayer.LocalToMap(localPos);

            if (_mapLayer.GetCellSourceId(cellCoord) != -1)
            {
                var tileData = _mapLayer.GetCellTileData(cellCoord);
                return tileData.Modulate;
            }

            return ColorConstants.WHITE;
        }

        private void FillMap()
        {
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Cols; j++)
                {
                    var coord = new Vector2I(i, j);

                    var color = ColorConstants.GREEN;
                    var atlasCoords = AtlasConstants.GetAtlasCoords(TileType.GRASSLAND);
                    var noiseVal = _heightNoise.GetNoise(j, i) * Math.SigmoidFallOffMapCircular(j, i, Cols, Rows);
                    var varyNoiseVal = _varianceNoise.GetNoise(j, i);
                    GetAtlasCoordsAndColor(noiseVal, varyNoiseVal, ref atlasCoords, ref color);

                    _mapLayer.SetCell(coord, 0, atlasCoords);

                    var tileData = _mapLayer.GetCellTileData(coord);
                    if (tileData != null)
                    {
                        tileData.Modulate = color;
                    }
                }
            }
        }

        private void SelectArea()
        {
            _selectLayer.Clear();

            var tileSize = _mapLayer.TileSet.TileSize;
            var left = _selectionRect.Position.X;
            var right = left + _selectionRect.Size.X;
            var top = _selectionRect.Position.Y;
            var bottom = top + _selectionRect.Size.Y;

            var startingCol = Mathf.FloorToInt(left / tileSize.X);
            var endingCol = Mathf.CeilToInt(right / tileSize.X);
            var startingRow = Mathf.FloorToInt(top / tileSize.Y);
            var endingRow = Mathf.CeilToInt(bottom / tileSize.Y);

            for (int i = startingRow; i < endingRow; i++)
            {
                for (int j = startingCol; j < endingCol; j++)
                {
                    var cellCoord = new Vector2I(j, i);

                    SelectCell(cellCoord, SelectionColor);
                }
            }
        }

        private void SelectCell(Vector2I cellCoord, Color modulate)
        {
            if (_mapLayer.GetCellSourceId(cellCoord) != -1)
            {
                var atlasCoord = _mapLayer.GetCellAtlasCoords(cellCoord);

                _selectLayer.SetCell(cellCoord, 0, atlasCoord);

                var tileData = _selectLayer.GetCellTileData(cellCoord);

                if (tileData != null)
                {
                    tileData.Modulate = modulate;
                }
            }
        }

        private void GetAtlasCoordsAndColor(float heightNoiseVal, float varyNoiseVal, ref Vector2I atlasCoord, ref Color cellColor)
        {
            if (heightNoiseVal < 25.0f)
            {
                atlasCoord = AtlasConstants.GetAtlasCoords(TileType.WATER);
                cellColor = AtlasConstants.GetColor(TileType.WATER);
            }
            else if (heightNoiseVal < 50.0f)
            {
                if (varyNoiseVal < 40.0f)
                {
                    atlasCoord = VaryTiles(AtlasConstants.AtlasCoords[TileType.GRASSLAND]);
                    cellColor = AtlasConstants.GetColor(TileType.GRASSLAND);
                }
                else //(varyNoiseVal < 100.0f
                {
                    atlasCoord = VaryTiles(AtlasConstants.AtlasCoords[TileType.FOREST]);
                    cellColor = AtlasConstants.GetColor(TileType.FOREST);
                }
            }
            else if (heightNoiseVal < 70.0f)
            {
                atlasCoord = VaryTiles(AtlasConstants.AtlasCoords[TileType.HILLS]);
                cellColor = AtlasConstants.GetColor(TileType.HILLS);
            }
            else //(heightNoiseVal < 100.0f
            {
                if (heightNoiseVal < 80.0f)
                {
                    atlasCoord = AtlasConstants.GetAtlasCoords(TileType.MOUNTAINS);
                    cellColor = AtlasConstants.GetColor(TileType.MOUNTAINS);
                }
                else //(heightNoiseVal < 100.0f
                {
                    atlasCoord = AtlasConstants.GetAtlasCoords(TileType.MOUNTAINS, 1);
                    cellColor = AtlasConstants.GetColor(TileType.MOUNTAINS);
                }
            }
        }

        private Vector2I VaryTiles(List<Vector2I> alts)
        {
            if (alts.Count <= 0)
            {
                return new();
            }

            return alts[_rng.RandiRange(0, alts.Count - 1)];
        }
    }
}
