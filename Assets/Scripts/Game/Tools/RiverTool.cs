using System;
using System.Linq;
using App.Services;
using Game.Grid;
using Game.Hexes;
using Game.Selection;
using UnityEngine;

namespace Game.Tools
{
    public class RiverTool : Tool
    {
        private readonly HexController _hexController = ServiceLocator.Instance.Get<HexController>();

        public RiverTool()
        {
            UseRadius = false;
            Icon = Resources.Load<Sprite>("Sprites/river");
            SelectionType = SelectionType.Edge;
        }
        
        public override bool Use(HexObject[] hexes)
        {
            if (hexes is not { Length: 2 }) return false;
            if (hexes.Any(hex => hex.Height < HexFactory.WaterHeight))
            {
                return false;
            }

            var sharedEdgeA = HexGrid.GetSharedEdgeIndex(hexes[0].Coordinate, hexes[1].Coordinate);
            if (sharedEdgeA < 0) return false;
            var sharedEdgeB = (sharedEdgeA + 3) % 6;
            
            var exists = hexes[0].Edges.Exists(sharedEdgeA); // if present on any hex, it is present for all
            var changed = Use(hexes[0], sharedEdgeA, exists);
            changed += Use(hexes[1], sharedEdgeB, exists);
            
            // TODO: Below code should be replaced when the feature meshes are decided based on neighbour hexes
            if (hexes[0].Height == hexes[1].Height || exists)
            {
                hexes[0].Edges.RemoveWaterfall(sharedEdgeA);
                hexes[1].Edges.RemoveWaterfall(sharedEdgeB);
                return changed > 0;
            }
            
            var waterfallA = _hexController.WaterfallFactory.CreateWaterFall(
                hexes[0], 
                hexes[1], 
                sharedEdgeA,
                sharedEdgeB);
            hexes[0].Edges.AddWaterfall(waterfallA, sharedEdgeA);
                
            var waterfallB = _hexController.WaterfallFactory.CreateWaterFall(
                hexes[1], 
                hexes[0], 
                sharedEdgeB,
                sharedEdgeA);
            hexes[1].Edges.AddWaterfall(waterfallB, sharedEdgeB);
            
            return changed > 0;
        }
        
        private int Use(HexObject hex, int edge, bool exists)
        {
            return UseToggle(hex, edge, exists);
        }

        private static int UseAdditive(HexObject hex, int edge)
        {
            if (hex.Edges.Exists(edge)) return 0;
            hex.Edges.Set(edge, true);
            return 1;
        }

        private static int UseSubtractive(HexObject hex, int edge)
        {
            if (!hex.Edges.Exists(edge)) return 0;
            hex.Edges.Set(edge, false);
            return 1;
        }
        
        private static int UseToggle(HexObject hex, int edge, bool exists)
        {
            return exists ? UseSubtractive(hex, edge) : UseAdditive(hex, edge);
        }
    }
}