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
            var cell = HexGrid.GetClosestCellToPosition(worldPosition);
            var (position, edgeIndex) = HexGrid.ClampWorldPositionToEdge(worldPosition);
            position.y = HexController.GetCellHeight(cell);
            CellHighlighter.localRotation = Quaternion.Euler(90, (60 * edgeIndex) + 30, 0);
            return new SelectionContext(SelectionType.Face, position, cell, edgeIndex, null);
        }
    }
}