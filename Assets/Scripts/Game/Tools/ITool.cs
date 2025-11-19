using Game.Hexes;
using UnityEngine;

namespace Game.Tools
{
    public interface ITool
    {
        public void Use(Cell cell, HexObject hex);
    }
}