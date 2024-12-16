using System.Linq;

namespace SR2E.Commands;

public class UpgradeCommand : SR2Command
{
    public override string ID => "upgrade";
    public override string Usage => "upgrade <increment/set/decrement/get> <id> [amount]";
    List<string> arg0List = new List<string> { "increment","set","decrement","get"};
    public override List<string> GetAutoComplete(int argIndex, string[] args)
    {
        if (argIndex == 0)
            return arg0List;
        if (argIndex == 1)
        {
            var identifiableTypeGroup =
                Resources.FindObjectsOfTypeAll<UpgradeDefinition>().Select(x => x.name);
            List<string> list = identifiableTypeGroup.ToList();
            list.Add("*");
            return list;
        }
        if (argIndex == 2)
            if(args[0]!="get")
                return new List<string> { "0", "1", "2", "3", "4", "5", "6" };
        return null;
    }

    public override bool Execute(string[] args)
    {
        if (!args.IsBetween(2,3)) return SendUsage();
        if (!inGame) return SendLoadASaveFirst();
        if(!arg0List.Contains(args[0]))
            return SendError(translation("cmd.upgrade.notvalidoption",args[0]));

        
        int level = 1;
        if (args.Length == 3)
        {
            if(args[0]=="get") return SendError(translation("cmd.upgrade.errortoomanyargs",args[0])); 
            if (!int.TryParse(args[2], out level)) return SendError(translation("cmd.error.notvalidint",args[2]));
            if (level <= -1) return SendError(translation("cmd.error.notintabove",args[2],-1));
        }
        else switch (args[0])
        {
            case "increment": level = 1; break;
            case "set": level = 0; break;
            case "decrement": level = -1; break;
        }

        if (args[1] == "*")
        {
            bool isSilent = silent;
            foreach (UpgradeDefinition def in Resources.FindObjectsOfTypeAll<UpgradeDefinition>())
            {
                silent=isSilent?true:args[0]!="get";
                if(args.Length==3) Execute(new []{args[0], def.name, args[2]});
                else Execute(new []{args[0], def.name});
                silent=args[0]!="get";
            }
            if(silent)
            {
                silent = isSilent;
                switch (args[0])
                {
                    case "increment":
                        SendMessage(translation("cmd.upgrade.successalladd", level));
                        break;
                    case "set":
                        SendMessage(translation("cmd.upgrade.successallset", level));
                        break;
                    case "decrement":
                        SendMessage(translation("cmd.upgrade.successallremove", level));
                        break;
                }
            }
            return true;
        }
        string identifierTypeName = args[1];
        UpgradeDefinition id = Resources.FindObjectsOfTypeAll<UpgradeDefinition>().FirstOrDefault(x => x.name.Equals(identifierTypeName));
        if(id==null) return SendError(translation("cmd.error.notvalidupgrade",identifierTypeName));
        string itemName = id.name;
        int maxUpgrade = 0;
        bool continueTesting = true;
        while (continueTesting)
        {
            if (id.UpgradeLevelExist(maxUpgrade+1)) maxUpgrade++;
            else continueTesting = false;
        }

        int newLevel = 0;
        switch (args[0])
        {
            case "increment":
                newLevel = SceneContext.Instance.PlayerState._model.upgradeModel.GetUpgradeLevel(id) + level;
                if(newLevel > maxUpgrade) newLevel = maxUpgrade;
                SceneContext.Instance.PlayerState._model.upgradeModel.SetUpgradeLevel(id, newLevel);
                if(newLevel==maxUpgrade) SendMessage(translation("cmd.upgrade.successset",itemName,newLevel));
                else SendMessage(translation("cmd.upgrade.successadd",itemName,level));
                break;
            case "set":
                newLevel = level;
                if(newLevel > maxUpgrade) newLevel = maxUpgrade;
                SceneContext.Instance.PlayerState._model.upgradeModel.SetUpgradeLevel(id, newLevel);
                SendMessage(translation("cmd.upgrade.successset",itemName,newLevel)); 
                break;
            case "decrement":
                newLevel = Mathf.Clamp((SceneContext.Instance.PlayerState._model.upgradeModel.GetUpgradeLevel(id) - level), 0, int.MaxValue);
                if(newLevel < 0) newLevel = 0;
                SceneContext.Instance.PlayerState._model.upgradeModel.SetUpgradeLevel(id, newLevel);
                if(newLevel==maxUpgrade) SendMessage(translation("cmd.upgrade.successset",itemName,newLevel));
                else SendMessage(translation("cmd.upgrade.successremove",itemName,level));
                break;
            case "get":
                SendMessage(translation("cmd.upgrade.successget",SceneContext.Instance.PlayerState._model.upgradeModel.GetUpgradeLevel(id),itemName)); 
                break;
        }

        return false;
    }

    public System.ArgumentOutOfRangeException f;
}
