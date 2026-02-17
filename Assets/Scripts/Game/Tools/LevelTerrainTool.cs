using Game.Hexes;
using UnityEngine;

namespace Game.Tools
{
    public class LevelTerrainTool : Tool
    {
        public int Level { get; set; }

        public LevelTerrainTool()
        {
            RadiusIncrement = 1;
            UseMode = false;
            Icon = Resources.Load<Sprite>("Sprites/level");
        }
        
        public override bool Use(HexObject hex, ToolMode mode = ToolMode.Add)
        {
            return hex.SetHeight(Level);
        }
    }
}