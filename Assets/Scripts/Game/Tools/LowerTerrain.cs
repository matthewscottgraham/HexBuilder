using App.Services;
using Game.Hexes;
using UnityEngine;

namespace Game.Tools
{
    public class LowerTerrain : ITool
    {
        public void Use(Cell cell, HexObject hex)
        {
            var hexController = ServiceLocator.Instance.Get<HexController>();
            
            if (!hexController.InBounds(cell) || hex == null) return;
            
            hex.SetHeight(hex.Height - 1);
        }
    }
}