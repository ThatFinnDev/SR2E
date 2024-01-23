using SR2E.Saving;

[HarmonyPatch(typeof(AutoSaveDirector), nameof(AutoSaveDirector.Awake))]
public static class ExtraSlimeSavedDataPatch
{
    public static void Postfix(AutoSaveDirector __instance)
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
}