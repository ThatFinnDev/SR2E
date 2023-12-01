using Il2CppMonomiPark.SlimeRancher.Damage;
using Il2CppMonomiPark.SlimeRancher.DataModel;
using System.Reflection;

namespace SR2E.Commands
{
    internal class UtilCommand : SR2CCommand
    {
        public class ObjectBlocker : MonoBehaviour
        {
            public void Start()
            {
                Destroy(gameObject);
            }
        }

        public override string ID => "util";

        public override string Usage => "util <args>";

        public override string Description => "Utility Command";

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
            "CUSTOM_SIZE"
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
                SR2Console.SendError("This has not been implemented yet.");
                return false;
            }
            else if (args[0] == "GADGET")
            {
                SR2Console.SendError("This has not been implemented yet.");
                return false;
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
                    return ParamPlaceholder;
                }
                else if (args[0] == "GADGET")
                {
                    return ParamPlaceholder;
                }
            }
            else if (argIndex == 2)
            {
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
