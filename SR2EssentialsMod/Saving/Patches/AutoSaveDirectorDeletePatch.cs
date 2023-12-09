using System.IO;

[HarmonyPatch(typeof(AutoSaveDirector), nameof(AutoSaveDirector.DeleteSave))]
public static class AutoSaveDirectorDeletePatch
{
    public static void Prefix(AutoSaveDirector __instance, string saveName)
    {
        string path = Path.Combine(SystemContext.Instance.GetStorageProvider().Cast<FileStorageProvider>().savePath, $"{saveName}.sr2e");
        if (File.Exists(path))
            File.Delete(path);
    }
}