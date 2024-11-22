
using Il2CppMonomiPark.SlimeRancher.UI.ButtonBehavior;
using Il2CppMonomiPark.SlimeRancher.UI.MainMenu;
using SR2E.Buttons;

namespace SR2E.Patches.MainMenu;


[HarmonyPatch(typeof(MainMenuLandingRootUI), nameof(MainMenuLandingRootUI.CreateModels))]
public static class SR2MainMenuButtonPatch
{
    internal static List<ButtonBehaviorDefinition> buttons = new List<ButtonBehaviorDefinition>();
    internal static bool safeLock;
    internal static bool postSafeLock;
    public static void Prefix(MainMenuLandingRootUI __instance)
    {
        __instance._continueGameConfig.items.Reverse();
        __instance._newGameConfig.items.Reverse();
        __instance._existingGameNoContinueConfig.items.Reverse();
        foreach (ButtonBehaviorDefinition button in buttons)
        {
            __instance._continueGameConfig.items.Insert(1, button);
            __instance._newGameConfig.items.Insert(1, button);
            __instance._existingGameNoContinueConfig.items.Insert(1, button);
        }
        __instance._continueGameConfig.items.Reverse();
        __instance._newGameConfig.items.Reverse();
        __instance._existingGameNoContinueConfig.items.Reverse();
    }
}
