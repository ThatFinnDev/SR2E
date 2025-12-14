using SR2E.Enums;

namespace SR2E.Managers;

public static class SR2EBindingManger
{
    /// <summary>
    /// Bind a command to a key
    /// </summary>
    /// <param name="key">The key that should be bound</param>
    /// <param name="command">The command that should be executed</param>
    public static void BindKey(LKey key, string command)
    {
        if (SR2ESaveManager.data.keyBinds.ContainsKey(key)) SR2ESaveManager.data.keyBinds[key] += ";" + command;
        else SR2ESaveManager.data.keyBinds.Add(key, command);
        SR2ESaveManager.Save();
    }
    /// <summary>
    /// Unbinds every command currently bound to a key
    /// </summary>
    /// <param name="key">The key which should be unbound</param>
    public static void UnbindKey(LKey key)
    {
        if (SR2ESaveManager.data.keyBinds.ContainsKey(key)) SR2ESaveManager.data.keyBinds.Remove(key);
        SR2ESaveManager.Save();
    }
    /// <summary>
    /// Get every command separated by a semicolon which is bound to a key
    /// </summary>
    /// <param name="key">The key to be checked</param>
    /// <returns>The command</returns>
    public static string GetBind(LKey key)
    {
        if (SR2ESaveManager.data.keyBinds.ContainsKey(key)) return SR2ESaveManager.data.keyBinds[key]; return null;
    }

    /// <summary>
    /// Returns true if a key has a command bound to it
    /// </summary>
    /// <param name="key">The key to be checked</param>
    /// <returns>bool</returns>
    public static bool isKeyBound(LKey key) => SR2ESaveManager.data.keyBinds.ContainsKey(key);
    
        
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