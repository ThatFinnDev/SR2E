/*using System;
using SR2E.Saving;
using System.IO;
public static class AutoSaveDirectorLoadPatch
{
    public static string loadPath => GameContext.Instance.AutoSaveDirector.StorageProvider.Cast<FileStorageProvider>().savePath;

    public static void Postfix(AutoSaveDirector __instance, string gameName, string saveName)
    {
        string[] split = saveName.Split("_");
        string path = Path.Combine(loadPath, split[0] + "_" + split[1] + ".sr2ev2");
        if (File.Exists(path))
        {
            try
            {
                SR2ESavableDataV2.LoadFromStream(new FileStream(path, FileMode.Open));
                SR2ESavableDataV2.currPath = path;
                SR2ESavableDataV2.Instance.dir = $"{loadPath}\\";
                SR2ESavableDataV2.Instance.gameName = gameName;
            }
            catch (Exception ex)
            {
                SR2EConsole.SendWarning("Failed to load SR2E save data, creating new");
                SR2EConsole.SendWarning($"Developer error: {ex}");

                //File.Create(path);
                new SR2ESavableDataV2();
                SR2ESavableDataV2.currPath = path;
                SR2ESavableDataV2.Instance.dir = $"{loadPath}\\";
                SR2ESavableDataV2.Instance.gameName = gameName;
            }
        }
        else
        {
            //File.Create(path);
            new SR2ESavableDataV2();
            SR2ESavableDataV2.currPath = path;
            SR2ESavableDataV2.Instance.dir = $"{loadPath}\\";
            SR2ESavableDataV2.Instance.gameName = gameName;
        }


    }
}

[HarmonyPatch(typeof(AutoSaveDirector), nameof(AutoSaveDirector.BeginLoad), new Type[] { typeof(string), typeof(string) })]
public static class AutoSaveDirectorLoadPatchStringString
{
    public static void Postfix(AutoSaveDirector __instance, string gameName, string saveName)
    {
        MelonLogger.Msg("BEGIN LOAD STRING STRING");
        AutoSaveDirectorLoadPatch.Postfix(__instance,gameName,saveName);
    }
}
[HarmonyPatch(typeof(AutoSaveDirector), nameof(AutoSaveDirector.BeginLoad), new Type[] { typeof(Il2CppSystem.IO.Stream),typeof(string), typeof(string) })]
public static class AutoSaveDirectorLoadPatchStreamStringString
{
    public static void Postfix(AutoSaveDirector __instance, Il2CppSystem.IO.Stream gameData, string gameName, string saveName)
    {
        MelonLogger.Msg("BEGIN LOAD STREAM TRING STRING");
        AutoSaveDirectorLoadPatch.Postfix(__instance,gameName,saveName);
    }
}*/
//Broken as of SR2 0.6.0