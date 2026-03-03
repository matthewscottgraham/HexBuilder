using System;
using System.Linq;
using Game.Grid;
using Game.Hexes;
using Game.Hexes.Features;
using Game.Selection;
using UnityEngine;

namespace Game.Tools
{
    public class PathTool : Tool
    {
        public PathTool()
        {
            UseRadius = false;
            Icon = Resources.Load<Sprite>("Sprites/path");
            SelectionType = SelectionType.Vertex;
        }
        
        public override bool Use(HexObject[] hexes)
        {
            if (hexes is not { Length: 3 }) return false;
            if (hexes.Any(hex => hex.Height < HexFactory.WaterHeight))
            {
                return false;
            }

            var sharedVertexA = HexGrid.GetSharedVertexIndex(hexes[0].Coordinate, hexes[1].Coordinate, hexes[2].Coordinate);
            if (sharedVertexA < 0) return false;
            
            var sharedVertexB = HexGrid.GetSharedVertexIndex(hexes[1].Coordinate, hexes[2].Coordinate, hexes[0].Coordinate);
            if (sharedVertexB < 0) return false;
            
            var sharedVertexC = HexGrid.GetSharedVertexIndex(hexes[2].Coordinate, hexes[0].Coordinate, hexes[1].Coordinate);
            if (sharedVertexC < 0) return false;

            var exists = hexes[0].Vertices.Exists(sharedVertexA); // if present on any hex, it is present for all
            var changed = Use(hexes[0], sharedVertexA, exists);
            changed += Use(hexes[1], sharedVertexB, exists); 
            changed += Use(hexes[2], sharedVertexC, exists);

            return changed > 0;
        }

        private int Use(HexObject hex, int vertex, bool exists)
        {
            return UseToggle(hex, vertex, exists);
        }

        private static int UseAdditive(HexObject hex, int vertex)
        {
            if (hex.Vertices.Exists(vertex)) return 0;
            hex.Vertices.Set(vertex, true);
            return 1;
        }

        private static int UseSubtractive(HexObject hex, int vertex)
        {
            if (!hex.Vertices.Exists(vertex)) return 0;
            hex.Vertices.Set(vertex, false);
            return 1;
        }
        
        private static int UseToggle(HexObject hex, int vertex, bool exists)
        {
            return exists ? UseSubtractive(hex, vertex) : UseAdditive(hex, vertex);
        }
    }
}