using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Il2CppMono.Security.X509.X520;

namespace SR2E.Commands.Library
{
    internal class RefreshEatmapCommand : SR2CCommand
    {
        public override string ID => "refresheatmap";

        public override string Usage => "refresheatmap [slime]";

        public override string Description => "Refreshes the eatmap of a slime(s)";

        public override bool Execute(string[] args)
        {
            if (args != null)
            {
                var slime = slimes.GetAllMembersArray().FirstOrDefault((IdentifiableType x) => x.name == args[0]);
                if (slime != null)
                    slime.Cast<SlimeDefinition>().RefreshEatmap();
                else return false;
            }
            else
            {
                foreach (var slime in slimes.GetAllMembersArray())
                {
                    slime.Cast<SlimeDefinition>().RefreshEatmap();
                }
            }
            return true;

        }
    }
}
