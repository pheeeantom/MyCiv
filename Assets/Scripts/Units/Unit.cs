using System;
using System.Collections.Generic;
using UnityEngine;
using World;

namespace Units
{
    public abstract class Unit : MonoBehaviour
    {
        World.World _world;
        [SerializeField] int _movement;
        Hex _hex;
        Navigator _navigator;

        public World.World World => _world;
        public int Movement => _movement;
        public Hex Hex => _hex;

        [Obsolete("This method mustn't be called. Use Spawn instead.", true)]
        void Start() {}
        
        public List<Hex> FindShortestPath(Hex dest)
        {
            return _navigator.FindShortestPath(Movement, Hex, dest);
        }
        
        public List<Hex> GetReachableHexes()
        {
            return _navigator.GetReachableHexes(Movement, Hex);
        }

        public virtual int GetMovementPoints(HexType hexType)
        {
            return hexType.MovementPoints;
        }

        public Unit Spawn(World.World world, int x, int y)
        {
            this._world = world;
            this._hex = world.GetHex(new Vector2Int(x, y));
            this._navigator = new Navigator(this);
            Unit instantiation = Instantiate(this, this._hex.GetWorldPosition(this._world.Grid), Quaternion.identity);
            instantiation.transform.SetParent(this._world.transform);
            this._hex.AddUnit(this);
            return instantiation;
        }
    }
}
