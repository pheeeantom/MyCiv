using UnityEngine;
using System.Collections.Generic;

namespace Assets.Scripts.Map
{
    class Direction
    {
        public static Direction west = new Direction(new Vector2Int(-1, 0));
        public static Direction northWest = new Direction(new Vector2Int(0, 1));
        public static Direction northEast = new Direction(new Vector2Int(1, 1));
        public static Direction east = new Direction(new Vector2Int(1, 0));
        public static Direction southWest = new Direction(new Vector2Int(0, -1));
        public static Direction southEast = new Direction(new Vector2Int(1, -1));

        public static List<Direction> directions = new List<Direction>() { west, northWest, northEast, east, southWest, southEast };
        public static Dictionary<Direction, Direction> _reverses = new Dictionary<Direction, Direction>() { { west, east }, { east, west }, { northWest, southEast }, { southEast, northWest }, { northEast, southWest }, { southWest, northEast } };

        public readonly Vector2Int vector;

        Direction(Vector2Int vec)
        {
            this.vector = vec;
        }

        public Direction Reversed()
        {
            return _reverses[this];
        }
    }
}
