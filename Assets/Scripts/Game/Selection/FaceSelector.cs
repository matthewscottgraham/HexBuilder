using Game.Grid;
using Game.Hexes;
using UnityEngine;

namespace Game.Selection
{
    public class FaceSelector : Selector
    {
        public override SelectionType SelectionType => SelectionType.Face;
        
        protected override SelectionContext GetClampedSelection(Vector3 worldPosition)
        {
            var hexCoordinate = HexGrid.GetClosestHexCoordinate(worldPosition);
            return new SelectionContext(SelectionType.Face, hexCoordinate);
        }
        
        protected override SelectionContext GetClampedSelection(HexObject hexObject, Vector3 worldPosition)
        {
            return new SelectionContext(SelectionType.Face, hexObject.Coordinate);
        }
    }
}