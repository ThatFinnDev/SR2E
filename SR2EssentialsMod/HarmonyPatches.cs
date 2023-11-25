using Il2CppMonomiPark.SlimeRancher.Event.Query;

namespace SR2E
{
    internal class HarmonyPatches
    {
        //test for adding unlock all portals command
        /*
            [HarmonyPatch(typeof(QueryToggle))]
            [HarmonyPatch("Awake")]
            public static class QueryTogglePatch
            {
                public static void Postfix(QueryToggle __instance)
                {
                    for (int i = 0; i < __instance.transform.childCount; i++)
                    {
                        if (__instance.transform.GetChild(i).gameObject.name == "TeleportTrigger")
                        {
                            __instance.transform.GetChild(i).gameObject.SetActive(true);
                        }
                    }
                }
            }
        */
    }
}