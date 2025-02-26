﻿
using Il2CppMonomiPark.SlimeRancher.UI.ButtonBehavior;
using Il2CppMonomiPark.SlimeRancher.UI.MainMenu;
using SR2E.Buttons;
using SR2E.Components;

namespace SR2E.Patches.MainMenu;


[HarmonyPatch(typeof(MainMenuLandingRootUI), nameof(MainMenuLandingRootUI.CreateModels))]
internal static class SR2MainMenuButtonPatch
{
    internal static List<CustomMainMenuButton> buttons = new List<CustomMainMenuButton>();
    internal static bool safeLock;
    internal static bool postSafeLock;
    internal static void Prefix(MainMenuLandingRootUI __instance)
    {
        if (!InjectMainMenuButtons.HasFlag()) return;
        foreach (CustomMainMenuButton button in buttons)
        {
            if (button.label == null || button.action == null) continue;
            try
            {
                if (button._prefabToSpawn == null)
                {
                    var obj = new GameObject();
                    UnityEngine.Object.DontDestroyOnLoad(obj);
                    obj.name = button.label.TableEntryReference.Key + "ButtonStarter";
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
                button._definition.name = button.label.TableEntryReference.Key;
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
        if (!InjectMainMenuButtons.HasFlag()) return;
        foreach (CustomMainMenuButton button in buttons)
        {
            if (button.label.GetLocalizedString() == null || button.label == null || button.action == null) continue;
            try
            {
                if (button._prefabToSpawn == null)
                {
                    var obj = new GameObject();
                    UnityEngine.Object.DontDestroyOnLoad(obj);
                    obj.name = button.label.GetLocalizedString() + "ButtonStarter";
                    obj.transform.parent = rootOBJ.transform;
                    obj.AddComponent<CustomMainMenuButtonPressHandler>();
                    button._prefabToSpawn = obj;
                    button._definition.prefabToSpawn = obj;
                }
            }
            catch (Exception e) { MelonLogger.Error(e); }
        }
    }
}
