using App.Services;
using Game.Hexes;
using UnityEngine;

namespace Game.Tools
{
    public class LevelTerrain : ITool
    {
        public float Level { get; set; }
        
        public void Use(Cell cell, GameObject hex)
        {
            var hexController = ServiceLocator.Instance.Get<HexController>();
            
            if (!hexController.InBounds(cell)) return;

            if (hex == null) hex = hexController.CreateNewHex(cell);

            var currentScale = hex.transform.localScale;
            hex.transform.localScale = new Vector3(currentScale.x, Level, currentScale.z);
        }
    }
}