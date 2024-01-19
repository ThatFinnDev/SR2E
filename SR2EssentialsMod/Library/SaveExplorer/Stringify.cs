using Il2CppMonomiPark.SlimeRancher.Persist;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SR2E.Library.SaveExplorer
{
    internal static class Stringify
    {
        internal static string ToString(object obj)
        {
            if (obj is ActorDataV01)
                return (obj as ActorDataV01).ConvertToLocalized_OnlyString();
            else if (obj is Vector3V01)
                return (obj as ActorDataV01).ConvertToLocalized_OnlyString();
            else
                try
                {
                    obj.ToString();
                }
                catch
                {
                    return "Error";

                }
            return "Error";

        }
    }
}
