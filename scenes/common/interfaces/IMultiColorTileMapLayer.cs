using Godot;
using Godot.Collections;
using System;

namespace Quasar.scenes.common.interfaces
{
    public interface IMultiColorTileMapLayer
    {
        public Vector2I TileSize { get; set; }

        public void SetCell(Vector2I coords, Vector2I? atlasCoords = null, Color? color = null);

        public Vector2I LocalToMap(Vector2 localPos);

        public Vector2 MapToLocal(Vector2I coords);

        public int GetCellSourceId(Vector2I coords);

        public Array<Vector2I> GetUsedCellsById(int sourceId = -1, Vector2I? atlasCoords = null, int alternativeTile = -1);
    }
}

