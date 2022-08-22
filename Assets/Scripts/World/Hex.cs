using Assets.Scripts.Map;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    class Hex
    {
        Dictionary<Direction, Hex> _neighbours = new Dictionary<Direction, Hex>();

        public HexType HexType { get; set; }
        Vector2Int _position;
        public Vector2Int Position { get => _position; }

        public Hex(HexType hexType, Vector2Int pos)
        {
            this.HexType = hexType;
            this._position = pos;
        }

        public void Connect(Hex hex, Direction direction)
        {
            _neighbours.Add(direction, hex);
        }

        public Hex GetNeighbour(Direction direction)
        {
            return _neighbours[direction];
        }
    }
}
