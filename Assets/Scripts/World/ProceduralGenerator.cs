using UnityEngine;

namespace Assets.Scripts.Map
{
    class ProceduralGenerator : MonoBehaviour
    {
        World _world;

        private void Start()
        {
            this._world = GetComponent<World>();

            this.Generate(_world);
        }

        void Generate(World world)
        {
            for (int i = 0; i < 10; ++i)
            {
                _world.AddHex(new Hex(HexType.Ocean, new Vector2Int(i, 0)));
            }

        }
    }
}
