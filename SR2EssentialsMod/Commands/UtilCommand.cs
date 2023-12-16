using Il2CppKinematicCharacterController;
using Il2CppMonomiPark.SlimeRancher.Player.CharacterController;
using System;
using SR2E.Saving;
using UnityEngine.UIElements;
using System.Linq;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine;

namespace SR2E.Commands
{
    internal class UtilCommand : SR2CCommand
    {
        

        public override string ID => "util";

        public override string Usage => "util <args>";

        public override string Description => "Utility Command";
        public override string ExtendedDescription => "Utility command, read the arguments on the help section on SR2E github wiki.";

        public const float playerColliderHeightBase = 2f;
        public const float playerColliderRadBase = 0.6f;

        public readonly List<string> TypeParam = new List<string>()
        {
            "GAME",
            "GORDO",
            "SLIME",
            "PLAYER",
            "GADGET"
        };

        public readonly List<string> GordoParam = new List<string>()
        {
            "BASE_SIZE",
            "EATEN_COUNT",
            "PRINT_ID"
        };

        public readonly List<string> GameParam = new List<string>()
        {
            "DISABLE_ACTOR_TYPE",
            "FAST_QUIT"
        };

        public readonly List<string> SlimeParam = new List<string>()
        {
            "SLIME_HUNGER",
            "SLIME_AGI",
            "SLIME_FEAR",
            "ZERO_GRAV",
            "CUSTOM_SCALE_XYZ"
        };

        public readonly List<string> ParamPlaceholder = new List<string>()
        {
            "PLACEHOLDER",
            "TEST",
            "1_2_3"
        };

        public readonly List<string> PlayerParam = new List<string>()
        {
            "CUSTOM_SIZE",
            "GRAVITY_LEVEL",
            "VAC_MODE"
        };

        public readonly List<string> GadgetParam = new List<string>()
        {
            "ROTATION",
            "POSITION"
        };



        public void SlimeEmotion(bool isGet, SlimeEmotions.Emotion emotion, float val = 1f)
        {
            if (Physics.Raycast(new Ray(Camera.main.transform.position, Camera.main.transform.forward), out var hit))
            {
                var slime = hit.collider.gameObject.GetComponent<SlimeEmotions>();
                if (slime != null)
                {
                    var emotions = new Dictionary<SlimeEmotions.Emotion, float>()
                    {
                        { SlimeEmotions.Emotion.HUNGER, slime._model.emotionHunger.CurrVal },
                        { SlimeEmotions.Emotion.AGITATION, slime._model.emotionAgitation.CurrVal },
                        { SlimeEmotions.Emotion.FEAR, slime._model.emotionFear.CurrVal }
                    };
                    if (isGet)
                    {
                        SR2Console.SendMessage($"The {slime.gameObject.GetComponent<Identifiable>().identType.localizedName.GetLocalizedString().ToLower()}\'s {emotion.ToString().ToLower()} is {emotions[emotion]}");
                        return;
                    }
                    else
                    {
                        if (emotion == SlimeEmotions.Emotion.HUNGER)
                        {
                            slime._model.emotionHunger.CurrVal = val;
                            SR2Console.SendMessage($"The {slime.gameObject.GetComponent<Identifiable>().identType.localizedName.GetLocalizedString().ToLower()}\'s {emotion.ToString().ToLower()} is now {val}");
                        }
                        if (emotion == SlimeEmotions.Emotion.AGITATION)
                        {
                            slime._model.emotionAgitation.CurrVal = val;
                            SR2Console.SendMessage($"The {slime.gameObject.GetComponent<Identifiable>().identType.localizedName.GetLocalizedString().ToLower()}\'s {emotion.ToString().ToLower()} is now {val}");
                        }
                        if (emotion == SlimeEmotions.Emotion.FEAR)
                        {
                            slime._model.emotionFear.CurrVal = val;
                            SR2Console.SendMessage($"The {slime.gameObject.GetComponent<Identifiable>().identType.localizedName.GetLocalizedString().ToLower()}\'s {emotion.ToString().ToLower()} is now {val}");
                        }

                    }
                }
            }
        }
        public void ToggleActorZeroGrav()
        {
            if (Physics.Raycast(new Ray(Camera.main.transform.position, Camera.main.transform.forward), out var hit))
            {
                var actor = hit.collider.gameObject.GetComponent<Vacuumable>();
                if (actor != null)
                {
                    actor.ignoresGravity = !actor.ignoresGravity;
                    var logString = "NULL";
                    if (actor.ignoresGravity)
                        logString = "deactivated";
                    else
                        logString = "activated";
                    SR2Console.SendMessage($"The {actor.gameObject.GetComponent<Identifiable>().identType.localizedName.GetLocalizedString().ToLower()}\'s gravity is now {logString}");
                }
            }
        }

        public void SetActorScale(float x, float y, float z)
        {
            if (Physics.Raycast(new Ray(Camera.main.transform.position, Camera.main.transform.forward), out var hit))
            {
                var actor = hit.collider.gameObject.GetComponent<Vacuumable>();
                if (actor != null)
                {
                    actor.transform.localScale = new Vector3(x, y, z);

                    SR2Console.SendMessage($"The {actor.gameObject.GetComponent<Identifiable>().identType.localizedName.GetLocalizedString().ToLower()}\'s scale vector is now {x}, {y}, {z}");
                }
            }
        }

        public bool ExcSlime(string[] cmd)
        {
            if (cmd[1] == "SLIME_HUNGER")
            {
                if (cmd.Length == 2)
                {
                    SlimeEmotion(true, SlimeEmotions.Emotion.HUNGER);
                    return true;
                }
                else
                {
                    SlimeEmotion(false, SlimeEmotions.Emotion.HUNGER, float.Parse(cmd[2]));
                    return true;
                }
            }
            else if (cmd[1] == "SLIME_AGI")
            {
                if (cmd.Length == 2)
                {
                    SlimeEmotion(true, SlimeEmotions.Emotion.AGITATION);
                    return true;
                }
                else
                {
                    SlimeEmotion(false, SlimeEmotions.Emotion.AGITATION, float.Parse(cmd[2]));
                    return true;
                }
            }
            else if (cmd[1] == "SLIME_FEAR")
            {
                if (cmd.Length == 2)
                {
                    SlimeEmotion(true, SlimeEmotions.Emotion.FEAR);
                    return true;
                }
                else
                {
                    SlimeEmotion(false, SlimeEmotions.Emotion.FEAR, float.Parse(cmd[2]));
                    return true;
                }
            }
            else if (cmd[1] == "ZERO_GRAV")
            {
                ToggleActorZeroGrav();
            }
            else if (cmd[1] == "CUSTOM_SCALE_XYZ")
            {
                SetActorScale(float.Parse(cmd[2]), float.Parse(cmd[3]), float.Parse(cmd[4]));
            }
            return false;
        }

        public void GordoSize(bool isGet, float size = 4)
        {
            if (Physics.Raycast(new Ray(Camera.main.transform.position, Camera.main.transform.forward), out var hit))
            {
                var gordo = hit.collider.gameObject.GetComponent<GordoEat>();
                if (gordo != null)
                {
                    if (isGet)
                    {
                        SR2Console.SendMessage($"The {gordo.SlimeDefinition.name.ToLower()} gordo\'s base size is {gordo._initScale}");
                        return;
                    }
                    else
                    {
                        gordo._initScale = size;
                        SR2Console.SendMessage($"The {gordo.SlimeDefinition.name} Gordo\'s size is now {size}");
                    }
                }
            }
        }
        public void GordoEatenAmount(bool isGet, int amount = 49)
        {
            if (Physics.Raycast(new Ray(Camera.main.transform.position, Camera.main.transform.forward), out var hit))
            {
                var gordo = hit.collider.gameObject.GetComponent<GordoEat>();
                if (gordo != null)
                {
                    if (isGet)
                    {
                        SR2Console.SendMessage($"The amount of food the {gordo.SlimeDefinition.name.ToLower()} gordo ate is {gordo.GetEatenCount()}");
                        return;
                    }
                    else
                    {
                        gordo.SetEatenCount(amount);
                        SR2Console.SendMessage($"The {gordo.SlimeDefinition.name} Gordo\'s eaten count is now {amount}");
                    }
                }
            }
        }
        public void PrintGordoID()
        {
            if (Physics.Raycast(new Ray(Camera.main.transform.position, Camera.main.transform.forward), out var hit))
            {
                var gordo = hit.collider.gameObject.GetComponent<GordoEat>();
                if (gordo != null)
                {
                    SR2Console.SendMessage($"This {gordo.SlimeDefinition.name.ToLower()} gordo\'s ID is {gordo._id}");
                }
            }
        }
        public bool ExcGordo(string[] cmd)
        {
            if (cmd[1] == "BASE_SIZE")
            {
                if (cmd.Length == 2)
                {
                    GordoSize(true);
                    return true;
                }
                else
                {
                    GordoSize(false, float.Parse(cmd[2]));
                    return true;
                }
            }
            else if (cmd[1] == "EATEN_COUNT")
            {
                if (cmd.Length == 2)
                {
                    GordoEatenAmount(true);
                    return true;
                }
                else
                {
                    GordoEatenAmount(false, int.Parse(cmd[2]));
                    return true;
                }
            }
            else if (cmd[1] == "PRINT_ID")
            {
                PrintGordoID();
            }
            return false;
        }

        public void DisableIdent(string identName)
        {
            var type = SR2EEntryPoint.getIdentifiableByLocalizedName(identName.Replace("_", ""));
            MelonLogger.Warning(type.ToString() + "DEBUG LOG");
            foreach (var actor in Resources.FindObjectsOfTypeAll<IdentifiableActor>())
            {
                if (actor.identType == type)
                {
                    MelonLogger.Msg("DEBUG");
                    actor.gameObject.AddComponent<ObjectBlocker>();
                }
            }

        }

        public bool ExcGame(string[] cmd)
        {
            if (cmd[1] == "DISABLE_ACTOR_TYPE")
            {
                DisableIdent(cmd[2]);
                return true;
            }
            else if (cmd[1] == "FAST_QUIT")
            {
                Application.Quit();
                return true;
            }
            return false;
        }

        public void PlayerSize(bool isGet, float size = 1)
        {
            if (isGet)
            {
                SR2Console.SendMessage($"The current size of the player is {SceneContext.Instance.player.transform.localScale.x}");
            }
            else
            {
                SceneContext.Instance.player.transform.localScale = Vector3.one * size;
                var KCC = SceneContext.Instance.player.GetComponent<KinematicCharacterMotor>();
                KCC.CapsuleHeight = playerColliderHeightBase * size;
                KCC.CapsuleRadius = playerColliderRadBase * size;
                SR2ESavableData.Instance.playerSavedData.size = size;
                SR2Console.SendMessage($"The new size of the player is {size}");

            }
        }
        public static void RemoteExc_PlayerSize(float size)
        {
            SceneContext.Instance.player.transform.localScale = Vector3.one * size;
            var KCC = SceneContext.Instance.player.GetComponent<KinematicCharacterMotor>();
            KCC.CapsuleHeight = playerColliderHeightBase * size;
            KCC.CapsuleRadius = playerColliderRadBase * size;
        }
        public void PlayerGravity(bool isGet, float level = 1)
        {
            if (isGet)
            {
                SR2Console.SendMessage($"The current gravity level of the player is {SceneContext.Instance.player.GetComponent<SRCharacterController>()._gravityMagnitude}");
            }
            else
            {
                SceneContext.Instance.player.GetComponent<SRCharacterController>()._gravityMagnitude = new Il2CppSystem.Nullable<float>(level);
                SR2ESavableData.Instance.playerSavedData.gravityLevel = level;
                SR2Console.SendMessage($"The new gravity level of the player is {level}");

            }
        }

        public void GadgetPos(bool isGet, float posX = 0, float posY = 0, float posZ = 0)
        {
            try
            {
                if (isGet)
                {
                    var gadget = SR2EUtils.RaycastForGadget();
                    if (gadget != null)
                    {
                        var pos = gadget.transform.position;
                        SR2Console.SendMessage($"This {gadget.identType.LocalizedName.GetLocalizedString().ToLower()}\'s position is {pos.x}, {pos.y}, {pos.z}");
                    }
                }
                else
                {
                    var gadget = SR2EUtils.RaycastForGadget();
                    if (gadget != null)
                    {
                        var pos = new Vector3(posX, posY, posZ);
                        gadget.transform.position = pos;
                        gadget.Model.lastPosition = pos;
                        SR2Console.SendMessage($"This {gadget.identType.LocalizedName.GetLocalizedString().ToLower()}\'s position is now {posX}, {posY}, {posZ}");
                    }
                }
            }
            catch { }
            
        }
        public void GadgetRot(bool isGet, float rot = 0f)
        {
            try
            {
                if (isGet)
                {
                    var gadget = SR2EUtils.RaycastForGadget();
                    if (gadget != null)
                    {
                        var gadgetRot = gadget.transform.eulerAngles.y;
                        SR2Console.SendMessage($"This {gadget.identType.LocalizedName.GetLocalizedString().ToLower()}\'s rotation is {gadgetRot}");
                    }
                }
                else
                {
                    var gadget = SR2EUtils.RaycastForGadget();
                    if (gadget != null)
                    {
                            gadget.Model.yRotation = rot;
                            gadget.transform.rotation = Quaternion.EulerRotation(new Vector3(0, rot, 0));
                            SR2Console.SendMessage($"This {gadget.identType.LocalizedName.GetLocalizedString().ToLower()}\'s rotation is now {rot}");
                    }
                }
            }
            catch { }
            
        }
        public bool ExcGadget(string[] cmd)
        {
            if (cmd[1] == "POSITION")
            {
                if (cmd.Length > 2)
                {
                    GadgetPos(false, float.Parse(cmd[2]), float.Parse(cmd[3]), float.Parse(cmd[4]));
                }
                else
                {
                    GadgetPos(true);
                }
                return true;
            }
            else if (cmd[1] == "ROTATION")
            {
                if (cmd.Length > 2)
                {
                    GadgetRot(false, float.Parse(cmd[2]));
                }
                else
                {
                    GadgetRot(true);
                }
                return true;
            }
            return false;
        }

        

        public static void PlayerVacModeSet(VacModes mode)
        {
            if (mode == VacModes.NORMAL)
            {
                SceneContext.Instance.PlayerState.Vacuum.gameObject.SetActive(true);
                SceneContext.Instance.PlayerState.Vacuum._vacMode = WeaponVacuum.VacMode.NONE;
                SceneContext.Instance.Camera.RemoveComponent<IdentifiableObjectDragger>();
            }
            else if (mode == VacModes.AUTO_VAC)
            {
                SceneContext.Instance.PlayerState.Vacuum.gameObject.SetActive(true);
                SceneContext.Instance.PlayerState.Vacuum._vacMode = WeaponVacuum.VacMode.VAC;
                SceneContext.Instance.Camera.RemoveComponent<IdentifiableObjectDragger>();
            }
            else if (mode == VacModes.AUTO_SHOOT)
            {
                SceneContext.Instance.PlayerState.Vacuum.gameObject.SetActive(true);
                SceneContext.Instance.PlayerState.Vacuum._vacMode = WeaponVacuum.VacMode.SHOOT;
                SceneContext.Instance.Camera.RemoveComponent<IdentifiableObjectDragger>();
            }
            else if (mode == VacModes.NONE)
            {
                SceneContext.Instance.PlayerState.Vacuum._vacMode = WeaponVacuum.VacMode.NONE;

                MelonCoroutines.Start(waitForSeconds(1.5f));

                SceneContext.Instance.Camera.RemoveComponent<IdentifiableObjectDragger>();
                SceneContext.Instance.PlayerState.Vacuum.gameObject.SetActive(false);
            }
            else if (mode == VacModes.DRAG)
            {
                SceneContext.Instance.PlayerState.Vacuum._vacMode = WeaponVacuum.VacMode.NONE;

                MelonCoroutines.Start(waitForSeconds(1.5f));

                SceneContext.Instance.PlayerState.Vacuum.gameObject.SetActive(false);
                SceneContext.Instance.Camera.AddComponent<IdentifiableObjectDragger>();
            }
            SR2ESavableData.Instance.playerSavedData.vacMode = mode;
        }

        static System.Collections.IEnumerator waitForSeconds(float seconds)
        {
            yield return new WaitForSeconds(seconds);
        }

        public bool ExcPlayer(string[] cmd)
        {
            if (cmd[1] == "CUSTOM_SIZE")
            {
                if (cmd.Length > 2)
                {
                    PlayerSize(false, float.Parse(cmd[2]));
                }
                else
                {
                    PlayerSize(true);
                }
                return true;
            }
            else if (cmd[1] == "GRAVITY_LEVEL")
            {
                if (cmd.Length > 2)
                {
                    PlayerGravity(false, float.Parse(cmd[2]));
                }
                else
                {
                    PlayerGravity(true);
                }
                return true;
            }
            else if (cmd[1] == "VAC_MODE")
            {
                try
                {
                    PlayerVacModeSet(Enum.Parse<VacModes>(cmd[2]));
                }
                catch { return false; }
                return true;
            }
            return false;
        }
        public override bool Execute(string[] args)
        {
            if (args[0] == "GORDO")
            {
                return ExcGordo(args);
            }
            else if (args[0] == "SLIME")
            {
                return ExcSlime(args);
            }
            else if (args[0] == "GAME")
            {
                return ExcGame(args);
            }
            else if (args[0] == "PLAYER")
            {
                return ExcPlayer(args);
            }
            else if (args[0] == "GADGET")
            {
                return ExcGadget(args);
            }
            return false;
        }

        

        public override List<string> GetAutoComplete(int argIndex, string[] args)
        {
            if (argIndex == 0)
            {
                return TypeParam;
            }
            else if (argIndex == 1)
            {
                if (args[0] == "GORDO")
                {
                    return GordoParam;
                }
                else if (args[0] == "SLIME")
                {
                    return SlimeParam;
                }
                else if (args[0] == "GAME")
                {
                    return GameParam;
                }
                else if (args[0] == "PLAYER")
                {
                    return PlayerParam;
                }
                else if (args[0] == "GADGET")
                {
                    return GadgetParam;
                }
            }
            else if (argIndex == 2)
            {
                if (args[0] == "PLAYER")
                {
                    if (args[1] == "VAC_MODE")
                    {
                        return Enum.GetNames(typeof(VacModes)).ToList();
                    }
                }
                if (args[0] == "GAME")
                {
                    if (args[1] == "DISABLE_ACTOR_TYPE")
                    {
                        string firstArg = "";
                        if (args != null)
                            firstArg = args[0];
                        List<string> list = new List<string>();
                        int i = -1;
                        foreach (IdentifiableType type in SR2EEntryPoint.identifiableTypes)
                        {
                            if (i > 20)
                                break;
                            try
                            {
                                if (type.LocalizedName != null)
                                {
                                    string localizedString = type.LocalizedName.GetLocalizedString();
                                    var s = localizedString.Replace(" ", "");
                                    list.Add(s);
                                }
                            }
                            catch { }

                        }

                        return list;
                    }
                }
            }
            return null;
        }
    }
}

