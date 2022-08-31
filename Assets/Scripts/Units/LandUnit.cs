using Assets.Scripts.Map;

namespace Assets.Scripts.Units
{
    abstract class LandUnit : Unit
    {
        protected LandUnit(int movement, Hex hex, World world) : base(movement, hex, world)
        {
            
        }
    }
}
