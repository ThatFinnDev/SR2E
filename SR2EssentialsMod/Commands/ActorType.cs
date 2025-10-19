using SR2E.Components;
using SR2E.Enums;

namespace SR2E.Commands;

internal class ActorType : SR2ECommand
{
    public override string ID => "actortype";
    public override string Usage => "actortype <actor> [state}";
    public override CommandType type => CommandType.Cheat;
    
    List<string> disabledActors = new List<string>();
    public override List<string> GetAutoComplete(int argIndex, string[] args)
    {
        if (argIndex == 0) return LookupEUtil.GetStrongFilteredIdentifiableTypeStringListByPartialName(args==null?null:args[0], true,MAX_AUTOCOMPLETE.Get(),true);
        if (argIndex == 1) return new List<string> { "true", "false", "toggle" };
        return null;
    }
    public override bool Execute(string[] args)
    {
        if (!args.IsBetween(2, 2)) return SendUsage();
        if (!inGame) return SendLoadASaveFirst();
        if (args[0] == "*")
        {
            foreach (IdentifiableType loopedType in LookupEUtil.identifiableTypes)
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
            if (!TryParseTrool(args[1], out Trool troolAll)) return false;
            switch (troolAll)
            {
                case Trool.False: SendMessage(translation("cmd.actortype.disableall")); break;
                case Trool.True: SendMessage(translation("cmd.actortype.enableall")); break;
                case Trool.Toggle: SendMessage(translation("cmd.actortype.toggleeall")); break;
            }
            return true;
        }
        IdentifiableType type = LookupEUtil.GetIdentifiableTypeByName(args[0]);
        if (type == null) return SendNotValidIdentType(args[0]);
        if (type.isGadget()) return SendIsGadgetNotItem(type.GetName());
        bool enabled = !disabledActors.Contains(type.ReferenceId);
        if (!TryParseTrool(args[1], out Trool trool)) return false;
        switch (trool)
        {
            case Trool.False:
                if (!enabled) return SendError(translation("cmd.actortype.alreadydisabled", type.GetName()));
                disabledActors.Add(type.ReferenceId);
                foreach (var actor in Resources.FindObjectsOfTypeAll<IdentifiableActor>())
                    if (actor.identType == type) actor.gameObject.AddComponent<ObjectBlocker>();
                SendMessage(translation("cmd.actortype.disable",type.GetName()));
                return true;
            case Trool.True:
                if (enabled) return SendError(translation("cmd.actortype.alreadyenabled", type.GetName()));
                disabledActors.Remove(type.ReferenceId);
                foreach (var actor in Resources.FindObjectsOfTypeAll<IdentifiableActor>())
                    if (actor.identType == type) actor.gameObject.RemoveComponent<ObjectBlocker>();
                SendMessage(translation("cmd.actortype.enable",type.GetName()));
                return true;
            case Trool.Toggle:
                if (enabled)
                {
                    disabledActors.Add(type.ReferenceId);
                    foreach (var actor in Resources.FindObjectsOfTypeAll<IdentifiableActor>())
                        if (actor.identType == type) actor.gameObject.AddComponent<ObjectBlocker>();
                    SendMessage(translation("cmd.actortype.disable",type.GetName()));
                    return true;
                }
                disabledActors.Remove(type.ReferenceId);
                foreach (var actor in Resources.FindObjectsOfTypeAll<IdentifiableActor>())
                    if (actor.identType == type) actor.gameObject.RemoveComponent<ObjectBlocker>();
                SendMessage(translation("cmd.actortype.enable",type.GetName()));
                return true;
        }
        return SendUnknown();
    }
    public override void OnMainMenuUILoad()
    {
        disabledActors = new List<string>();
        foreach (var actor in Resources.FindObjectsOfTypeAll<IdentifiableActor>())
            actor.gameObject.RemoveComponent<ObjectBlocker>();
    }
    
}


