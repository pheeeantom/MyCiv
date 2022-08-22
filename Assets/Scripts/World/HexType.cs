using Assets.Scripts.World;
using UnityEngine.Tilemaps;

namespace Assets.Scripts.Map
{
    class HexType
    {
        public static HexType Ocean = new HexType("ocean", Tiles.Ocean);

        string _name;
        Tile _tile;

        public Tile Tile { get => _tile; }

        public HexType(string name, Tile tile)
        {
            this._tile = tile;
            this._name = name;
        }
    }
}
