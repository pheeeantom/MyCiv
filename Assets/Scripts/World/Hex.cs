using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Map
{
    class Hex : Cell
    {
        Dictionary<HexDirection, Hex> _neighbours = new Dictionary<HexDirection, Hex>();
        
        public HexType HexType { get; set; }
        public Vector2Int Position => new Vector2Int(X, Y);
        public List<Hex> Neighbours => new List<Hex>(_neighbours.Values);

        public Hex(Vector2Int pos)
        {
            this.X = pos.x;
            this.Y = pos.y;
        }

        void Connect(Hex hex, HexDirection dir)
        {
            _neighbours.Add(dir, hex);
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
