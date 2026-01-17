using System.Linq;
using System.Reflection;
using System;
using Il2CppInterop.Runtime.Injection;
using SR2E.Enums;
using SR2E.Managers;
using SR2E.Storage;

namespace SR2E.Patches.Context;

[HarmonyPatch(typeof(SystemContext), nameof(SystemContext.Start))]
internal class SystemContextPatch
{
    internal static bool didStart = false;
    internal static Dictionary<string, Shader> loadedShaders = new ();
    internal static Il2CppAssetBundle bundle = null;
    internal static Dictionary<string, Type> menusToInit = new ();
    
    static List<Object> assets = new (); //Prefabs are destroyed
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

    internal static void Prefix()
    {
        didStart = true;
    }
    internal static void Postfix(SystemContext __instance)
    {
        if(ChangeSystemContextIsModded.HasFlag()) SystemContext.IsModded = true;
        bundle = EmbeddedResourceEUtil.LoadIl2CppBundle("Assets.srtwoessentials.assetbundle");
        foreach (string path in bundle.GetAllAssetNames())
        {
            var asset = bundle.LoadAsset(path);
            if (asset.TryCast<Shader>()!=null)
            {
                var shader = asset.Cast<Shader>();
                shader.hideFlags |= HideFlags.DontUnloadUnusedAsset;
                loadedShaders[asset.name] = shader;

            }
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
                {
                    var instance = Object.Instantiate(obj).TryCast<GameObject>();
                    SR2ELogManager.Start();
                    SR2ESaveManager.Start();
                    SR2ECommandManager.Start();
                    SR2ERepoManager.Start();
                    SR2EEntryPoint.SR2EStuff = instance;
                    instance.name = "SR2EStuff";
                    instance.SetActive(false);
                    GameObject.DontDestroyOnLoad(instance);
                    
                    ExecuteInTicks(() =>
                    {
                        foreach (var melonBase in MelonBase.RegisteredMelons)
                        {
                            var exporters = melonBase.MelonAssembly.Assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(SR2EMenu)) && !t.IsAbstract);
                            foreach (Type type in exporters)
                                try
                                {
                                    var identifier = type.GetMenuIdentifierByType();
                                    if (!string.IsNullOrWhiteSpace(identifier.saveKey))
                                    {
                                        var path = getMenuPath(identifier);
                                        bool assetEmpty = true;
                                        if (!string.IsNullOrWhiteSpace(path)) assetEmpty = !bundle.Contains(path);

                                        UnityEngine.Object rootObject = type.GetMenuRootObject();
                                        
                                        if (!assetEmpty&&rootObject==null)
                                        {
                                            rootObject = GameObject.Instantiate(bundle.LoadAsset(path), instance.transform);
                                        }
                                        if (rootObject == null)
                                        {
                                            var message = $"The menu under the name {type.Name} couldn't be loaded! It's root object is null!";
                                            MelonLogger.Error(message);
                                            throw new Exception(message);
                                        }

                                        rootObject.Cast<GameObject>().transform.SetParent(instance.transform);
                                        menusToInit.Add(rootObject.name, type);
                                        if (!ClassInjector.IsTypeRegisteredInIl2Cpp(type))
                                            ClassInjector.RegisterTypeInIl2Cpp(type,
                                                new RegisterTypeOptions() { LogSuccess = false });

                                    }
                                    else MelonLogger.Error($"The menu under the name {type.Name} couldn't be loaded! It's MenuIdentifier is broken!");

                                }
                                catch (Exception e) { MelonLogger.Error(e); }
                        }

                        ExecuteInTicks(() =>
                        {
                            instance.SetActive(true);
                            foreach (var pair in new Dictionary<string, Type>(menusToInit))
                            foreach (var child in instance.GetChildren())
                                if (child.name == pair.Key)
                                {
                                    try
                                    {
                                        child.AddComponent(pair.Value);
                                        child.gameObject.SetActive(true);
                                        menusToInit.Remove(pair.Key);
                                    }
                                    catch (Exception e) { MelonLogger.Error(e); }
                                }
                            SR2EEntryPoint.menusFinished = true;
                        }, 1);
                    },1);
                    break;
                }

        var lang = __instance.LocalizationDirector.GetCurrentLocaleCode();
                    
        LoadLanguage(lang);
        
        foreach (var expansion in SR2EEntryPoint.expansionsV1V2)
            try { expansion.OnSystemContext(__instance); } 
            catch (Exception e) { MelonLogger.Error(e); }
        foreach (var expansion in SR2EEntryPoint.expansionsV3)
            try { expansion.AfterSystemContext(__instance); } 
            catch (Exception e) { MelonLogger.Error(e); }
        SR2ECallEventManager.ExecuteWithArgs(CallEvent.AfterSystemContextLoad, ("systemContext", __instance));
    }
}