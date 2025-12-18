using Il2CppMonomiPark.SlimeRancher;
using Il2CppMonomiPark.SlimeRancher.Damage;

namespace SR2E;

public static class ContextShortcuts
{
    internal static GameObject prefabHolder;
    public static SystemContext systemContext => SystemContext.Instance;
    public static GameContext gameContext => GameContext.Instance;
    public static SceneContext sceneContext => SceneContext.Instance;
    internal static Damage _killDamage;
    public static Damage killDamage => _killDamage;
    public static AutoSaveDirector autoSaveDirector => gameContext.AutoSaveDirector;

    public static bool inGame
    {
        get
        {
            try
            {
                if (sceneContext == null) return false;
                if (sceneContext.PlayerState == null) return false;
            }
            catch
            { return false; }
            return true;
        }
    }
    public static bool IsBetween(this string[] list, uint min, int max)
    {
        if (list == null)
        {
            if (min > 0) return false;
        }
        else 
        {
            if (list.Length < min) return false;
            if(max!=-1) if (list.Length > max) return false;
        }

        return true;
    }

    private static bool _ml072OrNewer = true;

    public static bool ml072OrNewer
    {
        get
        {
            if (SR2EEntryPoint._mlVersion == "undefined")
            {
                var tmp = mlVersion;
            }
            return _ml072OrNewer;
        }
    }

    public static string mlVersion
    { get {
            if(SR2EEntryPoint._mlVersion=="undefined")
            {
                try
                {
                    // This works on ML 0.7.2 and later
                    var propertiesBuildInfo = System.Type.GetType("MelonLoader.Properties.BuildInfo, MelonLoader");
                    SR2EEntryPoint._mlVersion = (string)propertiesBuildInfo.GetProperty("Version").GetValue(null, null);
                }
                catch
                {
                    _ml072OrNewer = false;
                    try
                    {
                        // This works on ML 0.7.1 and older
                        var buildInfo = System.Type.GetType("MelonLoader.BuildInfo, MelonLoader");
                        SR2EEntryPoint._mlVersion = (string)buildInfo.GetProperty("Version").GetValue(null, null);
                    }
                    catch
                    {
                        // This works on some version even lower than 0.7.1 and is a fallback for a new version
                        //Do this if ML changes MelonLoader.BuildInfo.Version again...
                        MelonLogger.Error("MelonLoader.BuildInfo.Version changed, if you are using not using the latest ML version, please update," +
                                          "otherwise this will be fixed in the next SR2E release!");
                        try
                        {
                            string logFilePath = Application.dataPath + "/../MelonLoader/Latest.log";
                            using (System.IO.FileStream logFileStream = new System.IO.FileStream(logFilePath, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.ReadWrite))
                            using (System.IO.StreamReader logFileReader = new System.IO.StreamReader(logFileStream))
                            {
                                string text = logFileReader.ReadToEnd();
                                var split = text.Split("\n");
                                if (string.IsNullOrWhiteSpace(split[0])) SR2EEntryPoint._mlVersion = split[2].Split("v")[1].Split(" ")[0];
                                else SR2EEntryPoint._mlVersion = split[1].Split("v")[1].Split(" ")[0];
                            }
                            
                        }
                        catch { SR2EEntryPoint._mlVersion = "unknown"; }
                    }
                }
            }
            return SR2EEntryPoint._mlVersion;
        }
    }
}