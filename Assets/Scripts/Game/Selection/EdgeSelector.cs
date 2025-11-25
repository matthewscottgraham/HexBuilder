using App.Services;
using Game.Grid;
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
            highlighter.GetComponent<Renderer>().material = Resources.Load<Material>("Materials/mat_highlight");
            return highlighter;
        }
        
        protected override SelectionContext GetClampedSelection(Vector3 worldPosition)
        {
            var cell = ServiceLocator.Instance.Get<HexGrid>().GetClosestCellToPosition(worldPosition);
            var (position, edgeIndex) = ServiceLocator.Instance.Get<HexGrid>().GetClosestEdgePosition(worldPosition);
            position.y = ServiceLocator.Instance.Get<HexController>().GetCellHeight(cell);
            return new SelectionContext(SelectionType.Face, position, cell, edgeIndex, null);
        }
    }
}