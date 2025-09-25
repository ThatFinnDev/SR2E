using System;
using Il2CppMonomiPark.SlimeRancher.UI.ButtonBehavior;

namespace SR2E.Patches.MainMenu;

[HarmonyPatch(typeof(ButtonBehaviorViewHolder), nameof(ButtonBehaviorViewHolder.OnSelected))]
internal static class SaveButtonPatch
{
    internal static void Postfix(ButtonBehaviorViewHolder __instance)
    {
        GameContext.Instance.AutoSaveDirector._configuration._saveSlotCount=SAVESLOT_COUNT.Get();
        ExecuteInTicks((Action)(() => { GameContext.Instance.AutoSaveDirector._configuration._saveSlotCount=SAVESLOT_COUNT.Get();}), 3);
        if (!ExperimentalSaveExport.HasFlag()) return;
        if (!SR2EEntryPoint.mainMenuLoaded) return;
        if (__instance.gameObject.name == "SaveGameSlotButton(Clone)")
        {
            try
            {
                if (SaveGameRootUIPatch.path == null) return;
                SaveGameRootUIPatch.load = !SaveGameRootUIPatch.iconButton.gameObject.activeSelf;
                SaveGameRootUIPatch.selectedSave = __instance.transform.GetSiblingIndex() - 5;
            }
            catch { }
        }
    }
}