using Il2CppKinematicCharacterController;
using Il2CppMonomiPark.SlimeRancher.Player.CharacterController;
using System;
using System.Linq;
using Il2CppMonomiPark.SlimeRancher;
using Il2CppMonomiPark.SlimeRancher.Player.PlayerItems;
using Il2CppMonomiPark.SlimeRancher.Slime;
using SR2E.Components;
using SR2E.Enums;

namespace SR2E.Commands;

internal class UtilCommand : SR2ECommand
{
    public override string ID => "util";
    public override string Usage => "util <type> <parameter [value] [value2] [value3]";
    public override CommandType type => CommandType.Cheat;


    readonly List<string> TypeParam = new List<string>() { "GAME", "GORDO", "SLIME", "PLAYER" };
    readonly List<string> GordoParam = new List<string>() { "BASE_SIZE", "EATEN_COUNT", "PRINT_ID" };
    readonly List<string> SlimeParam = new List<string>() { "SLIME_HUNGER", "SLIME_AGI", "SLIME_FEAR","SLIME_SLEEPINESS", "USE_GRAVITY" };
    readonly List<string> GameParam = new List<string>() { "ACTOR_TYPE" };
    
    public override List<string> GetAutoComplete(int argIndex, string[] args)
    {
        if (argIndex == 0) return TypeParam;
        if (argIndex == 1)
            switch (args[0])
            {
                case "GORDO": return GordoParam;
                case "SLIME": return SlimeParam;
                case "GAME": return GameParam;
                default: return null;
            }
        if (argIndex == 2)
            switch (args[0])
            {
                case "GORDO": switch (args[1])
                    {
                        case "BASE_SIZE": return new List<string> { "0.25", "0.5", "1", "1.5", "2", "5" };
                        case "EATEN_COUNT": return new List<string> { "1", "5", "10", "45" };
                    }
                    return null;
                case "SLIME": switch (args[1])
                    {
                        case "SLIME_HUNGER": 
                        case "SLIME_AGI": 
                        case "SLIME_FEAR": 
                        case "SLIME_SLEEPINESS": return new List<string> { "0", "0.25", "0.5", "1" };
                        case "USE_GRAVITY": return new List<string> { "false", "true" };
                    }
                    return null;
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
            case "GORDO": return ExcGordo(args);
            case "SLIME": return ExcSlime(args);
            case "GAME": return ExcGame(args);
            default: return SendError(translation("cmd.util.invalidtype",args[0]));
        }
    }

    public bool ExcGordo(string[] cmd)
    {
        switch (cmd[1])
        {
            case "BASE_SIZE":
                if (cmd.Length == 2) return GordoSize(true);
                return GordoSize(false, cmd[2]);
            case "EATEN_COUNT":
                if (cmd.Length == 2) return GordoEatenAmount(true);
                return GordoEatenAmount(false, cmd[2]);
            case "PRINT_ID":
                return PrintGordoID();
            default: return SendError(translation("cmd.util.invalidparameter",cmd[1]));
        }
    }
    public bool ExcSlime(string[] cmd)
    {
        switch (cmd[1])
        {
            case "SLIME_HUNGER":
                if (cmd.Length == 2) return SlimeEmotion(true, SlimeEmotions.Emotion.HUNGER);
                return SlimeEmotion(false, SlimeEmotions.Emotion.HUNGER, cmd[2]);
            case "SLIME_AGI":
                if (cmd.Length == 2) return SlimeEmotion(true, SlimeEmotions.Emotion.AGITATION);
                return SlimeEmotion(false, SlimeEmotions.Emotion.AGITATION, cmd[2]);
            case "SLIME_FEAR":
                if (cmd.Length == 2) return SlimeEmotion(true, SlimeEmotions.Emotion.FEAR);
                return SlimeEmotion(false, SlimeEmotions.Emotion.FEAR, cmd[2]);
            case "SLIME_SLEEPINESS":
                if (cmd.Length == 2) return SlimeEmotion(true, SlimeEmotions.Emotion.SLEEPINESS);
                return SlimeEmotion(false, SlimeEmotions.Emotion.SLEEPINESS, cmd[2]);
            case "USE_GRAVITY":
                if (cmd.Length == 2) return ToggleActorZeroGrav(true);
                return ToggleActorZeroGrav(false,cmd[2]);
        }

        return false;
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

    public bool SlimeEmotion(bool isGet, SlimeEmotions.Emotion emotion, string valString = "1f")
    {
        
        Camera cam = Camera.main;
        if (cam == null) return SendError(translation("cmd.error.nocamera"));
        if (Physics.Raycast(new Ray(cam.transform.position, cam.transform.forward), out var hit))
        {
            var slime = hit.collider.gameObject.GetComponent<SlimeEmotions>();
            if (slime != null)
            {
                if (isGet)
                {
                    switch (emotion)
                    {
                        case SlimeEmotions.Emotion.FEAR:
                            SendMessage(translation("cmd.util.emotion.fear.show",
                                slime.gameObject.GetComponent<Identifiable>().identType.getName(), slime.Get(emotion)));
                            break;
                        case SlimeEmotions.Emotion.HUNGER:
                            SendMessage(translation("cmd.util.emotion.hunger.show",
                                slime.gameObject.GetComponent<Identifiable>().identType.getName(), slime.Get(emotion)));
                            break;
                        case SlimeEmotions.Emotion.AGITATION:
                            SendMessage(translation("cmd.util.emotion.agitation.show",
                                slime.gameObject.GetComponent<Identifiable>().identType.getName(), slime.Get(emotion)));
                            break;
                        case SlimeEmotions.Emotion.SLEEPINESS:
                            SendMessage(translation("cmd.util.emotion.sleepiness.show",
                                slime.gameObject.GetComponent<Identifiable>().identType.getName(), slime.Get(emotion)));
                            break;
                    }

                    return true;
                }
                else
                {
                    float parsed = float.Parse(valString);
                    slime.Set(emotion, parsed);
                    
                    switch (emotion)
                    {
                        case SlimeEmotions.Emotion.FEAR:
                            SendMessage(translation("cmd.util.emotion.fear.edit",
                                slime.gameObject.GetComponent<Identifiable>().identType.getName(), parsed));
                            break;
                        case SlimeEmotions.Emotion.HUNGER:
                            SendMessage(translation("cmd.util.emotion.hunger.edit",
                                slime.gameObject.GetComponent<Identifiable>().identType.getName(), parsed));
                            break;
                        case SlimeEmotions.Emotion.AGITATION:
                            SendMessage(translation("cmd.util.emotion.agitation.edit",
                                slime.gameObject.GetComponent<Identifiable>().identType.getName(), parsed));
                            break;
                        case SlimeEmotions.Emotion.SLEEPINESS:
                            SendMessage(translation("cmd.util.emotion.sleepiness.edit",
                                slime.gameObject.GetComponent<Identifiable>().identType.getName(), parsed));
                            break;
                    }

                    return true;
                }
            }

            return SendError(translation("cmd.error.notlookingatvalidobject"));
        }
        return SendError(translation("cmd.error.notlookingatanything"));
        
    }

    public bool ToggleActorZeroGrav(bool isGet, string gravity = "true")
    {
        bool newGravityState = false;
        string boolToParse = gravity.ToLower();
        if (boolToParse != "true" && boolToParse != "false") return SendError(translation("cmd.error.notvalidbool",gravity));
        if (boolToParse == "true")  newGravityState = true;
        Camera cam = Camera.main;
        if (cam == null)  return SendError(translation("cmd.error.nocamera"));
        
        if (Physics.Raycast(new Ray(cam.transform.position, cam.transform.forward), out var hit))
        {
            var actor = hit.collider.gameObject.GetComponent<Vacuumable>();
            if (actor != null)
            {
                if (isGet)
                {
                    if(actor._ignoresGravity) SendMessage(translation("cmd.util.actorgravity.showenable",actor._identifiable.identType.getName()));
                    else SendMessage(translation("cmd.util.actorgravity.showdisable",actor._identifiable.identType.getName()));
                    return true;
                }
                actor._ignoresGravity = !newGravityState;
                if(actor._ignoresGravity) SendMessage(translation("cmd.util.actorgravity.editenable",actor._identifiable.identType.getName()));
                else SendMessage(translation("cmd.util.actorgravity.editdisable",actor._identifiable.identType.getName()));
                return true;
            }
            return SendError(translation("cmd.error.notlookingatvalidobject"));
        }
        return SendError(translation("cmd.error.notlookingatanything"));
    }

    public override void OnMainMenuUILoad()
    {
        disabledActors = new List<string>();
    }

    public static LayerMask maskForGordo
    {
        get
        {
            LayerMask mask = ~0;
            mask &= ~(1 << Layers.GadgetPlacement);
            return mask;
        }
    }

    public bool GordoSize(bool isGet, string sizeString = "1f")
    {
        Camera cam = Camera.main;
        if (cam == null)  return SendError(translation("cmd.error.nocamera"));

        if (Physics.Raycast(new Ray(cam.transform.position, cam.transform.forward), out var hit,Mathf.Infinity,maskForGordo))
        {
            GordoIdentifiable gordo = hit.collider.gameObject.GetComponent<GordoIdentifiable>();
            GordoEat eat = hit.collider.gameObject.GetComponent<GordoEat>();
            if (gordo != null)
            {
                if (isGet)
                {
                    SendMessage(translation("com.util.gordosize.show",gordo.identType.getName(),eat._initScale/4));
                    return true;
                }
                float size = -1;
                try { size = float.Parse(sizeString); }
                catch { return SendNotValidFloat(sizeString); }
                if (size <= 0) return SendError(translation("cmd.error.notfloatabove",sizeString,0));
                eat._initScale = size*4;
                SendMessage(translation("com.util.gordosize.edit",gordo.identType.getName(),size));
                return true;
                
            }
            return SendError(translation("cmd.error.notlookingatvalidobject"));
        }
        return SendError(translation("cmd.error.notlookingatanything"));
    }

    public bool GordoEatenAmount(bool isGet, string amountString = "49")
    {
        
        Camera cam = Camera.main;
        if (cam == null) return SendError(translation("cmd.error.nocamera"));
        if (Physics.Raycast(new Ray(cam.transform.position, cam.transform.forward), out var hit,Mathf.Infinity,maskForGordo))
        {
            GordoIdentifiable gordo = hit.collider.gameObject.GetComponent<GordoIdentifiable>();
            GordoEat eat = hit.collider.gameObject.GetComponent<GordoEat>();
            if (gordo != null)
            {
                if (isGet)
                {
                    SendMessage(translation("com.util.gordosize.show",gordo.identType.getName(),eat.GetEatenCount()));
                    return true;
                }
                int amount = -1;
                try { amount = int.Parse(amountString); }
                catch { return SendNotValidInt(amountString); }
                if (amount <= 0) return SendError(translation("cmd.error.notintabove",amountString,0));
                eat.SetEatenCount(amount); 
                SendMessage(translation("com.util.gordosize.edit",gordo.identType.getName(),amount));

                return true;
            }
            return SendError(translation("cmd.error.notlookingatvalidobject"));
        }
        return SendError(translation("cmd.error.notlookingatanything"));
    }

    public bool PrintGordoID()
    {
        Camera cam = Camera.main;
        if (cam == null) return SendError(translation("cmd.error.nocamera"));
        if (Physics.Raycast(new Ray(cam.transform.position, cam.transform.forward), out var hit))
        {
            var gordo = hit.collider.gameObject.GetComponent<GordoEat>();
            if (gordo != null)
            {
                SendMessage($"This {gordo.SlimeDefinition.name.ToLower()} gordo\'s ID is {gordo._id}");
                return true;
            }
            return SendError(translation("cmd.error.notlookingatvalidobject"));
        }
        return SendError(translation("cmd.error.notlookingatanything"));
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


