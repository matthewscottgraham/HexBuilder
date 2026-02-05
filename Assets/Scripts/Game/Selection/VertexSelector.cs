using App.Utils;
using Game.Hexes;
using UnityEngine;

namespace Game.Selection
{
    public class VertexSelector : Selector
    {
        public override SelectionType SelectionType => SelectionType.Vertex;
        
        protected override SelectionContext GetClampedSelection(HexObject hexObject, Vector3 cursorPosition)
        {
            var vertex = hexObject.Vertices.GetClosestFeatureCoordinate(cursorPosition);
            if (!vertex.HasValue) return BlankSelection;
            var vertexPosition = hexObject.Vertices.Position(vertex.Value.W);
            return new SelectionContext(SelectionType.Vertex, vertexPosition, hexObject.Coordinate, vertex.Value.W);
        }
    }
}