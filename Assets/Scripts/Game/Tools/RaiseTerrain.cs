using UnityEngine;

namespace Game.Tools
{
    public class RaiseTerrain : ITool
    {
        public void Use(GameObject hex)
        {
            var currentScale = hex.transform.localScale;
            hex.transform.localScale = new Vector3(currentScale.x, currentScale.y + 1, currentScale.z);
        }
    }
}