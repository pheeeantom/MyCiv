using UnityEngine.Tilemaps;
using Assets.Scripts.Units;

namespace Assets.Scripts.Map
{
    class HexType
    {
        public static HexType Ocean = new HexType("ocean", Tiles.Ocean);
        public static HexType Desert = new HexType("desert", Tiles.Desert);
        public static HexType Grassland = new HexType("grassland", Tiles.Grassland);
        public static HexType Mountains = new HexType("mountains", Tiles.Mountains);
        public static HexType CoastLake = new HexType("coast_lake", Tiles.CoastLake);
        public static HexType Plains = new HexType("plains", Tiles.Plains);
        public static HexType Snow = new HexType("snow", Tiles.Snow);
        public static HexType Tundra = new HexType("tundra", Tiles.Tundra);
        public static HexType Fog = new HexType("fog", Tiles.Fog);

        //List<HexType> lands = new List<HexType>() { Plains };

        string _name;
        Tile _tile;

        public Tile Tile { get => _tile; }

        public static int GetMovementPoints(HexType hexType, Unit unit)
        {
            if (unit is NavalUnit)
            {
                if (hexType == Ocean || hexType == CoastLake)
                    return 1;
                return 1000;
            }
            if (hexType == Desert || hexType == Snow)
                return 2;
            if (hexType == Mountains)
                return 3;
            return 1;
        }

        public static HexType GetHexType(HeightType height, MoistureType moisture, HeatType heat)
        {
            if (height == HeightType.Sand || height == HeightType.Grass || height == HeightType.Forest)
            {
                if (heat == HeatType.Coldest || heat == HeatType.Colder)
                {
                    return Snow;
                }
                if (heat == HeatType.Cold)
                {
                    if (moisture == MoistureType.Wettest || moisture == MoistureType.Wettest || moisture == MoistureType.Wet)
                    {
                        return Tundra;
                    }

                    if (moisture == MoistureType.Dry)
                    {
                        return Plains;
                    }
                    return Snow;
                }

                if (heat == HeatType.Warm)
                {
                    if (moisture == MoistureType.Dryest || moisture == MoistureType.Dryer || moisture == MoistureType.Dry || moisture == MoistureType.Wet)
                    {
                        return Plains;
                    }

                    return Grassland;
                }
                if (heat == HeatType.Warmer)
                {
                    if (moisture == MoistureType.Dry || moisture == MoistureType.Dryer)
                    {
                        return Plains;
                    }

                    if (moisture == MoistureType.Wet || moisture == MoistureType.Wetter || moisture == MoistureType.Wettest)
                    {
                        return Grassland;
                    }
                    return Desert;
                }
                return Desert;
            }

            if (height == HeightType.Snow || height == HeightType.Rock)
            {
                return Mountains;
            }
            if (height == HeightType.ShallowWater)
            {
                return CoastLake;
            }
            return Ocean;
        }

        public HexType(string name, Tile tile)
        {
            this._tile = tile;
            this._name = name;
        }
    }
}
