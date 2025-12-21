using System;
using Il2CppMonomiPark.SlimeRancher.Script.UI.Pause;
using Il2CppMonomiPark.SlimeRancher.UI;
using Il2CppMonomiPark.SlimeRancher.UI.Pause;
using SR2E.Buttons;

namespace SR2E.Patches.InGame;

[HarmonyPatch(typeof(PauseMenuDirector), nameof(PauseMenuDirector.Awake))]
internal static class SR2PauseDirectorPatch
{
    internal static void Prefix(PauseMenuDirector __instance)
    {
        if (!InjectPauseButtons.HasFlag()) return;
        SR2PauseMenuButtonPatch.Prefix(Get<PauseMenuRoot>("PauseMenuRoot"));
    }
}
internal static class SR2PauseMenuButtonPatch
{
    internal static List<CustomPauseMenuButton> buttons = new List<CustomPauseMenuButton>();
    internal static bool safeLock;
    internal static bool postSafeLock;
    internal static void Prefix(PauseMenuRoot __instance)
    {
        if (!InjectPauseButtons.HasFlag()) return;
        if (safeLock) { return; }
        safeLock = true;
        try
        {
            PauseMenuRoot pauseMenuRoot = __instance;
            PauseItemModelList pauseItemModelList = pauseMenuRoot.pauseItemModelList;
            Il2CppSystem.Collections.Generic.List<PauseItemModel> items = pauseItemModelList.items;
            foreach (CustomPauseMenuButton button in buttons)
            {
                if (button.label == null || button.action == null) continue;
                try
                {
                    if (button._model != null)
                    {
                        if (!button.enabled)
                        {
                            if (items.Contains(button._model))
                                items.Remove(button._model);
                            continue;
                        }

                        if (items.Contains(button._model))
                            continue;
                        if (!items.Contains(button._model))
                            items.Insert(Math.Clamp(button.insertIndex,0,items.Count), button._model);
                        continue;
                    }

                    button._model = ScriptableObject.CreateInstance<CustomPauseItemModel>();
                    button._model.action = button.action;
                    button._model.label = button.label;
                    button._model.name = button.label.GetLocalizedString();
                    button._model.hideFlags |= HideFlags.HideAndDontSave;
                    //button._model.prefabToSpawn = button._prefabToSpawn;

                    if (!button.enabled)
                    {
                        if (items.Contains(button._model))
                            items.Remove(button._model);
                        continue;
                    }

                    if (!items.Contains(button._model))
                        items.Insert(Math.Clamp(button.insertIndex,0,items.Count), button._model);

                }
                catch (Exception e)
                {
                    MelonLogger.Error(e);
                }
                
            }
            
            pauseItemModelList.items = items;
            pauseMenuRoot.pauseItemModelList = pauseItemModelList;
            
        }
        catch (Exception e) { MelonLogger.Error(e);}
        safeLock = false;
    }
}