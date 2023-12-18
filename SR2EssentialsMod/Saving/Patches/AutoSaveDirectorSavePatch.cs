using Il2CppMonomiPark.SlimeRancher.Player.CharacterController;
using SR2E.Saving;
using SR2E;
using System.IO;

[HarmonyPatch(typeof(AutoSaveDirector), nameof(AutoSaveDirector.SaveGame))]
public static class AutoSaveDirectorSavePatch
{
    public static void Postfix(AutoSaveDirector __instance)
    {
        if (SR2EEntryPoint.debugLogging)
        {
            MelonLogger.Msg("test");
        }

        SR2ESavableData.Instance.playerSavedData.velocity = new Vector3Data(SceneContext.Instance.player.GetComponent<SRCharacterController>().Velocity);
        foreach (var savableSlime in Resources.FindObjectsOfTypeAll<SR2ESlimeDataSaver>())
        {
            try
            {
                savableSlime.SaveData();
            }
            catch
            {

            }
        }
        foreach (var savableGadget in Resources.FindObjectsOfTypeAll<SR2EGadgetDataSaver>())
        {
            try
            {
                savableGadget.SaveData();
            }
            catch (Exception e)
            {
                MelonLogger.Error(e);
            }
        }
        foreach (var gordo in Resources.FindObjectsOfTypeAll<SR2EGordoDataSaver>())
        {
            try
            {
                gordo.SaveData();
            }
            catch
            {

            }
        }
        if (SR2ESavableData.Instance.idx != AutoSaveDirector.MAX_AUTOSAVES)
        {
            SR2ESavableData.currPath = $"{Path.Combine(SR2ESavableData.Instance.dir, SR2ESavableData.Instance.gameName)}_{SR2ESavableData.Instance.idx + 1}.sr2e";
            SR2ESavableData.Instance.idx++;
        }
        else
        {
            SR2ESavableData.currPath = $"{Path.Combine(SR2ESavableData.Instance.dir, SR2ESavableData.Instance.gameName)}_{0}.sr2e";
            SR2ESavableData.Instance.idx = 0;
        }
        if (SR2EEntryPoint.debugLogging)
            SR2Console.SendWarning(SR2ESavableData.currPath);
        SR2ESavableData.Instance.TrySave();
    }
}