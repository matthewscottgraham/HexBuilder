using App.Services;
using Game.Features;
using Game.Hexes;

namespace Game.Tools
{
    public class AddMountain : ITool
    {
        public void Use(Cell cell, HexObject hex)
        {
            if (hex == null) return;
            var feature = ServiceLocator.Instance.Get<FeatureFactory>().CreateFeature(FeatureType.Mountain);
            hex.AddFeature(feature);
        }
    }
}