using UnityEngine;
using System.Collections.Generic;

namespace World
{
    public class CardinalDirection
    {
        public static readonly CardinalDirection W = new CardinalDirection(new Vector2Int(-1, 0));
        public static readonly CardinalDirection NE = new CardinalDirection(new Vector2Int(1, 1));
        public static readonly CardinalDirection N = new CardinalDirection(new Vector2Int(0, 1));
        public static readonly CardinalDirection NW = new CardinalDirection(new Vector2Int(-1, 1));
        public static readonly CardinalDirection E = new CardinalDirection(new Vector2Int(1, 0));
        public static readonly CardinalDirection SW = new CardinalDirection(new Vector2Int(-1, -1));
        public static readonly CardinalDirection S = new CardinalDirection(new Vector2Int(0, -1));
        public static readonly CardinalDirection SE = new CardinalDirection(new Vector2Int(1, -1));

        public static readonly List<CardinalDirection> Directions = new List<CardinalDirection>() { W, NW, N, NE, E, SE, S, SW };
        static readonly Dictionary<CardinalDirection, CardinalDirection> Opposites = new Dictionary<CardinalDirection, CardinalDirection>() { { W, E }, { NW, SE }, { N, S }, { NE, SW }, { E, W }, { SE, NW }, { S, N }, { SW, NE } };

        public readonly Vector2Int vector;
        
        

        CardinalDirection(Vector2Int vec)
        {
            this.vector = vec;
        }

        public CardinalDirection Opposite()
        {
            return Opposites[this];
        }
    }
}
