using UnityEngine;
using System.Collections.Generic;

namespace Assets.Scripts.Map
{
    class HexDirection
    {
        public static readonly HexDirection W = new HexDirection(new Vector2Int(-1, 0));
        public static readonly HexDirection NW = new HexDirection(new Vector2Int(0, 1));
        public static readonly HexDirection NE = new HexDirection(new Vector2Int(1, 1));
        public static readonly HexDirection E = new HexDirection(new Vector2Int(1, 0));
        public static readonly HexDirection SW = new HexDirection(new Vector2Int(0, -1));
        public static readonly HexDirection SE = new HexDirection(new Vector2Int(1, -1));

        public static readonly List<HexDirection> Directions = new List<HexDirection>() { W, NW, NE, E, SW, SE };
        static readonly Dictionary<HexDirection, HexDirection> Opposites = new Dictionary<HexDirection, HexDirection>() { { W, E }, { E, W }, { NW, SE }, { SE, NW }, { NE, SW }, { SW, NE } };

        public readonly Vector2Int vector;

        HexDirection(Vector2Int vec)
        {
            this.vector = vec;
        }

        public HexDirection Opposite()
        {
            return Opposites[this];
        }
    }
}
