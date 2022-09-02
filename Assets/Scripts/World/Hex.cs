using System.Collections.Generic;
using Units;
using UnityEngine;

namespace World
{
    public class Hex : Cell
    {
        Dictionary<CardinalDirection, Hex> _neighbours = new Dictionary<CardinalDirection, Hex>();
        Unit _unit;

        public HexType HexType { get; set; }
        public HexPosition Position => new HexPosition(X, Y);
        public List<Hex> Neighbours => new List<Hex>(_neighbours.Values);
        public Unit Unit => _unit;

        public Hex(Vector2Int pos)
        {
            this.X = pos.x;
            this.Y = pos.y;
        }

        void Connect(Hex hex, CardinalDirection dir)
        {
            _neighbours.Add(dir, hex);
        }

        public static void Connect(Hex hex, Hex hex1, CardinalDirection dir)
        {
            hex.Connect(hex1, dir);
            hex1.Connect(hex, dir.Opposite());
        }

        public Hex GetNeighbour(CardinalDirection dir)
        {
            return _neighbours[dir];
        }

        public Vector3 GetWorldPosition(Grid grid)
        {
            return grid.CellToWorld(this.Position);
        }

        public void AddUnit(Unit unit)
        {
            this._unit = unit;
        }
    }
}
