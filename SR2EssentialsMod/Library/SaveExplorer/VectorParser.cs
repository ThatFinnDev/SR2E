using Il2CppMonomiPark.SlimeRancher.Persist;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SR2E.Library.SaveExplorer
{
    internal static class VectorParser
    {
        public static Vector3 ConvertToVector(this Vector3V01 saved) => saved.Value;
        public static string ConvertToString(this Vector3V01 saved) => $"{saved.Value.x} {saved.Value.y} {saved.Value.z}";
    }
}
