using System.Linq;

namespace SR2E.Commands;

public class GiveUpgradeCommand : SR2Command
{
    public override string ID => "giveupgrade";
    public override string Usage => "giveupgrade <id>";
    
    public override bool Execute(string[] args)
    {
        if (!args.IsBetween(1,1)) return SendUsage();
        if (!inGame) return SendLoadASaveFirst();

        if (args[0] == "*")
        {
            UpgradeDefinition[] ids = Resources.FindObjectsOfTypeAll<UpgradeDefinition>();
            foreach (UpgradeDefinition id in ids) 
                for (var i = 0; i < 10; i++)
                    SceneContext.Instance.PlayerState._model.upgradeModel.IncrementUpgradeLevel(id);

            SendMessage(translation("cmd.giveupgrade.successall"));
            return true;
        }
        else
        {
            UpgradeDefinition id = Resources.FindObjectsOfTypeAll<UpgradeDefinition>()
                .FirstOrDefault(x => x.ValidatableName.Equals(args[0]));
            if (id == null)
                return SendError(translation("cmd.error.notvalidupgrade",args[0]));
               

            SceneContext.Instance.PlayerState._model.upgradeModel.IncrementUpgradeLevel(id);
            SendMessage(translation("cmd.giveupgrade.success",
                id.ValidatableName,SceneContext.Instance.PlayerState._model.upgradeModel.GetUpgradeLevel(id)));
            return true;
        }
    }

    public override List<string> GetAutoComplete(int argIndex, string[] args)
    {
        if (argIndex == 0)
        {
            var identifiableTypeGroup =
                Resources.FindObjectsOfTypeAll<UpgradeDefinition>().Select(x => x.ValidatableName);
            List<string> list = identifiableTypeGroup.ToList();
            list.Add("*");
            return list;
        }

        return null;
    }
}