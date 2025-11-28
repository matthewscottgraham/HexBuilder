using App.Services;
using Game.Features;
using Game.Hexes;
using Game.Selection;

namespace Game.Tools
{
    public class AddTrees : ITool
    {
        public void Use(SelectionContext selection, HexObject hex)
        {
            if (hex == null) return;
            var feature = ServiceLocator.Instance.Get<FeatureFactory>().CreateFeature(FeatureType.Wilderness);
            hex.AddFeature(feature);
        }
    }
}