using Il2CppMonomiPark.SlimeRancher.UI;
namespace SR2E.Patches.General
{
    [HarmonyPatch(typeof(BaseUI), "Awake")]
    public class FixInputActionsPatch
    {
        public static void Postfix(BaseUI __instance)
        {
            foreach (var input in PausedActions.Values)
                input.Enable();
        } 
    }
}
