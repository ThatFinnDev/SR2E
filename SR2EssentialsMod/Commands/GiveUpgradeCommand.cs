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
            if (args == null || args.Length > 1)
            {
                MelonLogger.Error("Incorrect number of arguments!");
                return false;
            }

            UpgradeDefinition id = Resources.FindObjectsOfTypeAll<UpgradeDefinition>().FirstOrDefault(x => x.ValidatableName.Equals(args[0]));
            if (id == null)
                throw new ArgumentException("This ID is incorrect");

            SceneContext.Instance.PlayerState._model.upgradeModel.IncrementUpgradeLevel(id);

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
