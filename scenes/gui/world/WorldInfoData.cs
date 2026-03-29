using Catcophony.data.enums;
using Catcophony.scenes.systems.items;
using Godot;
using System.Collections.Generic;

namespace Catcophony.scenes.gui.world
{
    public partial class WorldInfoData
    {
        public Vector2I Coords { get; set; }

        public Vector2 LocalPos { get; set; }

        public WorkType WorkType { get; set; }

        public TileType TileType { get; set; }

        public RegionType RegionType { get; set; }

        public List<Item> Items { get; set; }
    }
}