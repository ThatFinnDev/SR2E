using Newtonsoft.Json;
using SR2E.Enums;
using SR2E.Storage;

namespace SR2E.Managers;

public static class SR2ESaveManager
{
    internal static SR2ESaveData data = null;
    internal static void Start()
    {
        Load();
    }
    internal static JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings
    {
        Error = (sender, args) =>
        {
            if(DebugLogging.HasFlag())
                MelonLogger.Msg($"Error: {args.ErrorContext.Error.Message}");
            args.ErrorContext.Handled = true;
        }
    };
    internal static void Load()
    {
        if (File.Exists(path))
        {
            data = JsonConvert.DeserializeObject<SR2ESaveData>(File.ReadAllText(path), jsonSerializerSettings);
            if (data.keyBinds == null) data.keyBinds = new Dictionary<Key, string>();
            if (data.warps == null) data.warps = new Dictionary<string, Warp>();
            if(data.themes == null) data.themes = new Dictionary<string, SR2EMenuTheme>();
        }
        else data = new SR2ESaveData();
    }
    internal static void Save() { File.WriteAllText(path,JsonConvert.SerializeObject(data, Formatting.Indented)); }
    static string path { get { FileStorageProvider provider = SystemContext.Instance.GetStorageProvider().TryCast<FileStorageProvider>(); if (provider==null) return Application.persistentDataPath + "/SR2E.data"; return provider.savePath + "/SR2E.data"; } }
   
    public class SR2ESaveData
    {
        public Dictionary<string, Warp> warps = new Dictionary<string, Warp>();
        public Dictionary<Key, string> keyBinds = new Dictionary<Key, string>();
        public Dictionary<string, SR2EMenuTheme> themes = new Dictionary<string, SR2EMenuTheme>();
    }
}