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
    public static AutoSaveDirector autoSaveDirector => GameContext.Instance.AutoSaveDirector;

    public static bool inGame
    {
        get
        {
            try
            {
                if (SceneContext.Instance == null) return false;
                if (SceneContext.Instance.PlayerState == null) return false;
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
}