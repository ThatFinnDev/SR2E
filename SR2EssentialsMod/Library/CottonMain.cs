global using static CottonLibrary.ExtentionLibrary;
using System.Linq;
using CottonLibrary;
using CottonLibrary.Patches;
using CottonLibrary.Save;
using MelonLoader;
using SR2E.Managers;
using UnityEngine;
using UnityEngine.SceneManagement;
using static CottonLibrary.Library;


namespace CottonLibrary;

public class CottonMain //: MelonMod
{
    public static void OnLateInitializeMelon()
    {
        SaveComponents.RegisterComponent(typeof(ModdedV01));
        

        foreach (MelonBase melonBase in MelonBase.RegisteredMelons)
        {
            if (melonBase is SR2EExpansionV2)
            {
                SR2EExpansionV2 mod = melonBase as SR2EExpansionV2;
                mods.Add(mod);
            }
        }
        if (Get("SR2ELibraryROOT")) { rootOBJ = Get("CottonLibraryROOT"); }
        else
        {
            rootOBJ = new GameObject();
            rootOBJ.SetActive(false);
            rootOBJ.name = "CottonLibraryROOT";
            Object.DontDestroyOnLoad(rootOBJ);
        }
    }

    public static void OnSceneWasLoaded(int buildIndex, string sceneName)
    {
        switch (sceneName)
        {
            case "GameCore":             
                MelonLogger.Msg(SR2ELanguageManger.translation("lib.registered_list"));

                foreach (SR2EExpansionV2 mod in mods)
                {          
                    MelonLogger.Msg(SR2ELanguageManger.translation("lib.registered", mod.MelonAssembly.Assembly.FullName));

                    mod.OnGameCoreLoaded();
                }
                break;
            case "ZoneCore":
                foreach (SR2EExpansionV2 mod in mods)
                    mod.OnZoneCoreLoaded();
                break;
            case "SystemCore":
                foreach (SR2EExpansionV2 mod in mods)
                    mod.OnSystemSceneLoaded();
                
                break;
            case "PlayerCore":
                foreach (SR2EExpansionV2 mod in mods)
                    mod.OnPlayerSceneLoaded();
                InitializeFX();
                break;
        }

        var pair = onSceneLoaded.FirstOrDefault(x => sceneName.Contains(x.Key));

        if (pair.Value != null)
            foreach (var action in pair.Value)
                action();
    }
}