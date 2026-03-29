using Catcophony.data.enums;
using Godot;
using System.Collections.Generic;

namespace Catcophony.scenes.systems.regions
{
    public partial class Region(int id, RegionType regionType, List<Vector2> localPos, Rect2 regionRect) : Resource
    {
        public int Id { get; set; } = id;

        public RegionType RegionType { get; set; } = regionType;

        public List<Vector2> LocalPos { get; set; } = localPos;

        public Rect2 RegionRect { get; set; } = regionRect;

        public bool IsPointInRegion(Vector2 localPos)
        {
            return RegionRect.HasPoint(localPos);
        }
    }
}