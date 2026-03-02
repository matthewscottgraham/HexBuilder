using Game.Hexes;
using Game.Hexes.Features;
using UnityEngine;

namespace Game.Tools
{
    public class Eraser : Tool
    {
        public Eraser()
        {
            FeatureType = FeatureType.None;
            Icon = Resources.Load<Sprite>("Sprites/subtract");
        }

        public override bool Use(HexObject hex, ToolMode toolMode)
        {
            var exists = hex.Face.Exists();
            hex.Face.Set(0, false);
            for (var i = 0; i < 6; i++)
            {
                if (hex.Edges.Exists(i) || hex.Vertices.Exists(i)) exists = true;
                hex.Edges.Set(i, false);
                hex.Vertices.Set(i, false);
            }
            return exists;
        }
    }
}