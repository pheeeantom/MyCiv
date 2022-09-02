using System.Collections.Generic;
using Units;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace World
{
    public class World : MonoBehaviour
    {
        Grid _grid;
        [SerializeField] public Tilemap landscape;
        [SerializeField] public Tilemap highlights;

        public Grid Grid => _grid;

        protected int _width = 512;
        protected int _height = 512;

        private Hex[,] _hexes;
        public Hex[,] HexMatrix => _hexes;

        Dictionary<Vector2Int, Hex> _hexPositionsDict = new Dictionary<Vector2Int, Hex>();
        

        private void Start()
        {
            _grid = GetComponent<Grid>();
        }

        public void createHexesRect()
        {
            _hexes = new Hex[_width, _height];
            this._width = GetComponent<HexWorldGenerator>().Width;
            this._height = GetComponent<HexWorldGenerator>().Height;
        }

        public void AddHex(Hex hex, int i, int j)
        {
            _hexes[i, j] = hex;
            AddHex(hex);
        }

        public void AddHex(Hex hex)
        {
            HexPosition pos = hex.Position;
            //Debug.Log(pos);
            landscape.SetTile(pos, hex.HexType.Tile);
            //if (_hexPositionsDict.ContainsKey(pos)) 
            //    _hexPositionsDict.Remove(pos);
            _hexPositionsDict.Add(pos, hex);

            foreach (CardinalDirection dir in HexPosition.Directions)
            {
                HexPosition neighborPos = pos.NeighbourInDirection(dir);
                if (_hexPositionsDict.ContainsKey(neighborPos))
                {
                    Hex.Connect(hex, _hexPositionsDict[neighborPos], dir);
                }
            }
        }

        public Hex GetHex(Vector2Int pos)
        {
            return _hexPositionsDict.ContainsKey(pos) ? _hexPositionsDict[pos] : null;
        }

        public void HighlightHexes(List<Hex> hexes)
        {
            this.highlights.ClearAllTiles();
            foreach (Hex hex in hexes)
            {
                this.highlights.SetTile((Vector3Int)hex.Position, Tiles.Movement);
            }
        }
    }
}
