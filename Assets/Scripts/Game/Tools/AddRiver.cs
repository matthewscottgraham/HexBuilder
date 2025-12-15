using App.Services;
using Game.Grid;
using Game.Hexes;
using Game.Selection;

namespace Game.Tools
{
    public class AddRiver : ITool
    {
        bool ITool.AllowAreaOfEffect => false;
        SelectionType ITool.SelectionType => SelectionType.Edge;
        
        public void Use(SelectionContext selection, HexObject hex)
        {
            if (selection.SelectionType != SelectionType.Edge) return;
            if (hex == null) return;

            hex.Edges.Toggle(selection.ComponentIndex);
            
            var neighbourCoordinate = HexGrid.GetNeighbourSharingEdge(selection.Coordinate, selection.ComponentIndex);
            var neighbourEdgeIndex = HexGrid.GetNeighboursSharedEdgeIndex(selection.ComponentIndex);
            var neighbourHex = ServiceLocator.Instance.Get<HexController>().GetHexObject(neighbourCoordinate);
            if (neighbourHex == null) return;

            var active = hex.Edges.Exists(selection.ComponentIndex);
            if( hex.Height == neighbourHex.Height)
            {
                neighbourHex.Edges.Set(neighbourEdgeIndex, active);     
            }
            else
            {
                // TODO: waterfall
            }
        }
    }
}