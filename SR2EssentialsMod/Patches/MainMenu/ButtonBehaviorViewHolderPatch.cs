using Il2CppMonomiPark.SlimeRancher.UI.ButtonBehavior;
using Il2CppTMPro;
using UnityEngine.UI;

namespace SR2E.Patches.MainMenu;

[HarmonyPatch(typeof(ButtonBehaviorViewHolder), nameof(ButtonBehaviorViewHolder.OnEnable))]
internal class ButtonBehaviorViewHolderPatch
{
    internal static void Postfix(ButtonBehaviorViewHolder __instance)
    {
        if (__instance.name != "SaveGameSlotButton(Clone)") return;
        var tmp = __instance.gameObject.GetObjectRecursively<TextMeshProUGUI>("OptionLabel");
        tmp.enableWordWrapping = false;
        tmp.maxVisibleLines = 1;
        tmp.maxVisibleCharacters = 13;
    }
}

