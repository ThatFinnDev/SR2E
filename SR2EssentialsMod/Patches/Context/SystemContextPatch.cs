using System.Reflection;
using Il2CppMonomiPark.SlimeRancher.Damage;

namespace SR2E.Patches.Context;

[HarmonyPatch(typeof(SystemContext), nameof(SystemContext.Start))]
internal class SystemContextPatch
{
    internal static void Postfix(SystemContext __instance)
    {
        System.IO.Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("SR2E.srtwoessentials.assetbundle");
        byte[] buffer = new byte[16 * 1024];
        System.IO.MemoryStream ms = new System.IO.MemoryStream();
        int read;
        while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
            ms.Write(buffer, 0, read);

        var bundle = AssetBundle.LoadFromMemory(ms.ToArray());
        foreach (var obj in bundle.LoadAllAssets())
        {
            if (obj != null)
                if (obj.name == "AllMightyMenus")
                {
                    Object.Instantiate(obj);
                    break;
                }
        }

        var lang = SystemContext.Instance.LocalizationDirector.GetCurrentLocaleCode();
                    
        if (languages.ContainsKey(lang))
            LoadLanguage(lang);
        else
            LoadLanguage("en");
        
        foreach (var expansion in SR2EEntryPoint.expansions)
            try
            {
                expansion.OnSystemContext(__instance);
            } 
            catch (Exception e) { MelonLogger.Error(e); }
    }
}