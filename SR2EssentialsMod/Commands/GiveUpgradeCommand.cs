﻿using System;
using System.Linq;

namespace SR2E.Commands
{
    public class GiveUpgradeCommand : SR2CCommand
    {
        public override string ID => "giveupgrade";

        public override string Usage => "giveupgrade <id>";

        public override string Description => "Gives an upgrade";

        public override bool Execute(string[] args)
        {
            if (args == null) { SR2Console.SendMessage($"Usage: {Usage}"); return false; }
            if (args.Length != 1) { SR2Console.SendMessage($"Usage: {Usage}"); return false; }
            if (SceneContext.Instance == null) { SR2Console.SendError("Load a save first!"); return false; }
            if (SceneContext.Instance.PlayerState == null) { SR2Console.SendError("Load a save first!"); return false; }

            if (args[0] == "*")
            {
                UpgradeDefinition[] ids = Resources.FindObjectsOfTypeAll<UpgradeDefinition>();
                foreach (UpgradeDefinition id in ids)
                {
                    for (var i = 0; i < 10; i++)
                    {
                        SceneContext.Instance.PlayerState._model.upgradeModel.IncrementUpgradeLevel(id);
                    }
                }
                SR2Console.SendMessage("Upgraded all upgrades!");
                return true;
            }
            else
            {
                UpgradeDefinition id = Resources.FindObjectsOfTypeAll<UpgradeDefinition>().FirstOrDefault(x => x.ValidatableName.Equals(args[0]));
                if (id == null)
                { SR2Console.SendError(args[0] + " is not a valid Upgrade!"); return false; }

                SceneContext.Instance.PlayerState._model.upgradeModel.IncrementUpgradeLevel(id);
                SR2Console.SendMessage($"{id.ValidatableName} is now on level {SceneContext.Instance.PlayerState._model.upgradeModel.GetUpgradeLevel(id)}");

                return true;
            }
            SR2Console.SendError("An unknown error occured!");
            return false;
        }

        public override List<string> GetAutoComplete(int argIndex, string[] args)
        {
            if (argIndex == 0)
            {
                var identifiableTypeGroup =
                    Resources.FindObjectsOfTypeAll<UpgradeDefinition>().Select(x => x.ValidatableName);
                List<string > list = identifiableTypeGroup.ToList();
                list.Add("*");
                return list;
            }

            return null;
        }
    }
}
