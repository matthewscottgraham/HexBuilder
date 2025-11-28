using App.Services;
using Game.Features;
using Game.Hexes;
using Game.Selection;

namespace Game.Tools
{
    public class AddMountain : ITool
    {
        public void Use(SelectionContext selection, HexObject hex)
        {
            if (hex == null) return;
            var feature = ServiceLocator.Instance.Get<FeatureFactory>().CreateFeature(FeatureType.Mountain);
            hex.AddFeature(feature);
        }
    }
}