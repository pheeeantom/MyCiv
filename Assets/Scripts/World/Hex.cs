using Assets.Scripts.Map;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Map
{
    class Hex : Cell
    {
        Dictionary<HexDirection, Hex> _neighbours = new Dictionary<HexDirection, Hex>();

        public HexType HexType { get; set; }
        Vector2Int _position;
        public Vector2Int Position { get => _position; }

        public Hex(Vector2Int pos)
        {
            this._position = pos;
        }

        void Connect(Hex hex, HexDirection direction)
        {
            //if (_neighbours.ContainsKey(direction))
            //    _neighbours.Remove(direction);
            _neighbours.Add(direction, hex);
        }

        public static void Connect(Hex hex, Hex hex1, HexDirection dir)
        {
            hex.Connect(hex1, dir);
            hex1.Connect(hex, dir.Opposite());
        }

        public Hex GetNeighbour(HexDirection dir)
        {
            return _neighbours[dir];
        }
    }
}
