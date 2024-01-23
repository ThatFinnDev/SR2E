using Il2CppTMPro;
using UnityEngine.UI;

namespace SR2E.Patches;

//Reimplementing the void to remove the errors that occur, when the component
//is not under a canvas.
[HarmonyPatch(typeof(TextMeshProUGUI), nameof(TextMeshProUGUI.Rebuild))]
public class TextMeshProUGUIRebuildPatch
{
    public static bool Prefix(TextMeshProUGUI __instance, ref CanvasUpdate update)
    {
        try
        {
            if (__instance == null) return false;

            if (update == CanvasUpdate.Prelayout)
            {
                if (__instance.m_autoSizeTextContainer)
                    __instance.m_rectTransform.sizeDelta = __instance.GetPreferredValues(Mathf.Infinity, Mathf.Infinity);
            }
            else if (update == CanvasUpdate.PreRender)
            {
                __instance.OnPreRenderCanvas();
                if (!__instance.m_isMaterialDirty) return false;
                __instance.UpdateMaterial();
                __instance.m_isMaterialDirty = false;
            }
            return false;
        }catch { return false; }
    }
}