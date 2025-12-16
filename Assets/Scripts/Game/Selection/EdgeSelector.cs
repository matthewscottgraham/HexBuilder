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
            Destroy(highlighter.GetComponent<Collider>());
            highlighter.localPosition = new Vector3(0, 0.5f, 0);
            highlighter.localRotation = Quaternion.Euler(90, 0, 0);
            highlighter.localScale = new Vector3(0.3f, 1f, 0.3f);
            highlighter.GetComponent<Renderer>().material = Resources.Load<Material>("Materials/mat_highlight");
            return highlighter;
        }
        
        protected override SelectionContext GetClampedSelection(HexObject hexObject, Vector3 cursorPosition)
        {
            var edge = hexObject.Edges.GetClosestFeatureCoordinate(cursorPosition);
            if (!edge.HasValue) return BlankSelection;
            var edgePosition = hexObject.Edges.Position(edge.Value.W);
            return new SelectionContext(SelectionType.Edge, edgePosition, hexObject.Coordinate, edge.Value.W);
        }

        protected override void SetHoverRotation(HexObject hexObject)
        {
            Highlighter.LookAt(hexObject.Face.Position);
            Highlighter.transform.rotation *= Quaternion.Euler(new Vector3(90, 0, 0));
        }
    }
}