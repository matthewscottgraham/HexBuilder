using App.Services;
using Game.Hexes;
using UnityEngine;

namespace Game.Tools
{
    public class RaiseTerrain : ITool
    {
        public void Use(Cell cell, GameObject hex)
        {
            var hexController = ServiceLocator.Instance.Get<HexController>();
            
            if (!hexController.InBounds(cell)) return;

            if (hex == null)
            {
                hexController.CreateNewHex(cell);
                return;
            }
            
            var currentScale = hex.transform.localScale;
            hex.transform.localScale = new Vector3(currentScale.x, currentScale.y + 1, currentScale.z);
        }
    }
}