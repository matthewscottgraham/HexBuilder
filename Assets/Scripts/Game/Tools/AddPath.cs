using Game.Hexes;
using Game.Selection;
using UnityEngine;

namespace Game.Tools
{
    public class AddPath : ITool
    {
        Sprite ITool.Icon => Resources.Load<Sprite>("Sprites/path");
        bool ITool.UseRadius => false;
        SelectionType ITool.SelectionType => SelectionType.Vertex;
        
        public bool Use(HexObject hex)
        {
            return false;
            // if (selection.SelectionType != SelectionType.Vertex) return;
            // if (hex == null) return;
            //
            // hex.Vertices.Toggle(selection.ComponentIndex);
            //
            // var neighbours = HexGrid.GetNeighboursSharingVertex(selection.Coordinate, selection.ComponentIndex);
            // var neighbourVertexIndices = HexGrid.GetNeighbourVertexIndices(selection.ComponentIndex);
            //
            // var neighbourHexA = ServiceLocator.Instance.Get<HexController>().GetHexObject(neighbours.A);
            // var neighbourHexB = ServiceLocator.Instance.Get<HexController>().GetHexObject(neighbours.B);
            //
            // var active = hex.Vertices.Exists(selection.ComponentIndex);
            //
            // if (neighbourHexA != null && hex.Height == neighbourHexA.Height)
            // {
            //     neighbourHexA.Vertices.Set(neighbourVertexIndices.A, active);    
            // }
            // if (neighbourHexB != null && hex.Height == neighbourHexB.Height)
            // {
            //     neighbourHexB.Vertices.Set(neighbourVertexIndices.B, active);
            // }
        }
    }
}