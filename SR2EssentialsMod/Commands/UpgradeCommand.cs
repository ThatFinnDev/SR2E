using System.Linq;

namespace SR2E.Commands;

public class UpgradeCommand : SR2Command
{
    public override string ID => "upgrade";
    public override string Usage => "upgrade <increment/set/decrement/unlock/lock> <id> [amount]";
    List<string> arg0List = new List<string> { "add","set","remove","get"};
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
            if (!int.TryParse(args[1], out level)) return SendError(translation("cmd.error.notvalidint",args[2]));
            if (level <= 0) return SendError(translation("cmd.error.notintabove",args[2],0));
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
                Execute(new []{args[0], def.name, args[3]});
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
        UpgradeDefinition id = Resources.FindObjectsOfTypeAll<UpgradeDefinition>().FirstOrDefault(x => x.name.Equals(args[0]));
        if(id==null) return SendError(translation("cmd.error.notvalidupgrade",identifierTypeName));
        string itemName = id.name;

        
        switch (args[0])
        {
            case "increment": 
                SceneContext.Instance.PlayerState._model.upgradeModel.SetUpgradeLevel(id,
                    SceneContext.Instance.PlayerState._model.upgradeModel.GetUpgradeLevel(id)+level);
                SendMessage(translation("cmd.upgrade.successadd",itemName,level));
                break;
            case "set":
                SceneContext.Instance.PlayerState._model.upgradeModel.SetUpgradeLevel(id,level);
                SendMessage(translation("cmd.upgrade.successset",itemName,level)); 
                break;
            case "decrement":
                SceneContext.Instance.PlayerState._model.upgradeModel.SetUpgradeLevel(id,
                    SceneContext.Instance.PlayerState._model.upgradeModel.GetUpgradeLevel(id)-level);
                SendMessage(translation("cmd.upgrade.successremove",itemName,level)); 
                break;
            case "get":
                SendMessage(translation("cmd.upgrade.successget",SceneContext.Instance.PlayerState._model.upgradeModel.GetUpgradeLevel(id),itemName)); 
                break;
        }

        return false;
    }
}
