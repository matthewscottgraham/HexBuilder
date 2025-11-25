using App.Services;
using Game.Grid;
using Game.Hexes;
using Game.Tools;
using UnityEngine;

namespace Game.Selection
{
    public class FaceSelector : Selector
    {
        protected override Transform CreateHighlighter()
        {
            var highlighter = GameObject.CreatePrimitive(PrimitiveType.Sphere).transform;
            highlighter.SetParent(transform);
            highlighter.localPosition = new Vector3(0, 0.5f, 0);
            highlighter.GetComponent<Renderer>().material = Resources.Load<Material>("Materials/mat_highlight");
            return highlighter;
        }
        
        protected override SelectionContext GetClampedSelection(Vector3 position)
        {
            var cell = ServiceLocator.Instance.Get<HexGrid>().GetClosestCellToPosition(position);
            var center = ServiceLocator.Instance.Get<HexGrid>().GetClosestFacePosition(position);
            center.y = ServiceLocator.Instance.Get<HexController>().GetCellHeight(cell);
            return new SelectionContext(SelectionType.Face, center, cell, null, null);
        }
    }
}