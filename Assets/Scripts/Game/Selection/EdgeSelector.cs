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
        
        protected override SelectionContext GetClampedSelection(Vector3 worldPosition)
        {
            var edge = HexGrid.GetClosestEdgeCoordinateToPosition(worldPosition);
            var position = HexGrid.GetEdgePosition(edge);
            position.y = HexController.GetHexHeight(edge.GetGridCoordinate);
            CellHighlighter.localRotation = Quaternion.Euler(90, (60 * edge.Z) + 30, 0);
            return new SelectionContext(SelectionType.Face, position, edge.GetGridCoordinate, edge.Z);
        }
    }
}