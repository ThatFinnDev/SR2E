using System.Linq;
using System.Reflection;
using System;
using SR2E.Enums;
using SR2E.Managers;
using SR2E.Storage;

namespace SR2E.Patches.Context;

[HarmonyPatch(typeof(SystemContext), nameof(SystemContext.Start))]
internal class SystemContextPatch
{
    internal static AssetBundle bundle = null;
    
    static List<Object> assets = new List<Object>(); //Prefabs are destroyed
    const string menuPath = "Assets/Menus/";
    const string popUpPath = "Assets/PopUps/";
    const string prefabSuffix = ".prefab";
    internal static string getPopUpPath(string identifier,SR2EMenuTheme currentTheme)
    {
        //now, currentTheme exists
        string extraTheme = "";
        if (currentTheme != SR2EMenuTheme.Default) extraTheme = "_"+currentTheme.ToString().Split(".")[0];
        return $"{popUpPath}{identifier}{extraTheme}{prefabSuffix}";
    }
    internal static string getMenuPath(MenuIdentifier menuIdentifier)
    {
        SR2ESaveManager.data.themes.TryAdd(menuIdentifier.saveKey, menuIdentifier.defaultTheme);
        SR2EMenuTheme currentTheme = SR2ESaveManager.data.themes[menuIdentifier.saveKey];
        List<SR2EMenuTheme> validThemes = MenuEUtil.GetValidThemes(menuIdentifier.saveKey);
        if (validThemes.Count == 0) return null;
        if(!validThemes.Contains(currentTheme)) currentTheme = validThemes.First();
        SR2ESaveManager.Save();
        //now, currentTheme exists
        string extraTheme = "";
        if (currentTheme != SR2EMenuTheme.Default) extraTheme = "_"+currentTheme.ToString().Split(".")[0];
        return $"{menuPath}{menuIdentifier.saveKey}{extraTheme}{prefabSuffix}";
    }
    internal static void Postfix(SystemContext __instance)
    {
        bundle = EmbeddedResourceEUtil.LoadBundle("srtwoessentials.assetbundle");
        foreach (string path in bundle.GetAllAssetNames())
        {
            var asset = bundle.LoadAsset(path);
            assets.Add(asset);
            if (path.StartsWith(menuPath, StringComparison.OrdinalIgnoreCase))
                if(path.EndsWith(prefabSuffix, StringComparison.OrdinalIgnoreCase))
                {
                    string menu = path.Substring(menuPath.Length, path.Length - menuPath.Length - prefabSuffix.Length);
                    SR2EMenuTheme theme = SR2EMenuTheme.Default;
                    var split = menu.Split("_");
                    var key = split[0];
                    if (menu.Contains("_"))
                    {
                        if (Enum.TryParse(typeof(SR2EMenuTheme), split[1], true, out object result))
                            theme = (SR2EMenuTheme)result;
                        else continue;
                    }
                    if (!MenuEUtil.validThemes.ContainsKey(key)) MenuEUtil.validThemes.Add(key,new List<SR2EMenuTheme>());
                    MenuEUtil.validThemes[key].Add(theme);
                }
        }
        foreach (var obj in assets)
            if (obj != null)
                if (obj.name == "AllMightyMenus")
                { Object.Instantiate(obj); break; }
        
        var lang = SystemContext.Instance.LocalizationDirector.GetCurrentLocaleCode();
                    
        LoadLanguage(lang);
        
        foreach (var expansion in SR2EEntryPoint.expansionsAll)
            try { expansion.OnSystemContext(__instance); } 
            catch (Exception e) { MelonLogger.Error(e); }
    }
}