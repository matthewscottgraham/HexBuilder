using Game.Grid;
using Game.Hexes;
using Game.Selection;
using UnityEngine;

namespace Game.Tools
{
    public class PathTool : Tool
    {
        public PathTool()
        {
            UseRadius = false;
            Icon = Resources.Load<Sprite>("Sprites/path");
            SelectionType = SelectionType.Vertex;
        }
        
        public override bool Use(HexObject[] hexes, ToolMode toolMode = ToolMode.Add)
        {
            if (hexes == null || hexes.Length != 3) return false;

            var sharedVertexA = HexGrid.GetSharedVertexIndex(hexes[0].Coordinate, hexes[1].Coordinate, hexes[2].Coordinate);
            if (sharedVertexA < 0) return false;
            
            var sharedVertexB = HexGrid.GetSharedVertexIndex(hexes[1].Coordinate, hexes[2].Coordinate, hexes[0].Coordinate);
            if (sharedVertexB < 0) return false;
            
            var sharedVertexC = HexGrid.GetSharedVertexIndex(hexes[2].Coordinate, hexes[0].Coordinate, hexes[1].Coordinate);
            if (sharedVertexC < 0) return false;

            var pathPresent = hexes[0].Vertices.Exists(sharedVertexA); // if present on any hex, it is present for all
            
            hexes[0].Vertices.Set(sharedVertexA, !pathPresent);
            hexes[1].Vertices.Set(sharedVertexB, !pathPresent);
            hexes[2].Vertices.Set(sharedVertexC, !pathPresent);

            return true;
        }
    }
}