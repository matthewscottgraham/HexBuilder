using UnityEngine;

namespace Game.Tools
{
    public class LowerTerrain : ITool
    {
        public void Use(GameObject hex)
        {
            var newScale = hex.transform.localScale;
            newScale.y -= 1;
            if (newScale.y < 0) newScale.y = 0;
            hex.transform.localScale = newScale;
        }
    }
}