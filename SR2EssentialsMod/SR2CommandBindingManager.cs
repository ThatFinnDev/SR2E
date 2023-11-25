using UnityEngine.InputSystem;
using String = Il2CppSystem.String;

namespace SR2E
{
    internal static class SR2CommandBindingManager
    {
        internal static Dictionary<Key, string> keyCodeCommands = new Dictionary<Key, string>();

        internal static void Start()
        {
            if (File.Exists("SR2EssentialsBinds.binds"))
            {
                LoadKeyBinds();
            }
        }

        internal static void SaveKeyBinds()
        {
            string safe = "";
            foreach (KeyValuePair<Key, string> keyValuePair in keyCodeCommands)
            {
                safe += keyValuePair.Key.ToString()+"\n";
                safe += keyValuePair.Value+"\n";
            }
            File.WriteAllText("SR2EssentialsBinds.binds",safe);
        }

        static void LoadKeyBinds()
        {
            bool isKey = true;
            Key key = Key.None;
            foreach (string line in File.ReadAllLines("SR2EssentialsBinds.binds"))
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
                    if (!SR2Console.isOpen)
                        if (!SR2ModMenu.isOpen)
                            if (Time.timeScale != 0)
                                if(SR2Warps.warpTo==null)
                                    SR2Console.ExecuteByString(keyValuePair.Value);
        }
    }
}