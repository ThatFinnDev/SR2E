
using System;
using Il2CppMonomiPark.SlimeRancher.UI.ButtonBehavior;
using Il2CppMonomiPark.SlimeRancher.UI.MainMenu;
using Il2CppMonomiPark.SlimeRancher.UI.MainMenu.Definition;
using Il2CppMonomiPark.SlimeRancher.UI.MainMenu.Model;
using Il2CppTMPro;
using SR2E.Buttons;
using SR2E.Components;

namespace SR2E.Patches.MainMenu;


[HarmonyPatch(typeof(MainMenuLandingRootUI), nameof(MainMenuLandingRootUI.Init))]
internal static class MainMenuLandingRootUIInitPatch
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
                if (button._definition != null)
                {
                    if (__instance._mainMenuConfig.items.Contains(button._definition))
                        continue;
                    int _offset = 0;
                    foreach (var item in __instance._mainMenuConfig.items) 
                        if(item is LoadGameItemDefinition) if(!(item is CustomMainMenuItemDefinition)) _offset=1;
                    __instance._mainMenuConfig.items.Insert(button.insertIndex+_offset, button._definition);
                    continue;
                }

                button._definition = ScriptableObject.CreateInstance<CustomMainMenuItemDefinition>();
                button._definition._label = button.label;
                button._definition.name = button.label.TableEntryReference.Key;
                button._definition._icon = button.icon;
                button._definition.hideFlags |= HideFlags.HideAndDontSave;
                button._definition.customAction = button.action;
                int offset = 0;
                foreach (var item in __instance._mainMenuConfig.items) 
                    if(item is LoadGameItemDefinition) if(!(item is CustomMainMenuItemDefinition)) offset=1;
                __instance._mainMenuConfig.items.Insert(button.insertIndex+offset, button._definition);
            }
            catch (Exception e) { MelonLogger.Error(e); }
        }
    }

    static void ChangeVersionLabel()
    {
        
        if (EnableLocalizedVersionPatch.HasFlag()) 
            try
            {
                var versionLabel = Get<LocalizedVersionText>("Version Label").GetComponent<TextMeshProUGUI>();
                if(!versionLabel.text.Contains("Mel"))
                {
                    if (SR2EEntryPoint.newVersion != null)
                        if (SR2EEntryPoint.newVersion != BuildInfo.DisplayVersion)
                        {
                            if (SR2EEntryPoint.updatedSR2E) versionLabel.text = translation("patches.localizedversionpatch.downloadedversion", SR2EEntryPoint.newVersion, versionLabel.text);
                            else versionLabel.text = translation("patches.localizedversionpatch.newversion", SR2EEntryPoint.newVersion, versionLabel.text);
                        }
                    versionLabel.text = translation("patches.localizedversionpatch.default", mlVersion, versionLabel.text);
                }
            }
            catch {  }
    }
    private static void Postfix()
    {
        ChangeVersionLabel();
        ExecuteInTicks((() => { ChangeVersionLabel();}), 1);
        ExecuteInTicks((() => { ChangeVersionLabel();}), 3);
        ExecuteInTicks((() => { ChangeVersionLabel();}), 10);
    }
}
