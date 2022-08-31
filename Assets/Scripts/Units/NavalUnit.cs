using Assets.Scripts.Map;

namespace Assets.Scripts.Units
{
    abstract class NavalUnit : Unit
    {
        protected NavalUnit(int movement, Hex hex, World world) : base(movement, hex, world)
        {
            
        }
        
        protected override int GetMovementPoints(HexType hexType)
        {
            return hexType == HexType.Ocean || hexType == HexType.CoastLake ? 1 : base.GetMovementPoints(hexType);
        }
    }
}
