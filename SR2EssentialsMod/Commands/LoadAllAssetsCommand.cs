using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SR2E.Commands
{
    internal class LoadAllAssetsCommand : SR2Command
    {
        public override string ID => "loadall";

        public override string Usage => "DO NOT SHARE ANYTHING IN THIS COMMAND";

        public override string Description => "DO NOT SHARE ANYTHING IN THIS COMMAND!!!\nDO NOT SHARE ANYTHING IN THIS COMMAND!!!\nDO NOT SHARE ANYTHING IN THIS COMMAND!!!";

        public override bool Execute(string[] args)
        {
            var bundle = Get<AssetBundle>("1937414ef44dd74c104e9348d08dfa93.bundle");
            var assets = bundle.LoadAllAssets();
            var types = GameContext.Instance.AutoSaveDirector.identifiableTypes;
            foreach (var asset in assets)
            {
                if (asset.GetType().IsSubclassOf(typeof(IdentifiableType)))
                {
                    if (!types._memberTypes.Contains(asset.Cast<IdentifiableType>()))
                    {
                        types._memberTypes.Add(asset.Cast<IdentifiableType>());
                    }
                }
                Object.DontDestroyOnLoad(asset);
            }

            return true;
        }
    }
}
