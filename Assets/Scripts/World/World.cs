using Assets.Scripts.Units;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets.Scripts.Map
{
    class World : MonoBehaviour
    {
        Grid _grid;
        [SerializeField] public Tilemap tilemap;
        [SerializeField] public Tilemap units;

        public Grid Grid => _grid;

        protected int width = 512;
        protected int height = 512;

        public Hex[,] hexes;

        Dictionary<Vector2Int, Hex> _hexPositionsDict = new Dictionary<Vector2Int, Hex>();
        Dictionary<Hex, List<Unit>> _hexUnitsDict = new Dictionary<Hex, List<Unit>>();

        private void Start()
        {
            _grid = GetComponent<Grid>();
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
            _hexUnitsDict.Add(hex, new List<Unit>());

            foreach (HexDirection dir in HexDirection.Directions)
            {
                Vector2Int neighborPos = pos + dir.vector;
                if (_hexPositionsDict.ContainsKey(neighborPos))
                {
                    Hex.Connect(hex, _hexPositionsDict[neighborPos], dir);
                }
            }
        }

        public void AddUnit(Hex hex, Unit unit)
        {
            _hexUnitsDict[hex].Add(unit);
        }

        public Hex GetHex(Vector2Int pos)
        {
            return _hexPositionsDict[pos];
        }

        public List<Unit> GetUnits(Hex hex)
        {
            return _hexUnitsDict[hex];
        }

        public void HighlightHexes(List<Hex> hexes)
        {
            foreach (Hex hex in hexes)
            {
                this.units.SetTile((Vector3Int)hex.Position, Tiles.Movement);
            }
        }
    }
}
