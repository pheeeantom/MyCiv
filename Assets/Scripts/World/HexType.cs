using UnityEngine.Tilemaps;

namespace Assets.Scripts.Map
{
    class HexType
    {
        public static HexType Ocean = new HexType("ocean", Tiles.Ocean, 1000);
        public static HexType Desert = new HexType("desert", Tiles.Desert, 2);
        public static HexType Grassland = new HexType("grassland", Tiles.Grassland, 1);
        public static HexType Mountains = new HexType("mountains", Tiles.Mountains, 3);
        public static HexType CoastLake = new HexType("coast_lake", Tiles.CoastLake, 1000);
        public static HexType Plains = new HexType("plains", Tiles.Plains, 1);
        public static HexType Snow = new HexType("snow", Tiles.Snow, 1);
        public static HexType Tundra = new HexType("tundra", Tiles.Tundra, 1);
        public static HexType Fog = new HexType("fog", Tiles.Fog, 1);
        
        //List<HexType> lands = new List<HexType>() { Plains };

        string _name;
        Tile _tile;
        int _movementPoints;
        
        public Tile Tile => _tile;
        public int MovementPoints => _movementPoints;
        
        public HexType(string name, Tile tile, int movementPoints)
        {
            this._name = name;
            this._tile = tile;
            this._movementPoints = movementPoints;
        }

        public static HexType GetHexType(HeightType height, MoistureType moisture, HeatType heat)
        {
            if (height == HeightType.Sand || height == HeightType.Grass || height == HeightType.Forest)
            {
                if (heat == HeatType.Coldest || heat == HeatType.Colder)
                    return Snow;
                
                if (heat == HeatType.Cold)
                {
                    if (moisture == MoistureType.Wettest || moisture == MoistureType.Wettest || moisture == MoistureType.Wet)
                        return Tundra;

                    if (moisture == MoistureType.Dry)
                        return Plains;
                    
                    return Snow;
                }

                if (heat == HeatType.Warm)
                {
                    if (moisture == MoistureType.Dryest || moisture == MoistureType.Dryer || moisture == MoistureType.Dry || moisture == MoistureType.Wet)
                        return Plains;

                    return Grassland;
                }
                
                if (heat == HeatType.Warmer)
                {
                    if (moisture == MoistureType.Dry || moisture == MoistureType.Dryer)
                        return Plains;

                    if (moisture == MoistureType.Wet || moisture == MoistureType.Wetter || moisture == MoistureType.Wettest)
                        return Grassland;
                    
                    return Desert;
                }
                return Desert;
            }

            if (height == HeightType.Snow || height == HeightType.Rock)
                return Mountains;
            
            if (height == HeightType.ShallowWater)
                return CoastLake;
            
            return Ocean;
        }
    }
}
