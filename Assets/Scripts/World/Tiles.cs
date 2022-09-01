using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets.Scripts.Map
{
    class Tiles : MonoBehaviour
    {
        public static Tile Ocean = (Tile)AssetDatabase.LoadAssetAtPath("Assets/Tiles/ocean.asset", typeof(Tile));
        public static Tile Desert = (Tile)AssetDatabase.LoadAssetAtPath("Assets/Tiles/desert.asset", typeof(Tile));
        public static Tile Grassland = (Tile)AssetDatabase.LoadAssetAtPath("Assets/Tiles/grassland.asset", typeof(Tile));
        public static Tile Mountains = (Tile)AssetDatabase.LoadAssetAtPath("Assets/Tiles/mountains.asset", typeof(Tile));
        public static Tile CoastLake = (Tile)AssetDatabase.LoadAssetAtPath("Assets/Tiles/coast_lake.asset", typeof(Tile));
        public static Tile Plains = (Tile)AssetDatabase.LoadAssetAtPath("Assets/Tiles/plains.asset", typeof(Tile));
        public static Tile Snow = (Tile)AssetDatabase.LoadAssetAtPath("Assets/Tiles/snow.asset", typeof(Tile));
        public static Tile Tundra = (Tile)AssetDatabase.LoadAssetAtPath("Assets/Tiles/tundra.asset", typeof(Tile));
        public static Tile Fog = (Tile)AssetDatabase.LoadAssetAtPath("Assets/Tiles/fog.asset", typeof(Tile));
        public static Tile Movement = (Tile)AssetDatabase.LoadAssetAtPath("Assets/Tiles/movement.asset", typeof(Tile));
    }
}
