using System;
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

            UpgradeDefinition id = Resources.FindObjectsOfTypeAll<UpgradeDefinition>().FirstOrDefault(x => x.ValidatableName.Equals(args[0]));
            if (id == null)
            { SR2Console.SendError(args[0] + " is not a valid Upgrade!"); return false; }

            SceneContext.Instance.PlayerState._model.upgradeModel.IncrementUpgradeLevel(id);
            SR2Console.SendMessage($"{id.ValidatableName} is now on level {SceneContext.Instance.PlayerState._model.upgradeModel.GetUpgradeLevel(id)}");
            return true;
        }

        public override List<string> GetAutoComplete(int argIndex, string[] args)
        {
            if (argIndex == 0)
            {
                var identifiableTypeGroup =
                    Resources.FindObjectsOfTypeAll<UpgradeDefinition>().Select(x => x.ValidatableName);
                return identifiableTypeGroup.ToList();
            }

            return null;
        }
    }
}
