using System.Collections;
using System.IO;
using Newtonsoft.Json.Linq;
using UnityEngine.Networking;

namespace SR2E.Managers;

internal static class SR2EUpdateManager
{
    internal static bool updatedSR2E = false;
    internal static string newVersion = null;
    static bool IsLatestVersion => newVersion == BuildInfo.DisplayVersion;
    static string branchJson = "";
    internal static IEnumerator GetBranchJson()
    {
        string checkLink = BuildInfo.PRE_INFO[SR2EEntryPoint.updateBranch].Item2;
        if (string.IsNullOrEmpty(checkLink)) yield break;
        UnityWebRequest uwr = UnityWebRequest.Get(checkLink);
        yield return uwr.SendWebRequest();
        if (uwr.isNetworkError || uwr.isHttpError) yield break;
        string json = uwr.downloadHandler.text;
        try { JObject.Parse(json); }
        catch { MelonLogger.Msg("SR2E API either changed or is broken."); yield break; }
        branchJson = json;
        MelonCoroutines.Start(CheckForNewVersion());
    }
    internal static IEnumerator CheckForNewVersion()
    {
        if (string.IsNullOrWhiteSpace(branchJson)) yield break;
        try
        {
            var jobject = JObject.Parse(branchJson);
            string latest = jobject["latest"].ToObject<string>();
            newVersion = latest;
            if (!IsLatestVersion) if (AllowAutoUpdate.HasFlag()) if (SR2EEntryPoint.autoUpdate)
                MelonCoroutines.Start(UpdateVersion());
        }
        catch { MelonLogger.Msg("SR2E API either changed or is broken."); }
    }
    internal static IEnumerator UpdateVersion()
    {
        if (string.IsNullOrWhiteSpace(branchJson)) yield break;
        string updateLink = "";
        try
        {
            var jobject = JObject.Parse(branchJson);
            string latest = jobject["latest"].ToObject<string>();
            var latestVersion = jobject["versions_info"][latest];
            updateLink = latestVersion["download_url"].ToObject<string>();
        }
        catch { MelonLogger.Msg("SR2E API either changed or is broken."); yield break; }
        if (string.IsNullOrEmpty(updateLink)) yield break;
        UnityWebRequest uwr = UnityWebRequest.Get(updateLink);
        yield return uwr.SendWebRequest();
        if (!uwr.isNetworkError && !uwr.isHttpError)
            if (uwr.result == UnityWebRequest.Result.Success)
            {
                MelonLogger.Msg("Downloading SR2E complete");
                string path = SR2EEntryPoint.MLAssembly.Assembly.Location;
                if (File.Exists(path))
                {
                    if(File.Exists(path + ".old")) File.Delete(path + ".old");
                    File.Move(path, path + ".old");
                }
                File.WriteAllBytes(Path.Combine(new FileInfo(path).Directory.FullName, "SR2E.dll"), uwr.downloadHandler.data);
                updatedSR2E = true;
                MelonLogger.Msg("Restart needed for applying SR2E update");
            }
    }

}