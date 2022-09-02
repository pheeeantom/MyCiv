using System.Collections.Generic;
using World;

namespace Units
{
    public class Navigator
    {
        Unit _unit;
        
        public Navigator(Unit unit)
        {
            this._unit = unit;
        }
        
        Dictionary<Hex, HexReach> GetPromotions(int movement, Hex src)
        {
            List<Hex> queue = new List<Hex>() { src };
            Dictionary<Hex, HexReach> promotions = new Dictionary<Hex, HexReach>() { { src, new HexReach(src, null, 0) } };

            while (queue.Count > 0)
            {
                Hex hex = queue[0];
                queue.RemoveAt(0);
                
                foreach (Hex neighbour in hex.Neighbours)
                {
                    if (!promotions.ContainsKey(neighbour))
                    {
                        int distance = promotions[hex].distance + _unit.GetMovementPoints(neighbour.HexType);
                        if (movement <= distance)
                            continue;
                        queue.Add(neighbour);
                        promotions.Add(neighbour, new HexReach(neighbour, promotions[hex], distance));
                    }
                }
            }

            return promotions;
        }

        public List<Hex> FindShortestPath(int movement, Hex src, Hex dest)
        {
            Dictionary<Hex, HexReach> promotions = GetPromotions(movement, src);
            return promotions.ContainsKey(dest) ? promotions[dest].BuildPath() : new List<Hex>();
        }
        
        public List<Hex> GetReachableHexes(int movement, Hex src)
        {
            return new List<Hex>(GetPromotions(movement, src).Keys);
        }
        
        class HexReach
        {
            public Hex hex;
            public HexReach previousHexReach;
            public int distance;

            public HexReach(Hex hex, HexReach previousHexReach, int distance)
            {
                this.hex = hex;
                this.previousHexReach = previousHexReach;
                this.distance = distance;
            }

            public List<Hex> BuildPath()
            {
                List<Hex> path = new List<Hex> { hex };
                HexReach prev = previousHexReach;
                while (prev != null)
                {
                    path.Add(prev.hex);
                    prev = prev.previousHexReach;
                }

                return path;
            }
        }
    }
}