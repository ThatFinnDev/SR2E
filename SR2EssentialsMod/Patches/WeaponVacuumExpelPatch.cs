using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SR2E.Patches;

[HarmonyPatch(typeof(WeaponVacuum), nameof(WeaponVacuum.Expel), typeof(GameObject), typeof(bool), typeof(float), typeof(SlimeAppearance.AppearanceSaveSet))]
internal class WeaponVacuumExpelPatch
{
    public static void Prefix(WeaponVacuum __instance, ref bool ignoreEmotions)
    {
        if (__instance._player.Ammo.GetSelectedEmotions() == null)
            ignoreEmotions = true;
    }
}