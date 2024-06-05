using Il2CppMonomiPark.SlimeRancher.Script.UI.Pause;
using SR2E.Buttons;

namespace SR2E.Patches.InGame;

[HarmonyPatch(typeof(PauseMenuDirector), nameof(PauseMenuDirector.Awake))]
public static class SR2PauseMenuButtonPatch
{
    internal static List<CustomPauseMenuButton> buttons = new List<CustomPauseMenuButton>();
    internal static bool safeLock;
    internal static bool postSafeLock;
    public static void Prefix(PauseMenuDirector __instance)
    {
        if (safeLock) { return; }
        safeLock = true;
        foreach (CustomPauseMenuButton button in buttons)
        {
            if (button.label == null || button.action == null) continue;
            try
            {
                if (button._model != null)
                {
                    if (__instance.pauseUIPrefab.pauseItemModelList.items.Contains(button._model))
                        continue;
                    if (!__instance.pauseUIPrefab.pauseItemModelList.items.Contains(button._model))
                        __instance.pauseUIPrefab.pauseItemModelList.items.Insert(button.insertIndex, button._model);
                    continue;
                }
                button._model = ScriptableObject.CreateInstance<CustomPauseItemModel>();
                button._model.action = button.action;
                button._model.label = button.label;
                button._model.name = button.label.GetLocalizedString();
                button._model.hideFlags |= HideFlags.HideAndDontSave;
                //button._model.prefabToSpawn = button._prefabToSpawn;

                if (!__instance.pauseUIPrefab.pauseItemModelList.items.Contains(button._model))
                    __instance.pauseUIPrefab.pauseItemModelList.items.Insert(button.insertIndex, button._model);
                
            }
            catch (Exception e) { Console.WriteLine(e); }

        }
        safeLock = false;
    }
}