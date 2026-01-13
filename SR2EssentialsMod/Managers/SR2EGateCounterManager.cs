using SR2E.Expansion;
using SR2E.Patches.Context;

namespace SR2E.Managers;

public static class SR2ECounterGateManager
{
    internal static List<object> useOcclusionCullingList = new List<object>();
    internal static List<object> disableCheatsList = new List<object>();
    public static bool playerCameraUseOcclusionCulling => useOcclusionCullingList.Count == 0;
    public static bool disableCheats => disableCheatsList.Count != 0;

    internal static void OnSceneWasLoaded(int buildIndex, string sceneName)
    {
        RefreshOcclusionCulling();
    }
    static void RefreshDisableCheats()
    {
        try
        {
            if(GameContextPatch.cheatMenuButton!=null)
                if(disableCheats) 
                    GameContextPatch.cheatMenuButton.Remove();
                else
                    GameContextPatch.cheatMenuButton.AddAgain();
        }
        catch 
        { }
    }
    static void RefreshOcclusionCulling()
    {
        try
        {
            foreach (var cam in Camera.allCameras)
                if(cam.name.Contains("Player")||cam.name.Contains("SRLECamera"))
                    cam.useOcclusionCulling = playerCameraUseOcclusionCulling;
        }
        catch 
        { }
    }
    public static void RegisterFor_PlayerCameraDisableUseOcclusionCulling(this SR2EExpansionV3 expansionV3)
    {
        if (!useOcclusionCullingList.Contains(expansionV3)) useOcclusionCullingList.Add(expansionV3);
        RefreshOcclusionCulling();
    }
    public static void DeregisterFor_PlayerCameraDisableUseOcclusionCulling(this SR2EExpansionV3 expansionV3)
    {
        if (!useOcclusionCullingList.Contains(expansionV3)) useOcclusionCullingList.Remove(expansionV3);
        RefreshOcclusionCulling();
    }
    public static void RegisterFor_PlayerCameraDisableUseOcclusionCulling(this MelonMod mod)
    {
        if (!useOcclusionCullingList.Contains(mod)) useOcclusionCullingList.Add(mod);
        RefreshOcclusionCulling();
    }
    public static void DeregisterFor_PlayerCameraDisableUseOcclusionCulling(this MelonMod mod)
    {
        if (!useOcclusionCullingList.Contains(mod)) useOcclusionCullingList.Remove(mod);
        RefreshOcclusionCulling();
    }
    
    
    
    
    public static void RegisterFor_DisableCheats(this SR2EExpansionV3 expansionV3)
    {
        if (!disableCheatsList.Contains(expansionV3)) disableCheatsList.Add(expansionV3);
        RefreshDisableCheats();
    }
    public static void DeregisterFor_DisableCheats(this SR2EExpansionV3 expansionV3)
    {
        if (!disableCheatsList.Contains(expansionV3)) disableCheatsList.Remove(expansionV3);
        RefreshDisableCheats();
    }
    public static void RegisterFor_DisableCheats(this MelonMod mod)
    {
        if (!disableCheatsList.Contains(mod)) disableCheatsList.Add(mod);
        RefreshDisableCheats();
    }
    public static void DeregisterFor_DisableCheats(this MelonMod mod)
    {
        if (!disableCheatsList.Contains(mod)) disableCheatsList.Remove(mod);
        RefreshDisableCheats();
    }
}