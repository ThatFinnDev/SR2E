/*using System.IO;

[HarmonyPatch(typeof(AutoSaveDirector), nameof(AutoSaveDirector.DeleteGame))]
internal static class AutoSaveDirectorDeleteGamePatch
{
    internal static string loadPath => GameContext.Instance.AutoSaveDirector.StorageProvider.Cast<FileStorageProvider>().savePath;
    internal static void Prefix(AutoSaveDirector __instance, string gameName)
    {
        //Delete SRE2V2 Save Files
        string path = Path.Combine(loadPath, gameName+".sr2ev2");
        FileInfo ew = new FileInfo(path);
        ew.Delete();
    }
    
}*/
//Broken as of SR2 0.6.0