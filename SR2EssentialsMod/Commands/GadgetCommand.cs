﻿namespace SR2E.Commands;

internal class GadgetCommand : SR2ECommand
{
    public override string ID => "gadget";
    public override string Usage => "gadget <add/set/remove/unlock/get> <gadget> [amount/show popup(true/false)]";
    //Lock is broken rn
    List<string> arg0List = new List<string> { "add","set","remove","get","unlock"/*, lock*/};
    public override CommandType type => CommandType.Cheat;
    public override List<string> GetAutoComplete(int argIndex, string[] args)
    {
        if (argIndex == 0)
            return arg0List;
        if (argIndex == 1)
            return LookupEUtil.GetFilteredIdentifiableTypeStringListByPartialName(args == null ? null : args[1], false, MAX_AUTOCOMPLETE.Get(),true);
        if (argIndex == 2)
            if(args[0]!="get")
                return new List<string> { "1", "5", "10", "20", "30", "50" };
        return null;
    }

    public override bool Execute(string[] args)
    {
        if (!args.IsBetween(2,3)) return SendUsage();
        if (!inGame) return SendLoadASaveFirst();
        if(!arg0List.Contains(args[0])) return SendNotValidOption(args[0]);

        bool showPopup = true;
        int amount = 1;
        if (args.Length == 3)
        {
            if(args[0]=="get") return SendErrorToManyArgs(args[0]);
            if (args[0] == "unlock" || args[0] == "lock")
            {
                if (!TryParseBool(args[2], out showPopup)) return false;
            }
            else if(!TryParseInt(args[2], out amount,1,true)) return false;
        }
        else
            switch (args[0])
            {
                case "remove":
                case "add": amount = 1; break;
                case "set": amount = 0; break;
            }
        
        if (args[1] == "*")
        {
            bool isSilent = silent;
            foreach (GadgetDefinition def in Resources.FindObjectsOfTypeAll<GadgetDefinition>())
            {
                silent=isSilent?true:args[0]!="get";
                if(args.Length==3) Execute(new []{args[0], def.name, args[2]});
                else if(args[0]=="unlock") Execute(new []{args[0], def.name, "false"});
                else Execute(new []{args[0], def.name});
                silent=args[0]!="get";
            }
            silent = isSilent;
            switch (args[0])
            {
                case "add": SendMessage(translation("cmd.gadget.successalladd", amount)); break;
                case "set": SendMessage(translation("cmd.gadget.successallset", amount)); break;
                case "remove": SendMessage(translation("cmd.gadget.successallremove", amount)); break;
                case "lock": SendMessage(translation("cmd.gadget.successalllock")); break;
                case "unlock": SendMessage(translation("cmd.gadget.successallunlock")); break;
            }
            return true;
        }
        string identifierTypeName = args[1];
        GadgetDefinition type = LookupEUtil.GetGadgetDefinitionByName(identifierTypeName);
        if (type == null) return SendNotValidGadget(identifierTypeName);
        string itemName = type.GetName();

        switch (args[0])
        {
            case "remove":
            case "add": 
            case "set":
                int newValue = 0;
                int oldValue = SceneContext.Instance.GadgetDirector.GetItemCount(type);
                switch (args[0])
                {
                    case "set": newValue = amount; break;
                    case "add": newValue = amount + oldValue; break;
                    case "remove": newValue = oldValue - amount; break; 
                }
                newValue=Mathf.Clamp(newValue, 0, int.MaxValue);
                
                if (newValue > 0 && !SceneContext.Instance.GadgetDirector.HasBlueprint(type)) SceneContext.Instance.GadgetDirector.AddBlueprint(type);
                
                //Adding one updates the new value everywhere. Not doing can causes issues
                SceneContext.Instance.GadgetDirector._model.SetCount(type,newValue-1);
                SceneContext.Instance.GadgetDirector.AddItem(type,1);
                
                switch (args[0])
                {
                    case "set": SendMessage(translation("cmd.gadget.successset", itemName, amount)); break;
                    case "add": SendMessage(translation("cmd.gadget.successadd",amount,itemName)); break;
                    case "remove": SendMessage(translation("cmd.gadget.successremove",amount,itemName)); break; 
                }
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
                SceneContext.Instance.GadgetDirector.AddBlueprint(type,showPopup);
                SendMessage(translation("cmd.gadget.successunlock",itemName)); 
                break;
        }
        return false;
    }
}
