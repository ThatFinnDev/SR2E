/*using SR2E.Saving;

[HarmonyPatch(typeof(AutoSaveDirector), nameof(AutoSaveDirector.Awake))]
internal static class ExtraSlimeSavedDataPatch
{
    internal static void Postfix(AutoSaveDirector __instance)
    {
        foreach (var ident in Resources.FindObjectsOfTypeAll<IdentifiableType>())
        {
            if (ident.prefab != null)
            {
                var p = ident.prefab;
                if(p.GetComponent<SR2ESlimeDataSaver>()==null)
                    p.AddComponent<SR2ESlimeDataSaver>();
            }
        }
        foreach (var ident in Resources.FindObjectsOfTypeAll<GadgetDefinition>())
        {
            if (ident.prefab != null)
            {
                var p = ident.prefab;
                p.RemoveComponent<SR2ESlimeDataSaver>();
                if(p.GetComponent<SR2EGadgetDataSaver>()==null)
                    p.AddComponent<SR2EGadgetDataSaver>();
            }
        }
    }
}*/
//Broken as of SR2 0.6.0