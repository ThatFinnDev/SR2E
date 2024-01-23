using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SR2E.Commands.Library
{
    internal class RemoveFromGroupCommand : SR2CCommand
    {
        public override string ID => "removefromgroup";

        public override string Usage => "removefromgroup <ident> <group>";

        public override string Description => "Removes a ident from a idgroup";

        public override bool Execute(string[] args)
        {
            Get<IdentifiableTypeGroup>(args[1]).memberTypes.Remove(GetAnyType(args[0]));
            return true;
        }
        public IdentifiableType GetAnyType(string name)
        {
            return Get<IdentifiableType>(name);
        }
        public override List<string> GetAutoComplete(int argIndex, string[] args)
        {
            List<string> result = new List<string>();
            if (argIndex == 1)
            {
                int i = 0;
                foreach (var group in Resources.FindObjectsOfTypeAll<IdentifiableTypeGroup>())
                {
                    if (i != 35)
                    {
                        result.Add(group.name);
                    }
                }
            }
            else if (argIndex == 0)
            {
                foreach (var ident in Resources.FindObjectsOfTypeAll<IdentifiableType>())
                {
                    result.Add(ident.name);
                }
            }

            return result;
        }
    }
}
