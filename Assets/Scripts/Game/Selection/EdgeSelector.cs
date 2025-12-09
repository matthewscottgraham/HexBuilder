using Game.Hexes;
using UnityEngine;

namespace Game.Selection
{
    public class EdgeSelector : Selector
    {
        public override SelectionType SelectionType => SelectionType.Edge;
        
        protected override Transform CreateHighlighter()
        {
            var highlighter = GameObject.CreatePrimitive(PrimitiveType.Cylinder).transform;
            highlighter.SetParent(transform);
            highlighter.localPosition = new Vector3(0, 0.5f, 0);
            highlighter.localRotation = Quaternion.Euler(90, 0, 0);
            highlighter.localScale = new Vector3(0.2f, 1f, 0.2f);
            highlighter.GetComponent<Renderer>().material = Resources.Load<Material>("Materials/mat_highlight");
            return highlighter;
        }
        
        protected override SelectionContext GetClampedSelection(HexObject hexObject, Vector3 cursorPosition)
        {
            var edge = hexObject.GetVertexCloseToPosition(cursorPosition);
            if (!edge.HasValue) return new SelectionContext(SelectionType.None, null, null, null);
            var edgePosition = hexObject.GetEdgePosition(edge.Value.W);
            return new SelectionContext(SelectionType.Edge, edgePosition, hexObject.Coordinate, edge.Value.W);
        }
    }
}