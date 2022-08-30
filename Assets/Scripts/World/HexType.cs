using Assets.Scripts.Map;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;
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
            if (unit is MarineUnit)
            {
                if (hexType == Ocean || hexType == CoastLake)
                    return 1;
                else
                    return 1000;
            }
            else
            {
                if (hexType == Desert || hexType == Snow)
                    return 2;
                else if (hexType == Mountains)
                    return 3;
                else
                    return 1;
            }
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
                    else if (moisture == MoistureType.Dry)
                    {
                        return Plains;
                    }
                    else
                    {
                        return Snow;
                    }
                }
                else if (heat == HeatType.Warm)
                {
                    if (moisture == MoistureType.Dryest || moisture == MoistureType.Dryer || moisture == MoistureType.Dry || moisture == MoistureType.Wet)
                    {
                        return Plains;
                    }
                    else
                    {
                        return Grassland;
                    }
                }
                else if (heat == HeatType.Warmer)
                {
                    if (moisture == MoistureType.Dry || moisture == MoistureType.Dryer)
                    {
                        return Plains;
                    }
                    else if (moisture == MoistureType.Wet || moisture == MoistureType.Wetter || moisture == MoistureType.Wettest)
                    {
                        return Grassland;
                    }
                    else
                    {
                        return Desert;
                    }
                }
                else {
                    return Desert;
                }
            }
            else if (height == HeightType.Snow || height == HeightType.Rock)
            {
                return Mountains;
            }
            else if (height == HeightType.ShallowWater)
            {
                return CoastLake;
            }
            else
            {
                return Ocean;
            }
        }

        public HexType(string name, Tile tile)
        {
            this._tile = tile;
            this._name = name;
        }
    }
}
