using MapModeFramework;

namespace VC_Territory
{
    public class WorldLayer_MapMode_Territory : WorldLayer_MapMode_Region
    {
        public static WorldLayer_MapMode_Territory Instance;

        public WorldLayer_MapMode_Territory() : base()
        {
            Instance = this;
        }
    }
}
