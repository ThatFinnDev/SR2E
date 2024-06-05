using Il2CppMonomiPark.SlimeRancher.UI.ButtonBehavior;
using Il2CppTMPro;

namespace SR2E.Patches.General;

[HarmonyPatch(typeof(ButtonBehaviorViewHolder), nameof(ButtonBehaviorViewHolder.OnEnable))]
public static class ButtonBehaviorViewHolderFontGetPatch
{
    public static void Postfix(ButtonBehaviorViewHolder __instance)
    {
        if (SR2EEntryPoint.SR2Font != null) return;
        TextMeshProUGUI label = __instance.gameObject.getObjRec<TextMeshProUGUI>("Button_Label");
        if (label != null) SR2EEntryPoint.SR2Font = label.font;
        SR2EEntryPoint.SetupFonts();
        
    }
    
}