using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Assets.Scripts.Map;

namespace Assets.Scripts.Units
{
    abstract class Unit : MonoBehaviour
    {
        public static List<Unit> units = new List<Unit>();
        public World world;
        public int movement;
        public Vector2Int pos;

        abstract public void SpawnUnit();

        public List<Hex> GetMovementField(int cost, Hex hex)
        {
            List<Hex> res = new List<Hex>();
            foreach (HexDirection dir in HexDirection.directions)
            {
                hex = hex.GetNeighbour(dir);
                int costNew = cost;
                costNew -= HexType.GetMovementPoints(hex.HexType, this);
                if (cost >= 0)
                {
                    res.Add(hex);
                }
                if (cost > 0)
                {
                    res.AddRange(this.GetMovementField(costNew, hex));
                }
            }
            return res;
        }
    }
}
