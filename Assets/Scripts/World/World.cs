using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets.Scripts.Map
{
    class World : MonoBehaviour
    {
        [SerializeField] Tilemap _tilemap;

        Dictionary<Vector2Int, Hex> _hexPositionsDict = new Dictionary<Vector2Int, Hex>();
        public List<Hex> hexes = new List<Hex>();


        private void Start()
        {
        }

        public void AddHex(Hex hex)
        {
            Vector2Int pos = hex.Position;
            _tilemap.SetTile((Vector3Int)pos, hex.HexType.Tile);
            _hexPositionsDict.Add(pos, hex);
            hexes.Add(hex);

            foreach (Direction dir in Direction.directions)
            {
                Vector2Int neighborPos = pos + dir.vector;
                if (_hexPositionsDict.ContainsKey(neighborPos))
                {
                    hex.Connect(_hexPositionsDict[neighborPos], dir);
                    _hexPositionsDict[neighborPos].Connect(hex, dir.Reversed());
                }
            }
        }

        public Hex GetHex(Vector2Int pos)
        {
            return _hexPositionsDict[pos];
        }
    }
}
