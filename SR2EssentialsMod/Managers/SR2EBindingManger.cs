using Il2CppMonomiPark.SlimeRancher.Player.CharacterController;
using Il2CppMonomiPark.SlimeRancher.Regions;
using Il2CppMonomiPark.SlimeRancher.SceneManagement;
using Il2CppMonomiPark.SlimeRancher.World.Teleportation;
using Newtonsoft.Json;
using SR2E.Components;
using SR2E.Enums;
using UnityEngine.InputSystem;

namespace SR2E.Managers;

public static class SR2EBindingManger
{
    public static void BindKey(Key key, string command)
    {
        if (SR2ESaveManager.data.keyBinds.ContainsKey(key)) SR2ESaveManager.data.keyBinds[key] += ";" + command;
        else SR2ESaveManager.data.keyBinds.Add(key, command);
        SR2ESaveManager.Save();
    }
    public static void UnbindKey(Key key)
    {
        if (SR2ESaveManager.data.keyBinds.ContainsKey(key)) SR2ESaveManager.data.keyBinds.Remove(key);
        SR2ESaveManager.Save();
    }
    public static string GetBind(Key key)
    {
        if (SR2ESaveManager.data.keyBinds.ContainsKey(key)) return SR2ESaveManager.data.keyBinds[key]; return null;
    }

    public static bool isKeyBound(Key key)
    {
        return SR2ESaveManager.data.keyBinds.ContainsKey(key);
    }
        
    internal static void Update()
    {
        try
        {
            foreach (KeyValuePair<Key,string> keyValuePair in SR2ESaveManager.data.keyBinds)
                if (keyValuePair.Key.OnKeyPressed())
                    if(SR2EWarpManager.warpTo==null)
                        SR2ECommandManager.ExecuteByString(keyValuePair.Value,true);
        }
        catch (Exception e) {MelonLogger.Error(e);}
    }
}