using App.Services;
using Game.Grid;
using Game.Hexes;
using Game.Tools;
using UnityEngine;

namespace Game.Selection
{
    public class FaceSelector : Selector
    {
        private ToolController _toolController;
        public override SelectionType SelectionType => SelectionType.Face;

        public void Start()
        {
            _toolController = ServiceLocator.Instance.Get<ToolController>();
        }
        
        protected override SelectionContext GetClampedSelection(Vector3 worldPosition)
        {
            var hexCoordinate = HexGrid.GetClosestHexCoordinate(worldPosition);
            var radius = _toolController.GetCurrentToolRadius();
            var cells = HexGrid.GetHexCoordinatesWithinRadius(hexCoordinate, radius);
            return new SelectionContext(SelectionType.Face, cells);
        }
        
        protected override SelectionContext GetClampedSelection(HexObject hexObject, Vector3 worldPosition)
        {
            var radius = _toolController.GetCurrentToolRadius();
            var cells = HexGrid.GetHexCoordinatesWithinRadius(hexObject.Coordinate, radius);
            return new SelectionContext(SelectionType.Face, cells);
        }
    }
}