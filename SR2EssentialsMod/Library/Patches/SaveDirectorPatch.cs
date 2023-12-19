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

        slimes = Get<IdentifiableTypeGroup>("SlimesGroup");
        baseSlimes = Get<IdentifiableTypeGroup>("BaseSlimeGroup");
        largos = Get<IdentifiableTypeGroup>("LargoGroup");
        meat = Get<IdentifiableTypeGroup>("MeatGroup");
        food = Get<IdentifiableTypeGroup>("FoodGroup");
        veggies = Get<IdentifiableTypeGroup>("VeggieGroup");
        fruits = Get<IdentifiableTypeGroup>("FruitGroup");
        plorts = Get<IdentifiableTypeGroup>("PlortGroup");
        crafts = Get<IdentifiableTypeGroup>("CraftGroup");

        if (SR2EEntryPoint.chaosMode)
            ChaosMode.OnSaveDirectorLoading(__instance);
        foreach (SR2EMod lib in mods)
        {
            lib.SaveDirectorLoading(__instance);
        }
    }
    public static void Postfix()
    {
        foreach (SR2EMod lib in mods)
        {
            lib.SaveDirectorLoaded();
        }
    }
}
