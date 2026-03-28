using Catcophony.data;
using Catcophony.data.enums;
using Catcophony.scenes.common.interfaces;
using Catcophony.scenes.systems.selection;
using Godot;

namespace Catcophony.scenes.systems.regions
{
    public partial class RegionSystem : Node2D
    {
        public RegionType CurrentRegionType { get; set; }

        private IMultiColorTileMapLayer _regionTileMapLayer;

        public override void _Ready()
        {
            _regionTileMapLayer = GetNode<IMultiColorTileMapLayer>("%RegionTileMapLayer");
        }

        public void CreateRegion(Selection selection)
        {
            if (selection.SelectionRect.Size.X <= 1 || 
                selection.SelectionRect.Size.Y <= 1)
            {
                return;
            }

            for (int i = selection.SelectionRect.Position.X; i < selection.SelectionRect.End.X; i++)
            {
                for (int j = selection.SelectionRect.Position.Y; j < selection.SelectionRect.End.Y; j++)
                {
                    var coords = new Vector2I(j, i);
                    var atlasCoords = GetAtlasCoords(i, j, selection.SelectionRect);
                    var color = GetColor();

                    if (atlasCoords != null)
                    {
                        SetCell(coords, atlasCoords, color);
                    }
                }
            }
        }

        private static Vector2I? GetAtlasCoords(int i, int j, Rect2I selection)
        {
            Vector2I? atlasCoord = null;

            var startingRow = selection.Position.X;
            var startingCol = selection.Position.Y;
            var endingRow = selection.End.X;
            var endingCol = selection.End.Y;
            
            if (i == startingRow && j == startingCol)
            {
                atlasCoord = AtlasConstants.GetAtlasCoords(TileType.REGION, (int)RegionIndex.LEFT_TOP);
            }
            else if (i == startingRow && j == endingCol - 1)
            {
                atlasCoord = AtlasConstants.GetAtlasCoords(TileType.REGION, (int)RegionIndex.RIGHT_TOP);
            }
            else if (i == endingRow - 1 && j == startingCol)
            {
                atlasCoord = AtlasConstants.GetAtlasCoords(TileType.REGION, (int)RegionIndex.LEFT_BOTTOM);
            }
            else if (i == endingRow - 1 && j == endingCol - 1)
            {
                atlasCoord = AtlasConstants.GetAtlasCoords(TileType.REGION, (int)RegionIndex.RIGHT_BOTTOM);
            }
            else if (i == startingRow)
            {
                atlasCoord = AtlasConstants.GetAtlasCoords(TileType.REGION, (int)RegionIndex.TOP);
            }
            else if (i == endingRow - 1)
            {
                atlasCoord = AtlasConstants.GetAtlasCoords(TileType.REGION, (int)RegionIndex.BOTTOM);
            }
            else if (j == startingCol)
            {
                atlasCoord = AtlasConstants.GetAtlasCoords(TileType.REGION, (int)RegionIndex.LEFT);
            }
            else if (j == endingCol - 1)
            {
                atlasCoord = AtlasConstants.GetAtlasCoords(TileType.REGION, (int)RegionIndex.RIGHT);
            }

            return atlasCoord;
        }

        private Color GetColor()
        {
            switch (CurrentRegionType)
            {
                case RegionType.PUBLIC_FORUM:
                case RegionType.HOUSING:
                case RegionType.STORAGE:
                    return AtlasConstants.GetColor(TileType.REGION);
                default:
                    GD.Print($"Incorrect RegionType {CurrentRegionType} in RegionSystem::GetColor.");
                    return ColorConstants.WARNING_RED;
            }
        }

        private void SetCell(Vector2I coords, Vector2I? atlasCoords, Color? color)
        {
            _regionTileMapLayer.SetCell(coords, atlasCoords, color);
        }
    }
}