using Game.Hexes;
using UnityEngine;

namespace Game.Tools
{
    public class LevelTerrainTool : Tool
    {
        public int Level { get; set; }
        
        public override void SetMode(ToolMode mode)
        {
            CurrentMode = ToolMode.Add;
        }

        public override ToolMode[] GetModes()
        {
            return null;
        }
        
        public LevelTerrainTool()
        {
            RadiusIncrement = 1;
            Icon = Resources.Load<Sprite>("Sprites/level");
        }
        
        public override bool Use(HexObject hex)
        {
            return hex.SetHeight(Level);
        }
    }
}