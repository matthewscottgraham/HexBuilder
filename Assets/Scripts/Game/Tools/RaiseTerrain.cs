using App.Services;
using Game.Hexes;
using UnityEngine;

namespace Game.Tools
{
    public class RaiseTerrain : ITool
    {
        public void Use(Cell cell, HexObject hex)
        {
            var hexController = ServiceLocator.Instance.Get<HexController>();
            
            if (!hexController.InBounds(cell)) return;

            if (hex == null)
            {
                hexController.CreateNewHex(cell);
                return;
            }
            
            hex.SetHeight(hex.Height + 1);
        }
    }
}