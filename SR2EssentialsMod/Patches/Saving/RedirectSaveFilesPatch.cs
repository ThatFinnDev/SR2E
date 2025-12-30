using System.IO;

namespace SR2E.Patches.Saving;

[HarmonyPatch(typeof(SystemContext), nameof(SystemContext.GetStorageProvider))]
internal static class RedirectSaveFilesPatch
{
    static StorageProvider _Provider = null;
    static StorageProvider Provider
    {
        get
        {
            if (_Provider == null)
            {
                var savePath = Path.Combine(SR2EEntryPoint.DataPath, "redirectedSaves");
                Directory.CreateDirectory(savePath);
                _Provider = new FileStorageProvider(savePath).TryCast<StorageProvider>();
            }
            return _Provider;
        }
    }
    public static bool Prefix(SystemContext __instance, ref StorageProvider __result)
    {
        if (!RedirectSaveFiles.HasFlag()) return true;
        __result = Provider;
        return false; 
    }
    
}