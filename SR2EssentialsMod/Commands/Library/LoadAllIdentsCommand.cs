using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SR2E.Commands.Library
{
    internal class LoadAllIdentsCommand : SR2CCommand
    {
        public override string ID => "loadallidents";

        public override string Usage => "loadallidents";

        public override string Description => "Loads every identifiable";

        public override bool Execute(string[] args)
        {
            var group = Get<IdentifiableTypeGroup>("IdentifiableTypesGroup");
            var group2 = Get<IdentifiableTypeGroup>("PlortGroup");
            var group3 = Get<IdentifiableTypeGroup>("FruitGroup");
            var group4 = Get<IdentifiableTypeGroup>("VeggieGroup");
            var group5 = Get<IdentifiableTypeGroup>("MeatGroup");
            var group6 = Get<IdentifiableTypeGroup>("ResourceOreGroup");
            var group7 = Get<IdentifiableTypeGroup>("VaccableNonLiquids");
            foreach (var obj in Get<AssetBundle>("1937414ef44dd74c104e9348d08dfa93.bundle").LoadAllAssets())
            {
                var ident = obj.TryCast<IdentifiableType>();
                if (ident != null)
                {
                    if (!group.memberTypes.Contains(ident))
                    {
                        group.memberTypes.Add(ident);
                        group7.memberTypes.Add(ident);
                    }

                    if (ident.name.ToLower().Contains("plort"))
                        if (!group2.memberTypes.Contains(ident))
                            group2.memberTypes.Add(ident);

                    if (ident.name.ToLower().Contains("fruit"))
                        if (ident.TryCast<GadgetDefinition>() == null)
                            if (!group3.memberTypes.Contains(ident))
                            group3.memberTypes.Add(ident);

                    if (ident.name.ToLower().Contains("veggie"))
                        if (ident.TryCast<GadgetDefinition>() == null)
                            if (!group4.memberTypes.Contains(ident))
                            group4.memberTypes.Add(ident);

                    if (ident.name.ToLower().Contains("hen"))
                        if (ident.TryCast<GadgetDefinition>() == null)
                            if (!group5.memberTypes.Contains(ident))
                                group5.memberTypes.Add(ident);

                    if (ident.name.ToLower().Contains("craft"))
                        if (ident.TryCast<GadgetDefinition>() == null)
                            if (!group6.memberTypes.Contains(ident))
                                group6.memberTypes.Add(ident);

                    SR2Console.ExecuteByString("refresheatmap");
                }
            }
            return true;
        }
    }
}
