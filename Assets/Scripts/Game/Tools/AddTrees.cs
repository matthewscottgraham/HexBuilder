using App.Services;
using Game.Hexes;
using Game.Hexes.Features;
using Game.Selection;

namespace Game.Tools
{
    public class AddTrees : ITool
    {
        bool ITool.AllowAreaOfEffect => false;
        
        public void Use(SelectionContext selection, HexObject hex)
        {
            if (hex == null) return;
            hex.Face.Add(FeatureType.Wilderness);
        }
    }
}