using UnityEngine;
using System.Collections.Generic;

namespace Assets.Scripts.Map
{
    class HexDirection
    {
        public static HexDirection W = new HexDirection(new Vector2Int(-1, 0));
        public static HexDirection NW = new HexDirection(new Vector2Int(0, 1));
        public static HexDirection NE = new HexDirection(new Vector2Int(1, 1));
        public static HexDirection E = new HexDirection(new Vector2Int(1, 0));
        public static HexDirection SW = new HexDirection(new Vector2Int(0, -1));
        public static HexDirection SE = new HexDirection(new Vector2Int(1, -1));

        public static List<HexDirection> directions = new List<HexDirection>() { W, NW, NE, E, SW, SE };
        public static Dictionary<HexDirection, HexDirection> _opposites = new Dictionary<HexDirection, HexDirection>() { { W, E }, { E, W }, { NW, SE }, { SE, NW }, { NE, SW }, { SW, NE } };

        public readonly Vector2Int vector;

        HexDirection(Vector2Int vec)
        {
            this.vector = vec;
        }

        public HexDirection Opposite()
        {
            return _opposites[this];
        }
    }
}
