using Game.Hexes;
using UnityEngine;

namespace Game.Selection
{
    public class VertexSelector : Selector
    {
        public override SelectionType SelectionType => SelectionType.Vertex;
        
        protected override Transform CreateHighlighter()
        {
            var highlighter = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
            highlighter.SetParent(transform);
            highlighter.localPosition = new Vector3(0, 0.125f, 0);
            highlighter.localScale = new Vector3(0.25f, 0.25f, 0.25f);
            highlighter.GetComponent<Renderer>().material = Resources.Load<Material>("Materials/mat_highlight");
            return highlighter;
        }
        
        protected override SelectionContext GetClampedSelection(HexObject hexObject, Vector3 cursorPosition)
        {
            var vertex = hexObject.GetVertexCloseToPosition(cursorPosition);
            if (!vertex.HasValue) return new SelectionContext(SelectionType.None, null, null, null);
            var vertexPosition = hexObject.GetVertexPosition(vertex.Value.W);
            return new SelectionContext(SelectionType.Vertex, vertexPosition, hexObject.Coordinate, vertex.Value.W);
        }
    }
}