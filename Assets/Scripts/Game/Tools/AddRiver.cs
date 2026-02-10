using Game.Hexes;
using Game.Selection;
using UnityEngine;

namespace Game.Tools
{
    public class AddRiver : ITool
    {
        Sprite ITool.Icon => Resources.Load<Sprite>("Sprites/river");
        bool ITool.UseRadius => false;
        SelectionType ITool.SelectionType => SelectionType.Edge;
        
        private HexController _hexController;
        
        public void Use(HexObject hex)
        {
            // if (selection.SelectionType != SelectionType.Edge) return;
            // if (!hex) return;
            //
            // hex.Edges.Toggle(selection.ComponentIndex);
            //
            // if (!_hexController) _hexController = ServiceLocator.Instance.Get<HexController>();
            //
            // var neighbourCoordinate = HexGrid.GetNeighbourSharingEdge(selection.Coordinate, selection.ComponentIndex);
            // var neighbourEdgeIndex = HexGrid.GetNeighboursSharedEdgeIndex(selection.ComponentIndex);
            // var neighbourHex = _hexController.GetHexObject(neighbourCoordinate);
            // if (!neighbourHex) return;
            //
            // var active = hex.Edges.Exists(selection.ComponentIndex);
            //
            // neighbourHex.Edges.Set(neighbourEdgeIndex, active);
            // if (hex.Height != neighbourHex.Height && active)
            // {
            //     var waterfall = _hexController.WaterfallFactory.CreateWaterFall(
            //         hex, 
            //         neighbourHex, 
            //         selection.ComponentIndex,
            //         neighbourEdgeIndex);
            //     hex.Edges.AddWaterfall(waterfall, selection.ComponentIndex);
            // }
            // else if (hex.Height != neighbourHex.Height)
            // {
            //     hex.Edges.RemoveWaterfall(selection.ComponentIndex);
            // }
        }
    }
}