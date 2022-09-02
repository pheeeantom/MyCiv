using World;
using Units;
using World;

namespace Units
{
    abstract class NavalUnit : Unit
    {
        public override int GetMovementPoints(HexType hexType)
        {
            return hexType == HexType.Ocean || hexType == HexType.CoastLake ? 1 : base.GetMovementPoints(hexType);
        }
    }
}
