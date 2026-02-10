using System.Collections.Generic;
using System.Linq;
using Game.Grid;
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
            // Noop
        }

        public void Use(HexObject[] hexes)
        {
            if (hexes == null || hexes.Length != 2) return;

            var sharedEdgeA = HexGrid.GetSharedEdgeIndex(hexes[0].Coordinate, hexes[1].Coordinate);
            if (sharedEdgeA < 0) return;
            var sharedEdgeB = (sharedEdgeA + 3) % 6;
            
            hexes[0].Edges.Set(sharedEdgeA, true);
            hexes[1].Edges.Set(sharedEdgeB, true);

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