using App.Services;
using Game.Features;
using Game.Hexes;
using Game.Selection;

namespace Game.Tools
{
    public class AddPath : ITool
    {
        bool ITool.AllowAreaOfEffect => false;
        SelectionType ITool.SelectionType => SelectionType.Vertex;
        
        public void Use(Cell cell, HexObject hex)
        {
            if (hex == null) return;
            var feature = ServiceLocator.Instance.Get<FeatureFactory>().CreateFeature(FeatureType.Path);
            hex.AddFeature(feature);
        }
    }
}