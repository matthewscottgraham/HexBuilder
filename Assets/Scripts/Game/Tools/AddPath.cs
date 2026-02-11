using Game.Grid;
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
        }
        
        public bool Use(HexObject[] hexes)
        {
            if (hexes == null || hexes.Length != 3) return false;

            var sharedVertexA = HexGrid.GetSharedVertexIndex(hexes[0].Coordinate, hexes[1].Coordinate, hexes[2].Coordinate);
            if (sharedVertexA < 0) return false;
            
            var sharedVertexB = HexGrid.GetSharedVertexIndex(hexes[1].Coordinate, hexes[2].Coordinate, hexes[0].Coordinate);
            if (sharedVertexB < 0) return false;
            
            var sharedVertexC = HexGrid.GetSharedVertexIndex(hexes[2].Coordinate, hexes[0].Coordinate, hexes[1].Coordinate);
            if (sharedVertexC < 0) return false;
            
            hexes[0].Vertices.Set(sharedVertexA, true);
            hexes[1].Vertices.Set(sharedVertexB, true);
            hexes[2].Vertices.Set(sharedVertexC, true);

            return true;
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