using App.Services;
using Game.Hexes;
using UnityEngine;

namespace Game.Tools
{
    public class LevelTerrain : ITool
    {
        public float Level { get; set; }
        
        public void Use(Cell cell, HexObject hex)
        {
            var hexController = ServiceLocator.Instance.Get<HexController>();
            
            if (!hexController.InBounds(cell)) return;

            if (hex == null) hex = hexController.CreateNewHex(cell);
            
            hex.RemoveFeature();
            
            hex.SetHeight(Level);
        }
    }
}