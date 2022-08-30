using Assets.Scripts.Map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets.Scripts.Units
{
    class Pop : LandUnit
    {
        public Pop(int movement, Vector2Int pos, World world)
        {
            this.movement = movement;
            this.pos = pos;
            this.world = world;
        }

        public override void SpawnUnit()
        {
            Unit.units.Add(this);
            this.world.units.SetTile((Vector3Int)pos, (Tile)AssetDatabase.LoadAssetAtPath("Assets/Tiles/pop.asset", typeof(Tile)));
        }
    }
}
