﻿using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Map;

namespace Assets.Scripts.Units
{
    abstract class Unit : MonoBehaviour
    {
        World _world;
        [SerializeField] int _movement;
        Hex _hex;
        
        public World World => _world;
        public int Movement => _movement;
        public Hex Hex => _hex;

        /*protected Unit(int movement, Hex hex, World world)
        {
            this._movement = movement;
            this._hex = hex;
            this._world = world;
        }*/

        void Start()
        {
            
        }

        public List<Hex> GetReachableHexes()
        {
            return GetReachableHexes(Movement, Hex, new List<Hex>());
        }

        List<Hex> GetReachableHexes(int movement, Hex hex, List<Hex> hexes)
        {
            foreach (Hex hex0 in hex.Neighbours)
            {
                int movement0 = movement;
                movement0 -= GetMovementPoints(hex0.HexType);
                if (movement >= 0)
                    hexes.Add(hex0);
                if (movement > 0)
                    hexes.AddRange(GetReachableHexes(movement0, hex0, hexes));
            }
            return hexes;
        }

        protected virtual int GetMovementPoints(HexType hexType)
        {
            return hexType.MovementPoints;
        }

        public Unit Spawn(World world, int x, int y)
        {
            this._world = world;
            Unit prefabInstantiation = Instantiate(this, this._world.Grid.CellToWorld(new Vector3Int(x, y, 0)), Quaternion.identity);
            //Debug.Log(this.transform + ", " + world.transform);
            prefabInstantiation.transform.SetParent(world.transform);
            this._hex = this._world.GetHex(new Vector2Int(x, y));
            this._world.AddUnit(this._hex, this);
            return prefabInstantiation;
        }
    }
}
