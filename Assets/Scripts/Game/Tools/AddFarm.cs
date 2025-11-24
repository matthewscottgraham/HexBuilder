using App.Services;
using Game.Features;
using Game.Hexes;

namespace Game.Tools
{
    public class AddFarm : ITool
    {
        public void Use(Cell cell, HexObject hex)
        {
            if (hex == null) return;
            var feature = ServiceLocator.Instance.Get<FeatureFactory>().CreateFeature(FeatureType.Settlement);
            hex.AddFeature(FeatureType.Settlement, feature);
        }
    }
}