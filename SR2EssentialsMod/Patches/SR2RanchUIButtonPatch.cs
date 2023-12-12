using Il2CppMonomiPark.SlimeRancher.UI.RanchHouse;

namespace SR2E.Patches;

[HarmonyPatch(typeof(RanchHouseMenuRoot), nameof(RanchHouseMenuRoot.Awake))]
public static class SR2RanchUIButtonPatch
{
    internal static List<CustomRanchUIButton> buttons = new List<CustomRanchUIButton>();
    internal static bool safeLock;
    internal static bool postSafeLock;
    public static void Prefix(RanchHouseMenuRoot __instance)
    {
        if (safeLock) { return; }
        safeLock = true;
        foreach (CustomRanchUIButton button in buttons)
        {
            if (button.name == null || button.label == null || button.action == null) continue;
            try
            {
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
                button._model.name = button.name;
                button._model.hideFlags |= HideFlags.HideAndDontSave;

                if (!__instance._menuItems.Contains(button._model))
                    __instance._menuItems.Insert(button.insertIndex, button._model);
            }
            catch (Exception e) { MelonLogger.Error(e); }


        }
        safeLock = false;
    }
}