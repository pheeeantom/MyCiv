using Assets.Scripts.Units;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets.Scripts.Map
{
    class World : MonoBehaviour
    {
        [SerializeField] public Tilemap tilemap;
        [SerializeField] public Tilemap units;

        protected int width = 512;
        protected int height = 512;

        public Hex[,] hexes;

        Dictionary<Vector2Int, Hex> _hexPositionsDict = new Dictionary<Vector2Int, Hex>();

        private void Start()
        {
            
        }

        public void createHexesRect()
        {
            hexes = new Hex[width, height];
            this.width = GetComponent<HexWorldGenerator>().Width;
            this.height = GetComponent<HexWorldGenerator>().Height;
        }

        public void AddHex(Hex hex, int i, int j)
        {
            hexes[i, j] = hex;
            AddHex(hex);
        }

        public void AddHex(Hex hex)
        {
            Vector2Int pos = hex.Position;
            //Debug.Log(pos);
            tilemap.SetTile((Vector3Int)pos, hex.HexType.Tile);
            //if (_hexPositionsDict.ContainsKey(pos)) 
            //    _hexPositionsDict.Remove(pos);
            _hexPositionsDict.Add(pos, hex);

            foreach (HexDirection dir in HexDirection.directions)
            {
                Vector2Int neighborPos = pos + dir.vector;
                if (_hexPositionsDict.ContainsKey(neighborPos))
                {
                    Hex.Connect(hex, _hexPositionsDict[neighborPos], dir);
                }
            }
        }

        public Hex GetHex(Vector2Int pos)
        {
            return _hexPositionsDict[pos];
        }

        public void HighlightHexes(List<Hex> hexes)
        {
            foreach (Hex hex in hexes)
            {
                this.units.SetColor((Vector3Int)hex.Position, new Color(0, 0, 0, 0.1f));
            }
        }
    }
}
