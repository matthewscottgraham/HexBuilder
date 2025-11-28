using App.Services;
using Game.Features;
using Game.Hexes;
using Game.Selection;

namespace Game.Tools
{
    public class AddWater : ITool
    {
        bool ITool.AllowAreaOfEffect => false;
        SelectionType ITool.SelectionType => SelectionType.Edge;
        
        public void Use(SelectionContext selection, HexObject hex)
        {
            if (hex == null) return;
            var feature = ServiceLocator.Instance.Get<FeatureFactory>().CreateFeature(FeatureType.Water);
            hex.AddFeature(feature);
        }
    }
}