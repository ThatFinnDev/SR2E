using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SR2E
{
    public static class Credits
    {
        public static string TeamAsString()
        {
            string teamStr = "SR2E Team:";

            foreach (var item in team)
                teamStr += $"\n{item.Key} - {item.Value}";

            return teamStr;
        }
        private static Dictionary<string,string> team = new Dictionary<string, string>() 
        {
            {"ThatFinn", "Lead Developer"},
            {"PinkTarr", "Developer"},
            {"AureumApes", "Contributor"},
        };
    }
}
