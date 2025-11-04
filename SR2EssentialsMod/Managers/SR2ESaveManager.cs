using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using SR2E.Enums;
using SR2E.Repos;
using SR2E.Storage;
using UnityEngine.InputSystem;

namespace SR2E.Managers;

internal static class SR2ESaveManager
{
    internal static SR2ESaveData data = null;
    internal static void Start()
    {
        Load();
    }
    static JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings
    {
        Error = (sender, args) =>
        {
            if (DebugLogging.HasFlag())
                MelonLogger.Msg($"Error: {args.ErrorContext.Error.Message}");

            // Handle broken fonts
            if (args.ErrorContext.Member is string memberName && args.ErrorContext.Path.Contains(nameof(SR2ESaveData.fonts)))
                ((Dictionary<string, SR2EMenuFont>)args.ErrorContext.OriginalObject)[memberName] = SR2EMenuFont.Default;

            // Handle broken themes
            if (args.ErrorContext.Member is string memberName2 && args.ErrorContext.Path.Contains(nameof(SR2ESaveData.themes)))
                ((Dictionary<string, SR2EMenuTheme>)args.ErrorContext.OriginalObject)[memberName2] = SR2EMenuTheme.Default;

            // Handle broken keybinds (invalid enum key)
            if (args.ErrorContext.Path.Contains(nameof(SR2ESaveData.keyBinds)) && args.ErrorContext.Member is string brokenKey)
            {
                var dict = args.ErrorContext.OriginalObject as Dictionary<LKey, string>;
                if (dict != null)
                {
                    // Try to find the invalid entry by string match (since enum parse failed)
                    var entry = dict.FirstOrDefault(kv => kv.Key.ToString() == brokenKey);
                    if (!entry.Equals(default(KeyValuePair<LKey, string>)))
                    {
                        dict.Remove(entry.Key);
                        if (DebugLogging.HasFlag())
                            MelonLogger.Msg($"Removed broken keybind: {brokenKey}");
                    }
                }
            }

            args.ErrorContext.Handled = true;
        }
    };

    internal static void Load()
    {
        var path = "";
        if (File.Exists(oldpath2)) path = oldpath2;
        if (File.Exists(oldpath1)) path = oldpath1;
        if (File.Exists(configPath)) path = configPath;

        if (string.IsNullOrWhiteSpace(path)) data = new SR2ESaveData();
        else try
            {
                data = JsonConvert.DeserializeObject<SR2ESaveData>(File.ReadAllText(path), jsonSerializerSettings);
            }
            catch (Exception e)
            {
                MelonLogger.Msg("SR2E save data is broken");
                MelonLogger.Msg(e);
                data = new SR2ESaveData();
            }
        if (File.Exists(oldpath2)) File.Delete(oldpath2);
        if (File.Exists(oldpath1)) File.Delete(oldpath1);

        if (data.keyBinds == null) data.keyBinds = new Dictionary<LKey, string>();
        if (data.warps == null) data.warps = new Dictionary<string, Warp>();
        if (data.themes == null) data.themes = new Dictionary<string, SR2EMenuTheme>();
        if (data.fonts == null) data.fonts = new Dictionary<string, SR2EMenuFont>();
        if (data.repos == null) data.repos = new List<RepoSave>();
        foreach (var pair in data.fonts)
            if (!Enum.IsDefined(typeof(SR2EMenuFont), pair.Value))
                data.fonts[pair.Key] = SR2EMenuFont.Default;
        foreach (var pair in data.themes)
            if (!Enum.IsDefined(typeof(SR2EMenuTheme), pair.Value))
                data.themes[pair.Key] = SR2EMenuTheme.Default;
        foreach (var pair in new Dictionary<LKey,String>(data.keyBinds))
            if (!Enum.IsDefined(typeof(LKey), pair.Key))
                data.keyBinds.Remove(pair.Key);
        if (data.keyBinds.ContainsKey(LKey.F11))
        {
            string cmd = data.keyBinds[LKey.F11];
            if (cmd.Contains("toggleconsole") || cmd.Contains("closeconsole") || cmd.Contains("openconsole"))
                data.keyBinds[LKey.F11] = cmd.Replace("toggleconsole", "").Replace("closeconsole", "")
                    .Replace("openconsole", "");
        }

        Save();
    }

    internal static void Save() { File.WriteAllText(configPath,JsonConvert.SerializeObject(data, Formatting.Indented)); }
    
    static string configPath => Path.Combine(SR2EEntryPoint.DataPath, "sr2e.data");
    
    
    static string oldpath2 = Path.Combine(Application.persistentDataPath, "SR2E.data");
    static string oldpath1
    {
        get
        {
            try
            {
                var provider = systemContext.GetStorageProvider();
                return provider.TryCast<FileStorageProvider>().savePath + "/SR2E.data";
            }
            catch 
            {
                return Application.persistentDataPath + "/SR2E.data";
            }
        }
    }

    public class SR2ESaveData
    {
        public Dictionary<string, Warp> warps = new Dictionary<string, Warp>();
        public Dictionary<LKey, string> keyBinds = new Dictionary<LKey, string>();
        public Dictionary<string, SR2EMenuTheme> themes = new Dictionary<string, SR2EMenuTheme>();
        public Dictionary<string, SR2EMenuFont> fonts = new Dictionary<string, SR2EMenuFont>();
        public List<RepoSave> repos = new List<RepoSave>();
    }
}