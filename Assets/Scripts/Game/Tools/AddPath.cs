using App.Services;
using Game.Grid;
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
            if (selection.SelectionType != SelectionType.Vertex) return;
            if (hex == null) return;
            
            hex.ToggleVertexFeature(selection.ComponentIndex);
            
            var neighbours = HexGrid.GetNeighboursSharingVertex(selection.Coordinate, selection.ComponentIndex);
            var neighbourVertexIndices = HexGrid.GetNeighbourVertexIndices(selection.ComponentIndex);
            
            var neighbourHexA = ServiceLocator.Instance.Get<HexController>().GetHexObject(neighbours.A);
            var neighbourHexB = ServiceLocator.Instance.Get<HexController>().GetHexObject(neighbours.B);

            var active = hex.HasVertexFeature(selection.ComponentIndex);
            
            if (neighbourHexA != null && hex.Height == neighbourHexA.Height)
            {
                neighbourHexA.SetVertexFeature(active, neighbourVertexIndices.A);    
            }
            if (neighbourHexB != null && hex.Height == neighbourHexB.Height)
            {
                neighbourHexB.SetVertexFeature(active, neighbourVertexIndices.B);
            }
        }
    }
}