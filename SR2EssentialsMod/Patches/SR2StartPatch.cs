using Il2CppMonomiPark.SlimeRancher.SceneManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SR2E.Patches;

[HarmonyPatch(typeof(SceneLoader), nameof(SceneLoader.LoadInitialSceneGroup))]
public static class SR2StartPatch
{
    public static bool Prefix()
    {
        if (SR2EEntryPoint.skipEngagementPrompt)
        {
            MelonLogger.Msg("Skipping engagement prompt!");

            var sl = SystemContext.Instance.SceneLoader;

            sl.LoadMainMenuSceneGroup();
            return false;
        }
        return true;
    }
}