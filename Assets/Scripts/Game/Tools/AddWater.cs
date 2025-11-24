using App.Services;
using Game.Features;
using Game.Hexes;

namespace Game.Tools
{
    public class AddWater : ITool
    {
        bool ITool.AllowAreaOfEffect => false;
        
        public void Use(Cell cell, HexObject hex)
        {
            if (hex == null) return;
            var feature = ServiceLocator.Instance.Get<FeatureFactory>().CreateFeature(FeatureType.Water);
            hex.AddFeature(FeatureType.Water, feature);
        }
    }
}