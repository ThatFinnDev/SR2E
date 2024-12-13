/*using System.IO;

[HarmonyPatch(typeof(AutoSaveDirector), nameof(AutoSaveDirector.DeleteGame))]
public static class AutoSaveDirectorDeleteGamePatch
{
    public static string loadPath => GameContext.Instance.AutoSaveDirector.StorageProvider.Cast<FileStorageProvider>().savePath;
    public static void Prefix(AutoSaveDirector __instance, string gameName)
    {
        //Delete SRE2V2 Save Files
        string path = Path.Combine(loadPath, gameName+".sr2ev2");
        FileInfo ew = new FileInfo(path);
        ew.Delete();
    }
    
}*/
//Broken as of SR2 0.6.0