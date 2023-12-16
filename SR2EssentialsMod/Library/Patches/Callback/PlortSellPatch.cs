using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SR2E.Library.Patches.Callback;

[HarmonyPatch(typeof(EconomyDirector), nameof(EconomyDirector.RegisterSold))]
static class PlortSellPatch
{
    public static void Postfix(EconomyDirector __instance, IdentifiableType id, int count)
    {
        Callbacks.Invoke_onPlortSold(count, id);
    }
}