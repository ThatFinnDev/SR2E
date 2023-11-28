using HarmonyLib;

namespace SR2E;

[HarmonyPatch(typeof(WeaponVacuum), nameof(WeaponVacuum.Expel), typeof(GameObject), typeof(bool), typeof(float), typeof(SlimeAppearance.AppearanceSaveSet))]
internal class WeaponVacuumExpelPatch
{
    public static void Prefix(WeaponVacuum __instance, ref bool ignoreEmotions)
    {
        if (__instance._player.Ammo.GetSelectedEmotions() == null)
        {
            ignoreEmotions = true;
        }
    }
}