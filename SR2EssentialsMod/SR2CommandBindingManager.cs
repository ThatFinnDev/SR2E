using UnityEngine.InputSystem;
using String = Il2CppSystem.String;

namespace SR2E
{
    internal static class SR2CommandBindingManager
    {
        private static string path = SystemContext.Instance.GetStorageProvider().Cast<FileStorageProvider>().savePath + "SR2EssentialsBinds.binds";
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
                    if(SR2Warps.warpTo==null)
                        SR2Console.ExecuteByString(keyValuePair.Value,true);
        }
    }
}