/*using Il2CppMonomiPark.SlimeRancher.Player.CharacterController;
using SR2E.Saving;
using System.IO;

[HarmonyPatch(typeof(AutoSaveDirector), nameof(AutoSaveDirector.SaveGame))]
internal static class AutoSaveDirectorSavePatch
{
    internal static void Postfix(AutoSaveDirector __instance)
    {
        MelonLogger.Msg("SAVE GAME");
        if (SR2EEntryPoint.debugLogging)
            MelonLogger.Msg("AutoSaveDirector.SaveGame PostFix");


        SR2ESavableDataV2.Instance.playerSavedData.velocity =
            new Vector3Data(SceneContext.Instance.player.GetComponent<SRCharacterController>().Velocity);

        foreach (var savableSlime in Resources.FindObjectsOfTypeAll<SR2ESlimeDataSaver>())
            try { savableSlime.SaveData(); } catch { }
        foreach (var savableGadget in Resources.FindObjectsOfTypeAll<SR2EGadgetDataSaver>())
            try { savableGadget.SaveData(); } catch { }
        foreach (var savableGordo in Resources.FindObjectsOfTypeAll<SR2EGordoDataSaver>())
            try { savableGordo.SaveData(); } catch { }

        SR2ESavableDataV2.currPath = Path.Combine(SR2ESavableDataV2.Instance.dir, SR2ESavableDataV2.Instance.gameName+".sr2ev2");
        if (SR2EEntryPoint.debugLogging)
            SR2EConsole.SendWarning(SR2ESavableDataV2.currPath);
        SR2ESavableDataV2.Instance.TrySave();
    }
}*/
//Broken as of SR2 0.6.0