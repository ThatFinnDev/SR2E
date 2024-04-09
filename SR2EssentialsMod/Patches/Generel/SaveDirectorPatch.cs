using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SR2E.Library.Patches;


[HarmonyPatch(typeof(AutoSaveDirector), "Awake")]
public static class SaveDirectorPatch
{
    public static void Prefix(AutoSaveDirector __instance)
    {
        SR2EEntryPoint.OnSaveDirectorLoading(__instance);
    }
    public static void Postfix()
    {
        SR2EEntryPoint.SaveDirectorLoaded();
    }
}
