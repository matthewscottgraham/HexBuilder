using App.Services;
using Game.Hexes;
using Game.Hexes.Features;
using Game.Selection;
using UnityEngine;

namespace Game.Tools
{
    public class AddTrees : ITool
    {
        Sprite ITool.Icon => Resources.Load<Sprite>("Sprites/wilderness");
        bool ITool.UseRadius => false;
        
        public void Use(SelectionContext selection, HexObject hex)
        {
            if (hex == null) return;
            hex.Face.Add(FeatureType.Wilderness);
        }
    }
}