using Il2CppTMPro;
using SR2E.Enums;
using SR2E.Managers;

namespace SR2E.Utils;

public static class FontUtil
{
    internal static void ReloadFont(SR2EPopUp popUp) => MenuUtil.ReloadFont(popUp);
    internal static void ReloadFont(SR2EMenu menu) => MenuUtil.ReloadFont(menu);

}