using App.Services;
using Game.Features;
using Game.Hexes;

namespace Game.Tools
{
    public class AddPath : ITool
    {
        public void Use(Cell cell, HexObject hex)
        {
            if (hex == null) return;
            var feature = ServiceLocator.Instance.Get<FeatureFactory>().CreateFeature(FeatureType.Path);
            hex.AddFeature(feature);
        }
    }
}