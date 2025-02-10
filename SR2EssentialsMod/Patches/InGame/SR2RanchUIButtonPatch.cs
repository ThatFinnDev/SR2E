using SR2E.Buttons;
using Il2CppMonomiPark.SlimeRancher.UI.RanchHouse;

namespace SR2E.Patches.InGame;

[HarmonyPatch(typeof(RanchHouseMenuRoot), nameof(RanchHouseMenuRoot.Awake))]
<<<<<<< HEAD
public static class SR2RanchUIButtonPatch
=======
internal static class SR2RanchUIButtonPatch
>>>>>>> experimental
{
    internal static List<CustomRanchUIButton> buttons = new List<CustomRanchUIButton>();
    internal static bool safeLock;
    internal static bool postSafeLock;
<<<<<<< HEAD
    public static void Prefix(RanchHouseMenuRoot __instance)
    {
=======
    internal static void Prefix(RanchHouseMenuRoot __instance)
    {
        if (!InjectRanchUIButtons.HasFlag()) return;
>>>>>>> experimental
        if (safeLock) { return; }
        safeLock = true;
        foreach (CustomRanchUIButton button in buttons)
        {
            if (button.label == null || button.action == null) continue;
            try
            {
                if (!button.enabled)
                {
                    if (__instance._menuItems.Contains(button._model))
                        __instance._menuItems.Remove(button._model);
                    continue;
                }
                if (button._model != null)
                {
                    if (__instance._menuItems.Contains(button._model))
                        continue;
                    if (!__instance._menuItems.Contains(button._model))
                        __instance._menuItems.Insert(button.insertIndex, button._model);
                    continue;
                }
                button._model = ScriptableObject.CreateInstance<RanchHouseMenuItemModel>();
                button._model._onClick.AddListener(button.action);
                button._model.label = button.label;
                button._model.name = button.label.GetLocalizedString();
                button._model.hideFlags |= HideFlags.HideAndDontSave;

                if (!button.enabled)
                {
                    if (__instance._menuItems.Contains(button._model))
                        __instance._menuItems.Remove(button._model);
                    continue;
                }
                if (!__instance._menuItems.Contains(button._model))
                    __instance._menuItems.Insert(button.insertIndex, button._model);
            }
            catch (Exception e) { MelonLogger.Error(e); }


        }
        safeLock = false;
    }
}