using System.Collections.Generic;

namespace SR2E
{
    public static class SR2Warps
    {
        internal static Dictionary<string,Warp> warps = new Dictionary<string,Warp>();
    }

    internal class Warp
    {
        internal string sceneGroup;
        internal float x;
        internal float y;
        internal float z;

        internal Warp(string sceneGroup, float x, float y, float z)
        {
            this.sceneGroup = sceneGroup;
            this.x = x;
            this.y = y;
            this.z = z;
        }
    }
}