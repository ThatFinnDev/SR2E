
using Il2CppMonomiPark.SlimeRancher.UI.ButtonBehavior;
using Il2CppMonomiPark.SlimeRancher.UI.MainMenu;

namespace SR2E.Patches;


[HarmonyPatch(typeof(MainMenuLandingRootUI), nameof(MainMenuLandingRootUI.CreateModels))]
public static class SR2MainMenuButtonPatch
{
    internal static List<CustomMainMenuButton> buttons = new List<CustomMainMenuButton>();
    internal static bool safeLock;
    internal static bool postSafeLock;
    public static void Prefix(MainMenuLandingRootUI __instance)
    {
        foreach (CustomMainMenuButton button in buttons)
        {
            if (button.name == null || button.label == null || button.action == null) continue;
            try
            {
                if (button._definition != null)
                {
                    if (__instance._continueGameConfig.items.Contains(button._definition))
                        continue;
                    __instance._continueGameConfig.items.Insert(button.insertIndex + 1, button._definition);
                    __instance._existingGameNoContinueConfig.items.Insert(button.insertIndex, button._definition);
                    __instance._newGameConfig.items.Insert(button.insertIndex, button._definition);
                    continue;
                }
                button._definition = ScriptableObject.CreateInstance<CustomMainMenuItemDefinition>();
                button._definition.action = button.action;
                button._definition.label = button.label;
                button._definition.name = button.name;
                button._definition.icon = button.icon;
                if( button._definition.loadGameBehaviorModel!=null)
                    button._definition.loadGameBehaviorModel.action = button.action;
                button._definition.hideFlags |= HideFlags.HideAndDontSave;
                __instance._continueGameConfig.items.Insert(button.insertIndex + 1, button._definition);
                __instance._existingGameNoContinueConfig.items.Insert(button.insertIndex, button._definition);
                __instance._newGameConfig.items.Insert(button.insertIndex, button._definition);
            }
            catch (Exception e) { MelonLogger.Error(e); }
        }
    }
/*
    public static void Prefix(MainMenuLandingRootUI __instance)
    {
        MelonLogger.Msg(buttons.Count);
        foreach (CustomMainMenuButton button in buttons)
        {
            if (button.name == null || button.label == null || button.action == null) continue;
            try
            {
                if (button._prefabToSpawn == null)
                {
                    var obj = new GameObject();
                    UnityEngine.Object.DontDestroyOnLoad(obj);
                    obj.name = button.name + "ButtonStarter";
                    obj.transform.parent = rootOBJ.transform;
                    obj.AddComponent<CustomMainMenuButtonPressHandler>();
                    button._prefabToSpawn = obj;
                }
                if (button._definition != null)
                {
                    if (__instance._continueGameConfig.items.Contains(button._definition))
                        continue;
                    __instance._continueGameConfig.items.Insert(button.insertIndex + 1, button._definition);
                    __instance._existingGameNoContinueConfig.items.Insert(button.insertIndex, button._definition);
                    __instance._newGameConfig.items.Insert(button.insertIndex, button._definition);
                    continue;
                }
                button._definition = ScriptableObject.CreateInstance<CreateNewUIItemDefinition>();
                button._definition.label = button.label;
                button._definition.name = button.name;
                button._definition.icon = button.icon;
                button._definition.hideFlags |= HideFlags.HideAndDontSave;
                button._definition.prefabToSpawn = button._prefabToSpawn;
                __instance._continueGameConfig.items.Insert(button.insertIndex + 1, button._definition);
                __instance._existingGameNoContinueConfig.items.Insert(button.insertIndex, button._definition);
                __instance._newGameConfig.items.Insert(button.insertIndex, button._definition);
            }
            catch (Exception e) { MelonLogger.Error(e); }
        }
    }
    private static void Postfix()
    {
        foreach (CustomMainMenuButton button in buttons)
        {
            if (button.name == null || button.label == null  || button.action == null) continue;
            try
            {
                if (button._prefabToSpawn == null)
                {
                    var obj = new GameObject();
                    UnityEngine.Object.DontDestroyOnLoad(obj);
                    obj.name = button.name + "ButtonStarter";
                    obj.transform.parent = rootOBJ.transform;
                    obj.AddComponent<CustomMainMenuButtonPressHandler>();
                    button._prefabToSpawn = obj;
                    button._definition.prefabToSpawn = obj;
                }
            }
            catch (Exception e) { MelonLogger.Error(e); }
        }
    }*/
}
