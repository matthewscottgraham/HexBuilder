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

        public bool Use(HexObject hex)
        {
            return false;
        }

        public bool Use(HexObject[] hexes)
        {
            if (hexes == null || hexes.Length != 2) return false;

            var sharedEdgeA = HexGrid.GetSharedEdgeIndex(hexes[0].Coordinate, hexes[1].Coordinate);
            if (sharedEdgeA < 0) return false;
            var sharedEdgeB = (sharedEdgeA + 3) % 6;
            
            var riverPresent = hexes[0].Edges.Exists(sharedEdgeA); // if present on any hex, it is present for all
            
            hexes[0].Edges.Set(sharedEdgeA, !riverPresent);
            hexes[1].Edges.Set(sharedEdgeB, !riverPresent);

            return true;

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