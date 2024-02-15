using UnityEngine.InputSystem;
using String = Il2CppSystem.String;

namespace SR2E
{
    public static class SR2ECommandBindingManager
    {
        /// <summary>
        /// Adds a keybind
        /// </summary>
        public static void AddKeyBinding(Key key, string command)
        {
            if (keyCodeCommands.ContainsKey(key)) keyCodeCommands[key] += ";" + command;
            else keyCodeCommands.Add(key, command);
        }
        
        /// <summary>
        /// Removes every bind from a key
        /// </summary>
        public static void UnbindKey(Key key)
        {
            if (keyCodeCommands.ContainsKey(key)) keyCodeCommands.Remove(key);
        }
        /// <summary>
        /// Returns the execute string of the bind
        /// </summary>
        public static string GetBind(Key key)
        {
            if (keyCodeCommands.ContainsKey(key)) return keyCodeCommands[key];
            return null;
        }
        
        
        private static string path
        {
            get
            {
                FileStorageProvider provider = SystemContext.Instance.GetStorageProvider().TryCast<FileStorageProvider>();
                if (provider==null) return Application.persistentDataPath + "/" +oldPath;
                return provider.savePath + oldPath;
            }
        }
            
        private static string oldPath = "SR2EssentialsBinds.binds";

        internal static Dictionary<Key, string> keyCodeCommands = new Dictionary<Key, string>();

        internal static void Start()
        {
            if (File.Exists(oldPath))
            {
                File.WriteAllText(path, File.ReadAllText(oldPath));
                File.Delete(oldPath);
            }
            if(File.Exists(path))
                LoadKeyBinds();
        }

        internal static void SaveKeyBinds()
        {
            string safe = "";
            foreach (KeyValuePair<Key, string> keyValuePair in keyCodeCommands)
            {
                safe += keyValuePair.Key.ToString()+"\n";
                safe += keyValuePair.Value+"\n";
            }
            File.WriteAllText(path,safe);
        }

        static void LoadKeyBinds()
        {
            bool isKey = true;
            Key key = Key.None;
            foreach (string line in File.ReadAllLines(path))
            {
                if(String.IsNullOrEmpty(line))
                    continue;
                if (isKey)
                {
                    if (!Key.TryParse(line, out key))
                    { keyCodeCommands = new Dictionary<Key, string>(); break; }
                    isKey = false;
                }
                else
                {
                    keyCodeCommands.Add(key,line);
                    isKey = true; 
                }
            }
        }
        internal static void Update()
        {
            foreach (KeyValuePair<Key,string> keyValuePair in keyCodeCommands)
                if (Keyboard.current[keyValuePair.Key].wasPressedThisFrame)
                    if(SR2EWarps.warpTo==null)
                        SR2EConsole.ExecuteByString(keyValuePair.Value,true);
        }
    }
}