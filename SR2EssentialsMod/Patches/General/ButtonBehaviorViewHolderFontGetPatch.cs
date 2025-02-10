using Il2CppMonomiPark.SlimeRancher.UI.ButtonBehavior;
<<<<<<< HEAD
using Il2CppTMPro;

namespace SR2E.Patches.General;

[HarmonyPatch(typeof(ButtonBehaviorViewHolder), nameof(ButtonBehaviorViewHolder.OnEnable))]
public static class ButtonBehaviorViewHolderFontGetPatch
{
    public static void Postfix(ButtonBehaviorViewHolder __instance)
=======
using Il2CppMonomiPark.SlimeRancher.UI.MainMenu;
using Il2CppTMPro;
using UnityEngine.InputSystem;

namespace SR2E.Patches.General;

// DO NOT DISABLE THIS
[HarmonyPatch(typeof(ButtonBehaviorViewHolder), nameof(ButtonBehaviorViewHolder.OnEnable))]
internal static class ButtonBehaviorViewHolderFontGetPatch
{
    internal static void Postfix(ButtonBehaviorViewHolder __instance)
>>>>>>> experimental
    {
        if (SR2EEntryPoint.SR2Font != null) return;
        TextMeshProUGUI label = __instance.gameObject.getObjRec<TextMeshProUGUI>("Button_Label");
        if (label != null) SR2EEntryPoint.SR2Font = label.font;
        SR2EEntryPoint.SetupFonts();
        
    }
    
<<<<<<< HEAD
=======
    
>>>>>>> experimental
}