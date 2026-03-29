using Catcophony.data.enums;
using Godot;
using System.Collections.Generic;

namespace Catcophony.scenes.systems.regions
{
    public partial class Region(int id, RegionType regionType, List<Vector2I> coords, Rect2I regionRect) : Resource
    {
        public int Id { get; set; } = id;

        public RegionType RegionType { get; set; } = regionType;

        public List<Vector2I> Coords { get; set; } = coords;

        public Rect2I RegionRect { get; set; } = regionRect;

        public bool IsPointInRegion(Vector2I coords)
        {
            return RegionRect.HasPoint(coords);
        }
    }
}