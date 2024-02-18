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
        internal static string DataToString(this object obj)
        {
            if (obj is ActorDataV01)
                return (obj as ActorDataV01).ConvertToLocalized_OnlyString();
            else if (obj is Vector3V01)
                return (obj as Vector3V01).ConvertToString();
            else if (obj is char)
            {
                var c = (char)obj;
                return c.ToString();
            }
            else if (obj is int)
            {
                var c = (int)obj; 
                return c.ToString();
            }
            else if (obj is float)
            {
                var c = (float)obj; 
                return c.ToString();
            }
            else if (obj is double)
            {
                var c = (double)obj; 
                return c.ToString();
            }
            else if (obj is string)
            {
                var c = (string)obj; 
                return c;
            }
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
