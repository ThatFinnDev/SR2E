namespace SR2E.Commands;

public class GiveBlueprintCommand : SR2Command
{
    public override string ID => "giveblueprint";
    public override string Usage => "giveblueprint <blueprint>";

    public override List<string> GetAutoComplete(int argIndex, string[] args)
    {
        if (argIndex == 0)
            return getIdentListByPartialName(args == null ? null : args[0], false, true);
        return null;
    }

    public override bool Execute(string[] args)
    {
        if (!args.IsBetween(1,1)) return SendUsage();
        if (!inGame) return SendLoadASaveFirst();



        string identifierTypeName = args[0];
        GadgetDefinition type = getGadgetDefByName(identifierTypeName);
        if(type==null) return SendError(translation("cmd.error.notvalidgadget",identifierTypeName));
        string bluePrintName = type.getName();



        if (SceneContext.Instance.GadgetDirector.HasBlueprint(type))
            return SendError(translation("cmd.giveblueprint.alreadyowned",bluePrintName));
        
        SceneContext.Instance.GadgetDirector.AddBlueprint(type);
        SendMessage(translation("cmd.giveblueprint.success",bluePrintName));

        return true;
    }
}
    
