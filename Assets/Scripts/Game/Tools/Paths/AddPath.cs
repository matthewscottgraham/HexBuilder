using App.Services;
using Game.Features;
using Game.Hexes;
using Game.Selection;

namespace Game.Tools.Paths
{
    public class AddPath : ITool
    {
        bool ITool.AllowAreaOfEffect => false;
        SelectionType ITool.SelectionType => SelectionType.Vertex;
        
        public void Use(SelectionContext selection, HexObject hex)
        {
            if (hex == null) return;
            ServiceLocator.Instance.Get<PathController>().TogglePathOnVertex(selection.Coordinate, selection.ComponentIndex);
        }
    }
}