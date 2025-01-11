using SR2E.Components;
using SR2E.Enums;

namespace SR2E.Commands;

internal class UtilCommand : SR2ECommand
{
    public override string ID => "util";
    public override string Usage => "util <type> <parameter [value] [value2] [value3]";
    public override CommandType type => CommandType.DontLoad;

    readonly List<string> TypeParam = new List<string>() { "GAME", };
    readonly List<string> GameParam = new List<string>() { "ACTOR_TYPE" };
    
    public override List<string> GetAutoComplete(int argIndex, string[] args)
    {
        if (argIndex == 0) return TypeParam;
        if (argIndex == 1)
            switch (args[0])
            {
                case "GAME": return GameParam;
                default: return null;
            }
        if (argIndex == 2)
            switch (args[0])
            {
                case "GAME": switch (args[1])
                    {
                        case "ACTOR_TYPE": return getIdentListByPartialName(args[2], true,true,true);
                    }
                    return null;
            }
        
        if (argIndex == 3)
            switch (args[0])
            {
                case "GAME": switch (args[1])
                    {
                        case "ACTOR_TYPE": return new List<string> { "true", "false", "toggle" };
                    }
                    return null;
            }
        return null;
    }
    public override bool Execute(string[] args)
    {
        if (!args.IsBetween(2, 5)) return SendUsage();
        if (!inGame) return SendLoadASaveFirst();
        switch (args[0])
        {
            case "GAME": return ExcGame(args);
            default: return SendError(translation("cmd.util.invalidtype",args[0]));
        }
    }
    public bool ExcGame(string[] cmd)
    {
        switch (cmd[1])
        {
            case "ACTOR_TYPE":
                if (cmd.Length == 2) return SendError(translation("cmd.util.requiresmore", cmd[1]));
                if (cmd.Length == 3) return ToggleActorType(true,cmd[2]);
                return ToggleActorType(false,cmd[2],cmd[3]);
            default: return false;
        }
    }
    public override void OnMainMenuUILoad()
    {
        disabledActors = new List<string>();
    }
    
    public List<string> disabledActors = new List<string>();
    public bool ToggleActorType(bool isGet, string identName, string action = ".") 
    {
        if (identName == "*")
        {
            foreach (IdentifiableType loopedType in identifiableTypes)
            {
                bool enabledLoop = !disabledActors.Contains(loopedType.ReferenceId);
                if (enabledLoop)
                {
                    disabledActors.Add(loopedType.ReferenceId);
                    foreach (var actor in Resources.FindObjectsOfTypeAll<IdentifiableActor>())
                        if (actor.identType == loopedType) actor.gameObject.AddComponent<ObjectBlocker>();
                }
                disabledActors.Remove(loopedType.ReferenceId);
                foreach (var actor in Resources.FindObjectsOfTypeAll<IdentifiableActor>())
                    if (actor.identType == loopedType) actor.gameObject.RemoveComponent<ObjectBlocker>();
            }

            return true;
        }
        IdentifiableType type = getIdentByName(identName);
        if (type == null) return SendError(translation("cmd.error.notvalididenttype", identName));
        if (type.isGadget()) return SendIsGadgetNotItem(type.getName());
        bool enabled = !disabledActors.Contains(type.ReferenceId);
        if (isGet)
        {
            if(enabled) SendMessage(translation("cmd.util.actor.showenable",type.getName()));
            else SendMessage(translation("cmd.util.actor.showdisable",type.getName()));
            return true;
        }

        Trool trool = Trool.False;
        string boolToParse = action.ToLower();
        if (boolToParse != "true" && boolToParse != "false" && boolToParse != "toggle") return SendError(translation("cmd.error.notvalidtrool",action));
        if (boolToParse == "true") trool = Trool.True;
        if (boolToParse == "toggle") trool = Trool.Toggle;
        switch (trool)
        {
            case Trool.False:
                if (!enabled) return SendError(translation("cmd.util.actor.alreadydisabled", type.getName()));
                disabledActors.Add(type.ReferenceId);
                foreach (var actor in Resources.FindObjectsOfTypeAll<IdentifiableActor>())
                    if (actor.identType == type) actor.gameObject.AddComponent<ObjectBlocker>();
                SendMessage(translation("cmd.util.actor.editdisable",type.getName()));
                return true;
            case Trool.True:
                if (enabled) return SendError(translation("cmd.util.actor.alreadyenabled", type.getName()));
                disabledActors.Remove(type.ReferenceId);
                foreach (var actor in Resources.FindObjectsOfTypeAll<IdentifiableActor>())
                    if (actor.identType == type) actor.gameObject.RemoveComponent<ObjectBlocker>();
                SendMessage(translation("cmd.util.actor.editenable",type.getName()));
                return true;
            case Trool.Toggle:
                if (enabled)
                {
                    disabledActors.Add(type.ReferenceId);
                    foreach (var actor in Resources.FindObjectsOfTypeAll<IdentifiableActor>())
                        if (actor.identType == type) actor.gameObject.AddComponent<ObjectBlocker>();
                    SendMessage(translation("cmd.util.actor.editdisable",type.getName()));
                    return true;
                }
                disabledActors.Remove(type.ReferenceId);
                foreach (var actor in Resources.FindObjectsOfTypeAll<IdentifiableActor>())
                    if (actor.identType == type) actor.gameObject.RemoveComponent<ObjectBlocker>();
                SendMessage(translation("cmd.util.actor.editenable",type.getName()));
                return true;
        }
        return false;

    }
}


