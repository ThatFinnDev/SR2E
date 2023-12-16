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
                var dataSaver = p.AddComponent<SR2ESlimeDataSaver>();
            }
        }
    }
}