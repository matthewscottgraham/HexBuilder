using App.Services;
using Game.Hexes;
using UnityEngine;

namespace Game.Tools
{
    public class LowerTerrain : ITool
    {
        public void Use(Cell cell, GameObject hex)
        {
            var hexController = ServiceLocator.Instance.Get<HexController>();
            
            if (!hexController.InBounds(cell) || hex == null) return;
            
            var newScale = hex.transform.localScale;
            newScale.y -= 1;
            if (newScale.y < 0) newScale.y = 0;
            hex.transform.localScale = newScale;
        }
    }
}