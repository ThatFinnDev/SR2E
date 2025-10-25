using SR2E.Enums;

namespace SR2E.Managers;

public static class SR2EBindingManger
{
    public static void BindKey(LKey key, string command)
    {
        if (SR2ESaveManager.data.keyBinds.ContainsKey(key)) SR2ESaveManager.data.keyBinds[key] += ";" + command;
        else SR2ESaveManager.data.keyBinds.Add(key, command);
        SR2ESaveManager.Save();
    }
    public static void UnbindKey(LKey key)
    {
        if (SR2ESaveManager.data.keyBinds.ContainsKey(key)) SR2ESaveManager.data.keyBinds.Remove(key);
        SR2ESaveManager.Save();
    }
    public static string GetBind(LKey key)
    {
        if (SR2ESaveManager.data.keyBinds.ContainsKey(key)) return SR2ESaveManager.data.keyBinds[key]; return null;
    }

    public static bool isKeyBound(LKey key)
    {
        return SR2ESaveManager.data.keyBinds.ContainsKey(key);
    }
        
    internal static void Update()
    {
        try
        {
            foreach (KeyValuePair<LKey,string> keyValuePair in SR2ESaveManager.data.keyBinds)
            {
                if (keyValuePair.Key.OnKeyDown())
                {
                    if (SR2EWarpManager.warpTo == null)
                        SR2ECommandManager.ExecuteByString(keyValuePair.Value, true);
                }
            }
        }
        catch (Exception e) {MelonLogger.Error(e);}
    }
}