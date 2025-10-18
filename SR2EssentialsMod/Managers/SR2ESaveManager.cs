using System;
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
            if(DebugLogging.HasFlag()) MelonLogger.Msg($"Error: {args.ErrorContext.Error.Message}");
            if (args.ErrorContext.Member is string memberName && args.ErrorContext.Path.Contains(nameof(SR2ESaveData.fonts))) 
                ((Dictionary<string, SR2EMenuFont>)args.ErrorContext.OriginalObject)[memberName] = SR2EMenuFont.Default;
            if (args.ErrorContext.Member is string memberName2 && args.ErrorContext.Path.Contains(nameof(SR2ESaveData.themes))) 
                ((Dictionary<string, SR2EMenuTheme>)args.ErrorContext.OriginalObject)[memberName2] = SR2EMenuTheme.Default;
            args.ErrorContext.Handled = true;
        }
    };
    internal static void Load()
    {
        if (File.Exists(path))
        {
            
            try { data = JsonConvert.DeserializeObject<SR2ESaveData>(File.ReadAllText(path), jsonSerializerSettings); }
            catch (Exception e) 
            { 
                MelonLogger.Msg("SR2E save data is broken"); 
                MelonLogger.Msg(e);
                data = new SR2ESaveData();
            }
            if (data.keyBinds == null) data.keyBinds = new Dictionary<Key, string>();
            if (data.warps == null) data.warps = new Dictionary<string, Warp>();
            if(data.themes == null) data.themes = new Dictionary<string, SR2EMenuTheme>();
            if(data.fonts == null) data.fonts = new Dictionary<string, SR2EMenuFont>();
            if (data.repos == null) data.repos = new List<RepoSave>();;
            foreach (var pair in data.fonts)
                if (!Enum.IsDefined(typeof(SR2EMenuFont), pair.Value))
                    data.fonts[pair.Key] = SR2EMenuFont.Default;
            foreach (var pair in data.themes)
                if (!Enum.IsDefined(typeof(SR2EMenuTheme), pair.Value))
                    data.themes[pair.Key] = SR2EMenuTheme.Default;
            if (data.keyBinds.ContainsKey(Key.F11))
            {
                string cmd = data.keyBinds[Key.F11];
                if (cmd.Contains("toggleconsole") || cmd.Contains("closeconsole") || cmd.Contains("openconsole"))
                    data.keyBinds[Key.F11] = cmd.Replace("toggleconsole", "").Replace("closeconsole", "").Replace("openconsole", "");
            }
            Save();
        }
        else data = new SR2ESaveData();
    }
    internal static void Save() { File.WriteAllText(path,JsonConvert.SerializeObject(data, Formatting.Indented)); }

    //This variable likes to be the main character :/
    static string path
    {
        get
        {
            try
            {
                var provider = SystemContext.Instance.GetStorageProvider();
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
        public Dictionary<Key, string> keyBinds = new Dictionary<Key, string>();
        public Dictionary<string, SR2EMenuTheme> themes = new Dictionary<string, SR2EMenuTheme>();
        public Dictionary<string, SR2EMenuFont> fonts = new Dictionary<string, SR2EMenuFont>();
        public List<RepoSave> repos = new List<RepoSave>();
    }
}