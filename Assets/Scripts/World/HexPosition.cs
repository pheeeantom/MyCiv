using System;
using System.Collections.Generic;
using UnityEngine;

namespace World
{
    public class HexPosition
    {
        const float HexSpacing = 1.5f;

        readonly int _gridX;
        readonly int _gridY;
     
        internal float X => _gridY % 2 == 0 ? _gridX * HexSpacing : _gridX * HexSpacing + HexSpacing * 0.5f;
        internal float Y => _gridY * (HexSpacing * 0.5f);
        internal Vector3 Vector3 => new Vector3(X, Y, 0f);
        internal Vector2Int Vector2Int => new Vector2Int(_gridX, _gridY);
        internal Vector3Int Vector3Int => new Vector3Int(_gridX, _gridY, 0);
        
        public static readonly List<CardinalDirection> Directions = new List<CardinalDirection>()
        {
            CardinalDirection.W,
            CardinalDirection.NW,
            CardinalDirection.NE,
            CardinalDirection.E,
            CardinalDirection.SE,
            CardinalDirection.SW
        };
     
        internal HexPosition(int gridx, int gridy)
        {
            this._gridX = gridx;
            this._gridY = gridy;
        }
        
        protected bool Equals(HexPosition other)
        {
            return this == other;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((HexPosition)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (_gridX * 397) ^ _gridY;
            }
        }

        internal List<HexPosition> NeighbourPositions
        {
            get
            {
                List<HexPosition> list = new List<HexPosition>();
                int xOffset = 0;
                if (_gridY % 2 != 0)
                    xOffset = 1;
     
                list.Add(new HexPosition(_gridX - 1, _gridY));
                list.Add(new HexPosition(_gridX + xOffset - 1, _gridY + 1));
                list.Add(new HexPosition(_gridX + xOffset, _gridY + 1));
                list.Add(new HexPosition(_gridX + 1, _gridY));
                list.Add(new HexPosition(_gridX + xOffset, _gridY - 1));
                list.Add(new HexPosition(_gridX + xOffset - 1, _gridY - 1));
     
                return list;
            }
        }
     
        internal HexPosition NeighbourInDirection(CardinalDirection dir)
        {
            int xOffset = 0;
            if (_gridY % 2 != 0)
                xOffset = 1;
     
            if (dir == CardinalDirection.W)
                return new HexPosition(_gridX - 1, _gridY);
            if (dir == CardinalDirection.NW)
                return new HexPosition(_gridX + xOffset - 1, _gridY + 1);
            if (dir == CardinalDirection.NE)
                return new HexPosition(_gridX + xOffset, _gridY + 1);
            if (dir == CardinalDirection.E)
                return new HexPosition(_gridX + 1, _gridY);
            if (dir == CardinalDirection.SE)
                return new HexPosition(_gridX + xOffset, _gridY - 1);
            if (dir == CardinalDirection.SW)
                return new HexPosition(_gridX + xOffset - 1, _gridY - 1);
            
            throw new Exception("Invalid direction.");
        }
     
        public static bool operator ==(HexPosition pos0, HexPosition pos1)
        {
            return pos0._gridX == pos1._gridX && pos0._gridY == pos1._gridY;
        }
        
        public static bool operator !=(HexPosition pos0, HexPosition pos1)
        {
            return !(pos0 == pos1);
        }
     
        public static implicit operator Vector3(HexPosition pos)
        {
            return pos.Vector3;
        }
        
        public static implicit operator Vector3Int(HexPosition pos)
        {
            return pos.Vector3Int;
        }
        
        public static implicit operator Vector2Int(HexPosition pos)
        {
            return pos.Vector2Int;
        }
     
        public override string ToString()
        {
            return "(X = " + _gridX + "; Y = " + _gridY + ")";
        }
    }
}