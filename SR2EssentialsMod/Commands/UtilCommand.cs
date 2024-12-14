using Il2CppKinematicCharacterController;
using Il2CppMonomiPark.SlimeRancher.Player.CharacterController;
using System;
using System.Linq;
using Il2CppMonomiPark.SlimeRancher;
using Il2CppMonomiPark.SlimeRancher.Player.PlayerItems;
using Il2CppMonomiPark.SlimeRancher.Slime;
using Il2CppMonomiPark.SlimeRancher.World;
using Unity.Mathematics;

namespace SR2E.Commands;

public class UtilCommand : SR2Command
{
    public override string ID => "util";
    public override string Usage => "util <type> <parameter [value] [value2] [value3]";

    public const float playerColliderHeightBase = 2f;
    public const float playerColliderRadBase = 0.6f;

    readonly List<string> TypeParam = new List<string>() { "GAME", "GORDO", "SLIME", "PLAYER", "GADGET" };
    readonly List<string> GordoParam = new List<string>() { "BASE_SIZE", "EATEN_COUNT", "PRINT_ID" };
    readonly List<string> SlimeParam = new List<string>() { "SLIME_HUNGER", "SLIME_AGI", "SLIME_FEAR","SLIME_SLEEPINESS", "USE_GRAVITY" };
    readonly List<string> GameParam = new List<string>() { "ACTOR_TYPE" };
    readonly List<string> PlayerParam = new List<string>() { "CUSTOM_SIZE", "GRAVITY_LEVEL", "VAC_MODE" };
    readonly List<string> GadgetParam = new List<string>() { "ROTATION", "POSITION" };
    
    public override List<string> GetAutoComplete(int argIndex, string[] args)
    {
        if (argIndex == 0) return TypeParam;
        if (argIndex == 1)
            switch (args[0])
            {
                case "GORDO": return GordoParam;
                case "SLIME": return SlimeParam;
                case "GAME": return GameParam;
                case "PLAYER": return PlayerParam;
                case "GADGET": return GadgetParam;
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
                case "PLAYER": switch (args[1])
                    {
                        case "CUSTOM_SIZE": return new List<string> { "0.25", "0.5", "1", "1.5", "2", "5" };
                        case "GRAVITY_LEVEL": return new List<string> { "-2", "-1", "1", "2", "5" };
                        case "VAC_MODE": return Enum.GetNames(typeof(VacModes)).ToList();
                    }
                    return null;
                case "GADGET": 
                    switch (args[1])
                    {
                        case "POSITION": return new List<string> { "0", "1" };
                        case "ROTATION": return new List<string> { "0", "90", "180", "270" };
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
                case "GADGET": 
                    switch (args[1])
                    {
                        case "POSITION": return new List<string> { "0", "1" };
                        case "ROTATION": return new List<string> { "0", "90", "180", "270" };
                    }
                    return null;
            }
        if (argIndex == 4)
            switch (args[0])
            {
                case "GADGET": 
                    switch (args[1])
                    {
                        case "POSITION": return new List<string> { "0", "1" };
                        case "ROTATION": return new List<string> { "0", "90", "180", "270" };
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
            case "PLAYER": return ExcPlayer(args);
            case "GADGET": return ExcGadget(args);
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
    public bool ExcPlayer(string[] cmd)
    {
        switch (cmd[1])
        {
            case "CUSTOM_SIZE":
                if (cmd.Length > 2) return PlayerSize(false, cmd[2]);
                return PlayerSize(true);
            case "GRAVITY_LEVEL":
                if (cmd.Length > 2) return PlayerGravity(false, cmd[2]);
                return PlayerGravity(true);
            case "VAC_MODE":
                if (cmd.Length > 2) return PlayerVacModeSet(false,silent,cmd[2]);
                return PlayerVacModeSet(true,silent);
            default: return false;
        }
    }

    public bool ExcGadget(string[] cmd)
    {
        switch (cmd[1])
        {
            case "POSITION":
                if (cmd.Length == 1) return GadgetPos(true);
                if (cmd.Length == 5) return GadgetPos(false, cmd[2], cmd[3], cmd[4]);
                return SendError(translation("cmd.util.requiresmore", cmd[1]));
            case "ROTATION":
                if (cmd.Length == 1) return GadgetRot(true);
                if (cmd.Length == 5) return GadgetRot(false, cmd[2], cmd[3], cmd[4]);
                return SendError(translation("cmd.util.requiresmore", cmd[1]));
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
                catch { return SendError(translation("cmd.error.notvalidfloat",sizeString)); }
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
                catch { return SendError(translation("cmd.error.notvalidint",amountString)); }
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
        IdentifiableType type = getIdentByName(identName);
        if (type == null) return SendError(translation("cmd.error.notvalididenttype", identName));
        if (type.isGadget()) return SendError(translation("cmd.give.isgadgetnotitem",type.getName()));
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


    public bool PlayerSize(bool isGet, string sizeString = "1")
    {
        if (isGet)
        {
            SendMessage(translation("cmd.util.player.size.show",SceneContext.Instance.player.transform.localScale.x));
            return true;
        }

        int size = -1;
        try { size = int.Parse(sizeString); }
        catch { return SendError(translation("cmd.error.notvalidint",sizeString)); }
        if (size <= 0) return SendError(translation("cmd.error.notintabove",sizeString,0));
        
        
        KinematicCharacterMotor KCC = null;
        
        try { KCC = SceneContext.Instance.player.GetComponent<KinematicCharacterMotor>(); }
        catch { return SendError(translation("cmd.error.kinematiccharactermotornull"));}
        
        SceneContext.Instance.player.transform.localScale = Vector3.one * size;
        KCC.CapsuleHeight = playerColliderHeightBase * size;
        KCC.CapsuleRadius = playerColliderRadBase * size;
        //SR2ESavableDataV2.Instance.playerSavedData.size = size;
        SendMessage(translation("cmd.util.player.size.edit",size));
        return true;

    }

    public static void RemoteExc_PlayerSize(float size)
    {
        try
        {

            SceneContext.Instance.player.transform.localScale = Vector3.one * size;
            var KCC = SceneContext.Instance.player.GetComponent<KinematicCharacterMotor>();
            KCC.CapsuleHeight = playerColliderHeightBase * size;
            KCC.CapsuleRadius = playerColliderRadBase * size;
        }
        catch { }
    }

    public bool PlayerGravity(bool isGet, string levelString = "1")
    {
        
        SRCharacterController SRCC = null;
        
        try { SRCC = SceneContext.Instance.player.GetComponent<SRCharacterController>(); }
        catch { return SendError(translation("cmd.error.srccnull"));}
        if (isGet)
        {
            SendMessage(translation("cmd.util.player.gravity.show",SRCC._gravityMagnitude.Value));
            return true;
        }
        
        float level = -1;
        try { level = float.Parse(levelString); }
        catch { return SendError(translation("cmd.error.notvalidfloat",levelString)); }
        
        SRCC._gravityMagnitude = new Il2CppSystem.Nullable<float>(level);
        //SR2ESavableDataV2.Instance.playerSavedData.gravityLevel = level;
        SendMessage(translation("cmd.util.player.gravity.edit",level));
        return true;

    }
    
    public bool GadgetPos(bool isGet, string xString = "0", string yString = "0", string zString = "0")
    {
        Camera cam = Camera.main;
        if (cam == null)  return SendError(translation("cmd.error.nocamera"));
        if (Physics.Raycast(new Ray(cam.transform.position, cam.transform.forward), out var hit,Mathf.Infinity,maskForGordo))
        {
            Gadget gadget = hit.collider.gameObject.GetComponentInParent<Gadget>();
            if (gadget != null)
            {
                if (isGet)
                {
                    var pos = gadget.transform.position;
                    SendMessage(translation("cmd.util.gadget.pos.show",gadget.identType.getName(),pos.x,pos.y,pos.z));
                    return true;
                }
                Vector3 vector3;
                try { vector3 = new Vector3(-float.Parse(xString), -float.Parse(yString), -float.Parse(zString)); }
                catch { return SendError(translation("cmd.error.notvalidvector3",xString,yString,zString)); }
                
                gadget.transform.position = vector3;
                gadget._model.lastPosition = vector3;
                SendMessage(translation("cmd.util.gadget.pos.edit",gadget.identType.getName(),vector3.x,vector3.y,vector3.z));
                return true;
                
            }
            return SendError(translation("cmd.error.notlookingatvalidobject"));
        }
        return SendError(translation("cmd.error.notlookingatanything"));
    }

    public bool GadgetRot(bool isGet, string xString = "0", string yString = "0", string zString = "0")
    {
        Camera cam = Camera.main;
        if (cam == null)  return SendError(translation("cmd.error.nocamera"));
        if (Physics.Raycast(new Ray(cam.transform.position, cam.transform.forward), out var hit,Mathf.Infinity,maskForGordo))
        {
            Gadget gadget = hit.collider.gameObject.GetComponentInParent<Gadget>();
            if (gadget != null)
            {
                if (isGet)
                {
                    var pos = gadget.transform.rotation.eulerAngles;
                    SendMessage(translation("cmd.util.gadget.rot.show",gadget.identType.getName(),pos.x,pos.y,pos.z));
                    return true;
                }
                Vector3 vector3;
                try { vector3 = new Vector3(-float.Parse(xString), -float.Parse(yString), -float.Parse(zString)); }
                catch { return SendError(translation("cmd.error.notvalidvector3",xString,yString,zString)); }
                
                gadget.transform.rotation = Quaternion.Euler(vector3);
                gadget._model.eulerRotation = vector3;
                SendMessage(translation("cmd.util.gadget.rot.edit",gadget.identType.getName(),vector3.x,vector3.y,vector3.z));
                return true;
                
            }
            return SendError(translation("cmd.error.notlookingatvalidobject"));
        }
        return SendError(translation("cmd.error.notlookingatanything"));
    }


    private static VacModes currVacMode;
    public static bool PlayerVacModeSet(bool isGet,bool silent,string modeString = ".")
    {
        if (isGet)
        {
            if(!silent) SR2EConsole.SendMessage(translation("cmd.util.vacmode.show",currVacMode.ToString().Replace("VacModes","")));
            return true;
        }
        VacModes mode;
        try { mode = Enum.Parse<VacModes>(modeString); }
        catch { if (!silent) SR2EConsole.SendError(translation("cmd.error.notvalidvacmode", modeString)); return false; }
        
        
        if (mode == VacModes.NORMAL)
        {
            SceneContext.Instance.PlayerState.VacuumItem.gameObject.SetActive(true);
            SceneContext.Instance.PlayerState.VacuumItem._vacMode = VacuumItem.VacMode.NONE;
            SceneContext.Instance.Camera.RemoveComponent<FlingMode>();
            SceneContext.Instance.Camera.RemoveComponent<IdentifiableObjectDragger>();
        }
        else if (mode == VacModes.AUTO_VAC)
        {
            SceneContext.Instance.PlayerState.VacuumItem.gameObject.SetActive(true);
            SceneContext.Instance.PlayerState.VacuumItem._vacMode = VacuumItem.VacMode.VAC;
            SceneContext.Instance.Camera.RemoveComponent<FlingMode>();
            SceneContext.Instance.Camera.RemoveComponent<IdentifiableObjectDragger>();
        }
        else if (mode == VacModes.AUTO_SHOOT)
        {
            SceneContext.Instance.PlayerState.VacuumItem.gameObject.SetActive(true);
            SceneContext.Instance.PlayerState.VacuumItem._vacMode = VacuumItem.VacMode.SHOOT;
            SceneContext.Instance.Camera.RemoveComponent<FlingMode>();
            SceneContext.Instance.Camera.RemoveComponent<IdentifiableObjectDragger>();
        }
        else if (mode == VacModes.NONE)
        {
            SceneContext.Instance.PlayerState.VacuumItem._vacMode = VacuumItem.VacMode.NONE;

            MelonCoroutines.Start(waitForSeconds(1.5f));

            SceneContext.Instance.Camera.RemoveComponent<IdentifiableObjectDragger>();
            SceneContext.Instance.Camera.RemoveComponent<FlingMode>();
            SceneContext.Instance.PlayerState.VacuumItem.gameObject.SetActive(false);
        }
        else if (mode == VacModes.DRAG)
        {
            SceneContext.Instance.PlayerState.VacuumItem._vacMode = VacuumItem.VacMode.NONE;

            MelonCoroutines.Start(waitForSeconds(1.5f));

            SceneContext.Instance.PlayerState.VacuumItem.gameObject.SetActive(false);
            SceneContext.Instance.Camera.RemoveComponent<FlingMode>();
            SceneContext.Instance.Camera.AddComponent<IdentifiableObjectDragger>();
        }
        else if (mode == VacModes.LAUNCH)
        {
            SceneContext.Instance.PlayerState.VacuumItem._vacMode = VacuumItem.VacMode.NONE;

            MelonCoroutines.Start(waitForSeconds(1.5f));

            SceneContext.Instance.PlayerState.VacuumItem.gameObject.SetActive(false);
            SceneContext.Instance.Camera.AddComponent<FlingMode>();
            SceneContext.Instance.Camera.RemoveComponent<IdentifiableObjectDragger>();
        }

        //SR2ESavableDataV2.Instance.playerSavedData.vacMode = mode;
        currVacMode = mode;
        if(!silent) SR2EConsole.SendMessage(translation("cmd.util.vacmode.success",mode.ToString().Replace("VacModes","")));
        return true;
    }

    static System.Collections.IEnumerator waitForSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
    }

    public override void OnPlayerCoreLoad()
    {
        switch (SceneContext.Instance.PlayerState.VacuumItem._vacMode)
        {
            case VacuumItem.VacMode.NONE:
                currVacMode = VacModes.NONE;
                break;
            case VacuumItem.VacMode.SHOOT:
                currVacMode = VacModes.AUTO_SHOOT;
                break;
            case VacuumItem.VacMode.VAC:
                currVacMode = VacModes.AUTO_VAC;
                break;
        }
        
    }
}


