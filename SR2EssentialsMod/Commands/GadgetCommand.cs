namespace SR2E.Commands;

public class GadgetCommand : SR2Command
{
    public override string ID => "gadget";
    public override string Usage => "gadget <add/set/remove/unlock/lock> <gadget> [amount]";
    //Lock is broken rn
    List<string> arg0List = new List<string> { "add","set","remove","get","unlock"/*, lock*/};
    public override List<string> GetAutoComplete(int argIndex, string[] args)
    {
        if (argIndex == 0)
            return arg0List;
        if (argIndex == 1)
            return getIdentListByPartialName(args == null ? null : args[0], false, true,true);
        if (argIndex == 2)
            if(args[0]!="get")
                return new List<string> { "1", "5", "10", "20", "30", "50" };
        return null;
    }

    public override bool Execute(string[] args)
    {
        if (!args.IsBetween(2,3)) return SendUsage();
        if (!inGame) return SendLoadASaveFirst();
        if(!arg0List.Contains(args[0]))
            return SendError(translation("cmd.gadget.notvalidoption",args[0]));
        
        int amount = 1;
        if (args.Length == 3)
        {
            if(args[0]=="get"||args[0]=="unlock"||args[0]=="lock") 
                return SendError(translation("cmd.gadget.errortoomanyargs",args[0])); 
            if (!int.TryParse(args[1], out amount)) return SendError(translation("cmd.error.notvalidint",args[2]));
            if (amount <= 0) return SendError(translation("cmd.error.notintabove",args[2],0));
        }
        else
        {
            switch (args[0])
            {
                case "add": amount = 1; break;
                case "set": amount = 0; break;
                case "remove": amount = -1; break;
            }
        }
        if (args[1] == "*")
        {
            bool isSilent = silent;
            foreach (GadgetDefinition def in Resources.FindObjectsOfTypeAll<GadgetDefinition>())
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
                    case "add":
                        SendMessage(translation("cmd.gadget.successalladd", amount));
                        break;
                    case "set":
                        SendMessage(translation("cmd.gadget.successallset", amount));
                        break;
                    case "remove":
                        SendMessage(translation("cmd.gadget.successallremove", amount));
                        break;
                    case "lock":
                        SendMessage(translation("cmd.gadget.successalllock"));
                        break;
                    case "unlock":
                        SendMessage(translation("cmd.gadget.successallunlock"));
                        break;
                }
            }
            return true;
        }
        string identifierTypeName = args[1];
        GadgetDefinition type = getGadgetDefByName(identifierTypeName);
        if(type==null) return SendError(translation("cmd.error.notvalidgadget",identifierTypeName));
        string itemName = type.getName();



        switch (args[0])
        {
            case "add": 
                SceneContext.Instance.GadgetDirector.AddItem(type, amount);
                SendMessage(translation("cmd.gadget.successadd",amount,itemName));
                break;
            case "set":
                SceneContext.Instance.GadgetDirector.AddItem(type, amount-SceneContext.Instance.GadgetDirector.GetItemCount(type));
                SendMessage(translation("cmd.gadget.successset",itemName,amount)); 
                break;
            case "remove":
                SceneContext.Instance.GadgetDirector.AddItem(type, -amount);
                SendMessage(translation("cmd.gadget.successremove",amount,itemName)); 
                break;
            case "get":
                SendMessage(translation("cmd.gadget.successget",SceneContext.Instance.GadgetDirector.GetItemCount(type),itemName)); 
                break;
            /*case "lock":
                if (!SceneContext.Instance.GadgetDirector.HasBlueprint(type))
                    return SendError(translation("cmd.gadget.errorlock",itemName));
                //This function doesn't exist  :(
                SceneContext.Instance.GadgetDirector.RemoveBlueprint(type);
                SendMessage(translation("cmd.gadget.successlock",itemName)); 
                break;*/
            case "unlock":
                if (SceneContext.Instance.GadgetDirector.HasBlueprint(type))
                    return SendError(translation("cmd.gadget.errorunlock",itemName));
                SceneContext.Instance.GadgetDirector.AddBlueprint(type);
                SendMessage(translation("cmd.gadget.successunlock",itemName)); 
                break;
        }

        return false;
    }
}
