using App.Services;
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
        
        private readonly HexController _hexController = ServiceLocator.Instance.Get<HexController>();

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
            
            // TODO: Below code should be replaced when the feature meshes are decided based on neighbour hexes
            if (hexes[0].Height == hexes[1].Height || riverPresent)
            {
                hexes[0].Edges.RemoveWaterfall(sharedEdgeA);
                hexes[1].Edges.RemoveWaterfall(sharedEdgeB);
                return true;
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
            
            return true;
        }
    }
}