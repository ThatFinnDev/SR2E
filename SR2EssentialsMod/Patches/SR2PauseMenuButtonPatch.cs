using Il2CppMonomiPark.SlimeRancher.Script.UI.Pause;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SR2E.Patches;

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
            if (button.name == null || button.label == null || button.action == null) continue;
            try
            {
                if (button._model != null)
                {
                    if (__instance.pauseUIPrefab.pauseItemModelListProvider.defaultAsset.items.Contains(button._model))
                        continue;
                    if (!__instance.pauseUIPrefab.pauseItemModelListProvider.defaultAsset.items.Contains(button._model))
                        __instance.pauseUIPrefab.pauseItemModelListProvider.defaultAsset.items.Insert(button.insertIndex, button._model);
                    if (!__instance.pauseUIPrefab.pauseItemModelListProvider.epicAsset.items.Contains(button._model))
                        __instance.pauseUIPrefab.pauseItemModelListProvider.epicAsset.items.Insert(button.insertIndex, button._model);
                    if (!__instance.pauseUIPrefab.pauseItemModelListProvider.editorAsset.items.Contains(button._model))
                        __instance.pauseUIPrefab.pauseItemModelListProvider.editorAsset.items.Insert(button.insertIndex, button._model);
                    if (!__instance.pauseUIPrefab.pauseItemModelListProvider.steamAsset.items.Contains(button._model))
                        __instance.pauseUIPrefab.pauseItemModelListProvider.steamAsset.items.Insert(button.insertIndex, button._model);
                    if (!__instance.pauseUIPrefab.pauseItemModelListProvider.standaloneAsset.items.Contains(button._model))
                        __instance.pauseUIPrefab.pauseItemModelListProvider.standaloneAsset.items.Insert(button.insertIndex, button._model);
                    if (!__instance.pauseUIPrefab.pauseItemModelListProvider.gameCoreWindowsAsset.items.Contains(button._model))
                        __instance.pauseUIPrefab.pauseItemModelListProvider.gameCoreWindowsAsset.items.Insert(button.insertIndex, button._model);
                    continue;
                }
                button._model = ScriptableObject.CreateInstance<CustomPauseItemModel>();
                button._model.action = button.action;
                button._model.label = button.label;
                button._model.name = button.name;
                button._model.hideFlags |= HideFlags.HideAndDontSave;
                //button._model.prefabToSpawn = button._prefabToSpawn;

                if (!__instance.pauseUIPrefab.pauseItemModelListProvider.defaultAsset.items.Contains(button._model))
                    __instance.pauseUIPrefab.pauseItemModelListProvider.defaultAsset.items.Insert(button.insertIndex, button._model);
                if (!__instance.pauseUIPrefab.pauseItemModelListProvider.epicAsset.items.Contains(button._model))
                    __instance.pauseUIPrefab.pauseItemModelListProvider.epicAsset.items.Insert(button.insertIndex, button._model);
                if (!__instance.pauseUIPrefab.pauseItemModelListProvider.editorAsset.items.Contains(button._model))
                    __instance.pauseUIPrefab.pauseItemModelListProvider.editorAsset.items.Insert(button.insertIndex, button._model);
                if (!__instance.pauseUIPrefab.pauseItemModelListProvider.steamAsset.items.Contains(button._model))
                    __instance.pauseUIPrefab.pauseItemModelListProvider.steamAsset.items.Insert(button.insertIndex, button._model);
                if (!__instance.pauseUIPrefab.pauseItemModelListProvider.standaloneAsset.items.Contains(button._model))
                    __instance.pauseUIPrefab.pauseItemModelListProvider.standaloneAsset.items.Insert(button.insertIndex, button._model);
                if (!__instance.pauseUIPrefab.pauseItemModelListProvider.gameCoreWindowsAsset.items.Contains(button._model))
                    __instance.pauseUIPrefab.pauseItemModelListProvider.gameCoreWindowsAsset.items.Insert(button.insertIndex, button._model);
                if (!__instance.pauseUIPrefab.pauseItemModelListProvider.gameCoreXboxSeriesAsset.items.Contains(button._model))
                    __instance.pauseUIPrefab.pauseItemModelListProvider.gameCoreXboxSeriesAsset.items.Insert(button.insertIndex, button._model);
            }
            catch (Exception e) { Console.WriteLine(e); }

        }
        safeLock = false;
    }
}