﻿global using static SR2E.Managers.SR2EInputManager;
using System;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using System.Linq;
using System.Reflection;
using Il2CppMonomiPark.SlimeRancher.Damage;
using Il2CppMonomiPark.SlimeRancher.DataModel;
using Il2CppMonomiPark.SlimeRancher.Script.UI.Pause;
using Il2CppMonomiPark.SlimeRancher.Script.Util;
using Il2CppMonomiPark.SlimeRancher.UI;
using Il2CppMonomiPark.SlimeRancher.UI.MainMenu;
using UnityEngine.Localization;
using UnityEngine.Localization.Tables;
using Il2CppMonomiPark.SlimeRancher.Weather;
using Il2CppMonomiPark.SlimeRancher.World;
using Il2CppSystem.IO;
using Il2CppTMPro;
using SR2E.Enums;
using SR2E.Managers;
using SR2E.Menus;
using UnityEngine.InputSystem;
using SR2E.Storage;
using Unity.Mathematics;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.UI;
using UnityEngine.Playables;
using UnityEngine.TextCore;
using UnityEngine.TextCore.LowLevel;
using Enumerable = Il2CppSystem.Linq.Enumerable;

namespace SR2E
{
    public static class SR2EUtils
    {
        internal static Dictionary<string, InputActionMap> actionMaps = new Dictionary<string, InputActionMap>();
        internal static Dictionary<string, InputAction> MainGameActions = new Dictionary<string, InputAction>();
        internal static Dictionary<string, InputAction> PausedActions = new Dictionary<string, InputAction>();
        internal static Dictionary<string, InputAction> DebugActions = new Dictionary<string, InputAction>();
        public static WeatherStateDefinition[] weatherStateDefinitions => Resources.FindObjectsOfTypeAll<WeatherStateDefinition>();

        internal static GameObject rootOBJ;

        public static GameObject? player;

        // public enum VanillaPediaEntryCategories { TUTORIAL, SLIMES, RESOURCES, WORLD, RANCH, SCIENCE, WEATHER }
        public static SystemContext systemContext => SystemContext.Instance;
        public static GameContext gameContext => GameContext.Instance;
        public static SceneContext sceneContext => SceneContext.Instance;
        internal static Damage _killDamage;
        public static Damage killDamage => _killDamage;

        public static WeatherStateDefinition getWeatherStateByName(string name)
        {
            foreach (WeatherStateDefinition state in weatherStateDefinitions)
                try
                {
                    if (state.name.ToUpper().Replace(" ", "") == name.ToUpper())
                        return state;
                }
                catch (System.Exception ignored)
                { }
            return null;
        }

        public static string LoadTextFile(string name)
        {
            System.IO.Stream stream = Assembly.GetCallingAssembly().GetManifestResourceStream(name);
            byte[] buffer = new byte[16 * 1024];
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            int read;
            while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
                ms.Write(buffer, 0, read);
            return System.Text.Encoding.Default.GetString(ms.ToArray());

        }

        public static Il2CppArrayBase<WeatherStateDefinition> weatherStates => GameContext.Instance.AutoSaveDirector.weatherStates.items.ToArray();
        public static WeatherStateDefinition WeatherState(string name) => weatherStates.FirstOrDefault((WeatherStateDefinition x) => x.name == name);


        public static List<LocalizedString> createdTranslations = new List<LocalizedString>();
        
        public static Dictionary<string, Dictionary<string, string>> addedTranslations = new Dictionary<string, System.Collections.Generic.Dictionary<string, string>>();

        internal static Dictionary<string, LocalizedString> sr2etosrlanguage = new Dictionary<string, LocalizedString>();
        internal static Dictionary<string, (string, string, LocalizedString)> sr2eReplaceOnLanguageChange = new Dictionary<string, (string, string, LocalizedString)>();
        public static LocalizedString AddTranslation(string localized, string key = "l.SR2ETest", string table = "Actor")
        {
            if (!InjectTranslations.HasFlag())
            {
                var tutorial = LocalizationUtil.GetTable("Tutorial");
                foreach (var pair in tutorial.m_TableEntries) return new LocalizedString(tutorial.SharedData.TableCollectionName, pair.Value.SharedEntry.Id);
            }
            StringTable table2 = LocalizationUtil.GetTable(table);


            StringTableEntry existing = null;
            try { existing = table2.GetEntry(key); } catch { }
            if (existing != null) return new LocalizedString(table2.SharedData.TableCollectionName, existing.SharedEntry.Id);
            System.Collections.Generic.Dictionary<string, string> dictionary;
            if (!addedTranslations.TryGetValue(table, out dictionary))
            {
                dictionary = new System.Collections.Generic.Dictionary<string, string>();

                addedTranslations.Add(table, dictionary);
            }

            dictionary.Add(key, localized);
            StringTableEntry stringTableEntry = table2.AddEntry(key, localized);
            return new LocalizedString(table2.SharedData.TableCollectionName, stringTableEntry.SharedEntry.Id);
        }
        public static LocalizedString AddTranslationFromSR2E(string sr2eTranslationID, string key = "l.SR2ETest", string table = "Actor")
        {
            LocalizedString localizedString = AddTranslation(translation(sr2eTranslationID), key, table);
            
            sr2etosrlanguage.TryAdd(sr2eTranslationID,localizedString);
            sr2eReplaceOnLanguageChange.TryAdd(sr2eTranslationID, (key, table, localizedString));
            
            return localizedString;
        }
        
        public static void SetTranslation(string localized, string key = "l.SR2ETest", string table = "Actor")
        {
            if (!InjectTranslations.HasFlag()) return;
            
            StringTable table2 = LocalizationUtil.GetTable(table);
            
            table2.GetEntry(key).Value = localized;
        }
        public static void SetTranslationFromSR2E(string sr2eTranslationID, string key = "l.SR2ETest", string table = "Actor")
        {
            SetTranslation(translation(sr2eTranslationID), key, table);
        }
        public static GameObject SpawnGadget(this GadgetDefinition def, Vector3 pos) => SpawnGadget(def, pos, Quaternion.identity);
        public static GameObject SpawnGadget(this GadgetDefinition def, Vector3 pos, Vector3 rot)=> SpawnGadget(def, pos, Quaternion.Euler(rot));
        public static GameObject SpawnGadget(this GadgetDefinition def, Vector3 pos, Quaternion rot)
        {
            throw new Exception("Currently broken");
            GameObject gadget = GadgetDirector.InstantiateGadget(def.prefab, SRSingleton<SceneContext>.Instance.RegionRegistry.CurrentSceneGroup, pos, rot);
            return gadget;
        }
        public static GameObject SpawnActor(this IdentifiableType ident, Vector3 pos) => SpawnActor(ident, pos, Quaternion.identity);
        public static GameObject SpawnActor(this IdentifiableType ident, Vector3 pos, Vector3 rot)=> SpawnActor(ident, pos, Quaternion.Euler(rot));
        public static GameObject SpawnActor(this IdentifiableType ident, Vector3 pos, Quaternion rot)
        {
            if (ident is GadgetDefinition gadgetDefinition) return SpawnGadget(gadgetDefinition, pos, rot);
            return InstantiationHelpers.InstantiateActor(ident.prefab,
                SRSingleton<SceneContext>.Instance.RegionRegistry.CurrentSceneGroup, pos, rot,
                false, SlimeAppearance.AppearanceSaveSet.NONE, SlimeAppearance.AppearanceSaveSet.NONE);
        }
        public static GameObject SpawnDynamic(this GameObject obj, Vector3 pos, Quaternion rot)
        {
            return InstantiationHelpers.InstantiateDynamic(obj, pos, rot);
        }
        
        public static GameObject SpawnFX(this GameObject fx, Vector3 pos) => SpawnFX(fx, pos, Quaternion.identity);
        
        public static GameObject SpawnFX(this GameObject fx, Vector3 pos, Quaternion rot)
        {
            return FXHelpers.SpawnFX(fx, pos, rot);
        }
        public static T? Get<T>(string name) where T : Object => Resources.FindObjectsOfTypeAll<T>().FirstOrDefault((T x) => x.name == name);
  
        public static Sprite ConvertToSprite(this Texture2D texture)
        {
            return Sprite.Create(texture, new Rect(0f, 0f, (float)texture.width, (float)texture.height), new Vector2(0.5f, 0.5f), 1f);
        }
        public static GameObject CopyObject(this GameObject obj) => Object.Instantiate(obj, rootOBJ.transform);
        internal static TMP_FontAsset FontFromGame(string name)
        {
            try
            {
                return Get<TMP_FontAsset>(name);
            }
            catch { SR2EEntryPoint.SendFontError(name); }

            return null;
        }
        internal static TMP_FontAsset FontFromOS(string name)
        {
            try
            { 
                string path = $"C:\\Windows\\Fonts\\{name}.ttf";
                if(!File.Exists(path)) throw new Exception();
                FontEngine.InitializeFontEngine();
                if (FontEngine.LoadFontFace(path, 90) != FontEngineError.Success) throw new Exception();
                TMP_FontAsset fontAsset = ScriptableObject.CreateInstance<TMP_FontAsset>();
                fontAsset.m_Version = "1.1.0";
                fontAsset.faceInfo = FontEngine.GetFaceInfo();
                fontAsset.sourceFontFile = Font.CreateDynamicFontFromOSFont(name, 16);
                fontAsset.atlasPopulationMode = AtlasPopulationMode.Dynamic;
                fontAsset.atlasWidth = 1024;
                fontAsset.atlasHeight = 1024;
                fontAsset.atlasPadding = 9;
                fontAsset.atlasRenderMode = GlyphRenderMode.SDFAA;
                fontAsset.atlasTextures = new Texture2D[1];
                Texture2D texture = new Texture2D(0, 0, TextureFormat.Alpha8, false);
                fontAsset.atlasTextures[0] = texture;
                fontAsset.isMultiAtlasTexturesEnabled = true;
                Material material = new Material(ShaderUtilities.ShaderRef_MobileSDF);
                material.SetTexture(ShaderUtilities.ID_MainTex, texture);
                material.SetFloat(ShaderUtilities.ID_WeightNormal, fontAsset.normalStyle); 
                material.SetFloat(ShaderUtilities.ID_WeightBold, fontAsset.boldStyle);
                material.SetFloat(ShaderUtilities.ID_TextureHeight, 1024);
                material.SetFloat(ShaderUtilities.ID_TextureWidth, 1024);
                material.SetFloat(ShaderUtilities.ID_GradientScale, 10);
                fontAsset.material = material;
                fontAsset.freeGlyphRects = new Il2CppSystem.Collections.Generic.List<GlyphRect>(8);
                fontAsset.freeGlyphRects.Add(new GlyphRect(0, 0, 1023, 1023));
                fontAsset.usedGlyphRects = new Il2CppSystem.Collections.Generic.List<GlyphRect>(8);
                fontAsset.ReadFontAssetDefinition();
                return fontAsset;
            }
            catch { SR2EEntryPoint.SendFontError(name); }
            return null;
        }
        internal static void ReloadFont(this SR2EMenu menu)
        {
            var ident = menu.GetIdentifierViaReflection();
            if (string.IsNullOrEmpty(ident.saveKey)) return;
            if(SR2ESaveManager.data.fonts.TryAdd(ident.saveKey, ident.defaultFont)) SR2ESaveManager.Save();
            var dataFont = SR2ESaveManager.data.fonts[ident.saveKey];
            TMP_FontAsset fontAsset = null;
            switch (dataFont)
            {
                case SR2EMenuFont.Default: fontAsset = SR2EEntryPoint.normalFont; break;
                case SR2EMenuFont.Bold: fontAsset = SR2EEntryPoint.boldFont; break;
                case SR2EMenuFont.Regular: fontAsset = SR2EEntryPoint.regularFont; break;
                case SR2EMenuFont.SR2: fontAsset = SR2EEntryPoint.SR2Font; break;
            }
            if(fontAsset!=null) menu.ApplyFont(fontAsset);
        }
        internal static MenuIdentifier GetIdentifierViaReflection(this SR2EMenu menu) => menu.GetType().GetIdentifierViaReflection();
        
        internal static MenuIdentifier GetIdentifierViaReflection(this Type type)
        {
            try
            {
                var methodInfo = type.GetMethod(nameof(SR2EMenu.GetMenuIdentifier), BindingFlags.Static | BindingFlags.Public);
                var result = methodInfo.Invoke(null, null);
                if(result is MenuIdentifier identifier) return identifier;
            }
            catch (Exception e) { MelonLogger.Error(e); }
            return new MenuIdentifier(); 
        }
        internal static SR2EMenu GetSR2EMenu(this MenuIdentifier identifier)
        {
            try
            {
                foreach (var pair in SR2EEntryPoint.menus)
                {
                    var ident = pair.Key.GetIdentifierViaReflection();
                    if (ident.saveKey == identifier.saveKey) return pair.Key;
                }
            }
            catch (Exception e) { MelonLogger.Error(e); }
            return null; 
        }
        public static GameObject? Get(string name) => Get<GameObject>(name);

        public static void MakePrefab(this GameObject obj)
        {
            UnityEngine.Object.DontDestroyOnLoad(obj);
            obj.transform.parent = rootOBJ.transform;
        }
        internal static void DoActions(List<MenuActions> actions)
        {
            if(actions.Contains(MenuActions.UnPauseGame)) TryUnPauseGame();
            if(actions.Contains(MenuActions.UnPauseGameFalse)) TryUnPauseGame(false);
            if(actions.Contains(MenuActions.PauseGameFalse)) TryPauseGame(false);
            if(actions.Contains(MenuActions.UnHideMenus)) TryUnHideMenus();
            if(actions.Contains(MenuActions.EnableInput)) TryEnableSR2Input();
            if(actions.Contains(MenuActions.DisableInput)) TryDisableSR2Input();
            if(actions.Contains(MenuActions.PauseGame)&&actions.Contains(MenuActions.HideMenus)) TryPauseAndHide();
            else
            {
                if(actions.Contains(MenuActions.HideMenus)) TryHideMenus();
                if(actions.Contains(MenuActions.PauseGame)) TryPauseGame();
            }

        }

        internal static Sprite LoadSprite(string fileName) => ConvertToSprite(LoadImage(fileName));

        internal static Texture2D LoadImage(string filename)
        {
            Assembly executingAssembly = Assembly.GetExecutingAssembly();
            System.IO.Stream manifestResourceStream = executingAssembly.GetManifestResourceStream(executingAssembly.GetName().Name + "." + filename + ".png");
            byte[] array = new byte[manifestResourceStream.Length];
            manifestResourceStream.Read(array, 0, array.Length);
            Texture2D texture2D = new Texture2D(1, 1);
            ImageConversion.LoadImage(texture2D, array);
            texture2D.filterMode = FilterMode.Bilinear;
            return texture2D;
        }


        internal static Dictionary<MelonPreferences_Entry, System.Action> entriesWithActions = new Dictionary<MelonPreferences_Entry, Action>();
        public static void AddNullAction(this MelonPreferences_Entry entry) => entriesWithActions.Add(entry, null);
        public static void AddAction(this MelonPreferences_Entry entry, System.Action action) => entriesWithActions.Add(entry, action);
        internal static Dictionary<string, List<SR2EMenuTheme>> validThemes = new Dictionary<string, List<SR2EMenuTheme>>();
        public static List<SR2EMenuTheme> getValidThemes(string saveKey)
        {
            if (validThemes.ContainsKey(saveKey.ToLower()))
                return validThemes[saveKey.ToLower()];
            return new List<SR2EMenuTheme>();
        }
        public static bool AddComponent<T>(this Transform obj) where T : Component => obj.gameObject.AddComponent<T>();
        public static bool HasComponent<T>(this Transform obj) where T : Component => HasComponent<T>(obj.gameObject);
        public static bool HasComponent<T>(this GameObject obj) where T : Component
        {
            try
            {
                return obj.GetComponent<T>()!=null;
            }
            catch { return false; }
        }
        public static bool RemoveComponent<T>(this Transform obj) where T : Component => RemoveComponent<T>(obj.gameObject);
        public static bool RemoveComponent<T>(this GameObject obj) where T : Component
        {
            try
            {
                T comp = obj.GetComponent<T>();
                var check = comp.gameObject;
                Object.Destroy(comp);
                return true;
            }
            catch { return false; }
        }
        public static T GM<T>() where T : SR2EMenu
        {
            foreach (var pair in SR2EEntryPoint.menus)
                if (pair.Key is T) return (T)pair.Key;
            return null;
        }
        public static Il2CppSystem.Type il2cppTypeof(this Type type)
        {
            string typeName = type.AssemblyQualifiedName;

            if (typeName.ToLower().StartsWith("il2cpp"))
            {
                typeName = typeName.Substring("il2cpp".Length);
            }

            Il2CppSystem.Type il2cppType = Il2CppSystem.Type.GetType(typeName);

            return il2cppType;
        }

        public static T getObjRec<T>(this GameObject obj, string name) where T : class
        {
            var transform = obj.transform;

            List<GameObject> totalChildren = getAllChildren(transform);
            for (int i = 0; i < totalChildren.Count; i++)
                if (totalChildren[i].name == name)
                {
                    if (typeof(T) == typeof(GameObject))
                        return totalChildren[i] as T;
                    if (typeof(T) == typeof(Transform))
                        return totalChildren[i].transform as T;
                    if (totalChildren[i].GetComponent<T>() != null)
                        return totalChildren[i].GetComponent<T>();
                }
            return null;
        }
        public static T getObjRec<T>(this Transform transform, string name) where T : class
        {
            List<GameObject> totalChildren = getAllChildren(transform);
            for (int i = 0; i < totalChildren.Count; i++)
                if (totalChildren[i].name == name)
                {
                    if (typeof(T) == typeof(GameObject))
                        return totalChildren[i] as T;
                    if (typeof(T) == typeof(Transform))
                        return totalChildren[i].transform as T;
                    if (totalChildren[i].GetComponent<T>() != null)
                        return totalChildren[i].GetComponent<T>();
                }
            return null;
        }
        

        public static List<Transform> GetChildren(this Transform obj)
        {
            List<Transform> children = new List<Transform>();
            for (int i = 0; i < obj.childCount; i++)
                children.Add(obj.GetChild(i)); 
            return children;
        }
        public static void DestroyAllChildren(this Transform obj)
        {
            for (int i = 0; i < obj.childCount; i++) GameObject.Destroy(obj.GetChild(i).gameObject); 
        }
        public static List<GameObject> getAllChildren(this GameObject obj)
        {
            var container = obj.transform;
            List<GameObject> allChildren = new List<GameObject>();
            for (int i = 0; i < container.childCount; i++)
            {
                var child = container.GetChild(i);
                allChildren.Add(child.gameObject);
                allChildren.AddRange(getAllChildren(child));
            }
            return allChildren;
        }

        public static T[] getAllChildrenOfType<T>(this GameObject obj) where T : Component
        {
            List<T> children = new List<T>();
            foreach (var child in obj.getAllChildren())
            {
                if (child.GetComponent<T>() != null)
                {
                    children.Add(child.GetComponent<T>());
                }
            }
            return children.ToArray();
        }

        public static T[] getAllChildrenOfType<T>(this Transform obj) where T : Component
        {
            List<T> children = new List<T>();
            foreach (var child in obj.getAllChildren())
            {
                if (child.GetComponent<T>() != null)
                {
                    children.Add(child.GetComponent<T>());
                }
            }
            return children.ToArray();
        }


        private static Sprite _whitePillBg;
        private static Texture2D _whitePillBgTex;

        internal static Sprite whitePillBg
        {
            get
            {
                if(_whitePillBg==null)
                {
                    _whitePillBgTex = Get<AssetBundle>("cc50fee78e6b7bdd6142627acdaf89fa.bundle")
                        .LoadAsset("Assets/UI/Textures/MenuDemo/whitePillBg.png").Cast<Texture2D>();
                    _whitePillBg = Sprite.Create(_whitePillBgTex,
                        new Rect(0f, 0f, _whitePillBgTex.width, _whitePillBgTex.height),
                        new Vector2(0.5f, 0.5f), 1f);
                }

                return _whitePillBg;
            }
        }
        internal static Texture2D whitePillBgTex
        {
            get
            {
                if(_whitePillBgTex==null)
                {
                    _whitePillBgTex = Get<AssetBundle>("cc50fee78e6b7bdd6142627acdaf89fa.bundle")
                        .LoadAsset("Assets/UI/Textures/MenuDemo/whitePillBg.png").Cast<Texture2D>();
                }

                return _whitePillBgTex;
            }
        }
        public static List<GameObject> getAllChildren(this Transform container)
        {
            List<GameObject> allChildren = new List<GameObject>();
            for (int i = 0; i < container.childCount; i++)
            {
                var child = container.GetChild(i);
                allChildren.Add(child.gameObject);
                allChildren.AddRange(getAllChildren(child));
            }
            return allChildren;
        }
        public static bool inGame
        {
            get
            {
                try
                {
                    if (SceneContext.Instance == null) return false;
                    if (SceneContext.Instance.PlayerState == null) return false;
                }
                catch
                { return false; }
                return true;
            }
        }

        internal static IdentifiableType[] identifiableTypes { get { return GameContext.Instance.AutoSaveDirector.identifiableTypes.GetAllMembers().ToArray().Where(identifiableType => !string.IsNullOrEmpty(identifiableType.ReferenceId)).ToArray(); } }
        internal static IdentifiableType[] vaccableTypes { get { return vaccableGroup.GetAllMembers().ToArray(); } }
        internal static IdentifiableType getIdentByName(string name)
        {
            if (String.IsNullOrWhiteSpace(name)) return null;
            if (name.ToLower() == "none" || name.ToLower() == "player") return null;
            foreach (IdentifiableType type in identifiableTypes)
                if (type.name.ToUpper() == name.ToUpper()) return type;
            foreach (IdentifiableType type in identifiableTypes) 
                try { if (type.LocalizedName.GetLocalizedString().ToUpper().Replace(" ","").Replace("_","") == name.Replace("_","").ToUpper()) return type; }catch {}
            return null;
        }
        internal static GadgetDefinition getGadgetDefByName(string name)
        {
            if (String.IsNullOrWhiteSpace(name)) return null;
            if (name.ToLower() == "none" || name.ToLower() == "player") return null;
            GadgetDefinition[] ids = Resources.FindObjectsOfTypeAll<GadgetDefinition>();
            foreach (GadgetDefinition type in ids)
                if (type.name.ToUpper() == name.ToUpper()) return type;
            foreach (GadgetDefinition type in ids) 
                try { if (type.LocalizedName.GetLocalizedString().ToUpper().Replace(" ","").Replace("_","") == name.Replace("_","").ToUpper()) return type; }catch {}
            return null;
        }

        static bool StartsWithContain(this string input, string value, bool useContain)
        {
            if (useContain) return input.Contains(value);
            return input.StartsWith(value);
        }

        public static IdentifiableTypeGroup vaccableGroup;
        public static List<string> getVaccableListByPartialName(string input, bool useContain)
        {
            IdentifiableType[] types = vaccableTypes;
            if (String.IsNullOrWhiteSpace(input))
            {
                List<string> cleanList = new List<string>();
                int j = 0;
                foreach (IdentifiableType type in types)
                {
                    bool isGadget = type.isGadget();
                    if (type.ReferenceId.ToLower() == "none" || type.ReferenceId.ToLower() == "player") continue;
                    if (j > MAX_AUTOCOMPLETE.Get()) break;
                    try
                    {if (type.LocalizedName != null)
                        {
                            string localizedString = type.LocalizedName.GetLocalizedString();
                            if(localizedString.StartsWith("!")) continue;
                            j++;
                            cleanList.Add(localizedString.Replace(" ", ""));
                        }
                    }catch { }
                }
                cleanList.Sort();
                return cleanList;
            }
            
            List<string> list = new List<string>();
            List<string> listTwo = new List<string>();
            int i = 0;
            foreach (IdentifiableType type in types)
            {
                if (type.ReferenceId.ToLower() == "none" || type.ReferenceId.ToLower() == "player") continue;
                
                if (i > MAX_AUTOCOMPLETE.Get()) break;
                try
                {
                    if (type.LocalizedName != null)
                    {
                        string localizedString = type.LocalizedName.GetLocalizedString();
                        if (localizedString.ToLower().Replace(" ", "").StartsWith(input.ToLower()))
                        {
                            if(localizedString.StartsWith("!")) continue;
                            i++;
                            list.Add(localizedString.Replace(" ", ""));
                        }
                    }
                }catch { }
            }
            if(useContain)
                foreach (IdentifiableType type in types)
                {
                    if (type.ReferenceId.ToLower() == "none" || type.ReferenceId.ToLower() == "player") continue;
                
                    if (i > MAX_AUTOCOMPLETE.Get()) break;
                    try
                    {
                        if (type.LocalizedName != null)
                        {
                            string localizedString = type.LocalizedName.GetLocalizedString();
                            if (localizedString.ToLower().Replace(" ", "").Contains(input.ToLower()))
                                if(!list.Contains(localizedString.Replace(" ", "")))
                                {
                                    if(localizedString.StartsWith("!")) continue;
                                    i++;
                                    listTwo.Add(localizedString.Replace(" ", ""));
                                }
                        }
                    }catch { }
                }
            list.Sort();
            listTwo.Sort();
            list.AddRange(listTwo);
            return list;
        }
        public static List<string> getIdentListByPartialName(string input, bool includeNormal, bool includeGadget, bool useContain,bool includeStars = false)
        {
            if (!includeGadget && !includeNormal)
                if (includeStars) return new List<string>() { "*" };
                else return new List<string>();
            if (String.IsNullOrWhiteSpace(input))
            {
                List<string> cleanList = new List<string>();
                int j = 0;
                foreach (IdentifiableType type in identifiableTypes)
                {
                    bool isGadget = type.isGadget();
                    if(type.ReferenceId.ToLower().Contains("Gordo")) continue;
                    if (type.ReferenceId.ToLower() == "none" || type.ReferenceId.ToLower() == "player") continue;
                    if (!includeGadget && isGadget) continue;
                    if (!includeNormal && !isGadget) continue;
                    if (j > MAX_AUTOCOMPLETE.Get()) break;
                    try
                    {if (type.LocalizedName != null)
                        {
                            string localizedString = type.LocalizedName.GetLocalizedString();
                            if(localizedString.StartsWith("!")) continue;
                            j++;
                            cleanList.Add(localizedString.Replace(" ", ""));
                        }
                    }catch { }
                }
                cleanList.Add("*");
                cleanList.Sort();
                return cleanList;
            }
            
            List<string> list = new List<string>();
            List<string> listTwo = new List<string>();
            int i = 0;
            foreach (IdentifiableType type in identifiableTypes)
            {
                bool isGadget = type.isGadget();
                if(type.ReferenceId.ToLower().Contains("Gordo")) continue;
                if (type.ReferenceId.ToLower() == "none" || type.ReferenceId.ToLower() == "player") continue;
                if (!includeGadget && isGadget) continue;
                if (!includeNormal && !isGadget) continue;
                
                if (i > MAX_AUTOCOMPLETE.Get()) break;
                try
                {
                    if (type.LocalizedName != null)
                    {
                        string localizedString = type.LocalizedName.GetLocalizedString();
                        if (localizedString.ToLower().Replace(" ", "").StartsWith(input.ToLower()))
                        {
                            if(localizedString.StartsWith("!")) continue;
                            i++;
                            list.Add(localizedString.Replace(" ", ""));
                        }
                    }
                }catch { }
            }
            if(useContain)
                foreach (IdentifiableType type in identifiableTypes)
                {
                    bool isGadget = type.isGadget();
                    if(type.ReferenceId.ToLower().Contains("Gordo")) continue;
                    if (type.ReferenceId.ToLower() == "none" || type.ReferenceId.ToLower() == "player") continue;
                    if (!includeGadget && isGadget) continue;
                    if (!includeNormal && !isGadget) continue;
                
                    if (i > MAX_AUTOCOMPLETE.Get()) break;
                    try
                    {
                        if (type.LocalizedName != null)
                        {
                            string localizedString = type.LocalizedName.GetLocalizedString();
                            if (localizedString.ToLower().Replace(" ", "").Contains(input.ToLower()))
                                if(!list.Contains(localizedString.Replace(" ", "")))
                                {
                                    if(localizedString.StartsWith("!")) continue;
                                    i++;
                                    listTwo.Add(localizedString.Replace(" ", ""));
                                }
                        }
                    }catch { }
                }
            list.Sort();
            listTwo.Sort();
            list.AddRange(listTwo);
            return list;
        }
        public static List<string> getKeyListByPartialName(string input, bool useContain)
        {
            if (String.IsNullOrWhiteSpace(input))
            {
                List<string> nullList = new List<string>();
                foreach (Key key in System.Enum.GetValues<Key>())
                    if (key != Key.None)
                        if (key.ToString().ToLower().StartsWith(input.ToLower()))
                            nullList.Add(key.ToString());
                nullList.Sort();
                return nullList;
            }
            
            List<string> list = new List<string>();
            List<string> listTwo = new List<string>();
            foreach (Key key in System.Enum.GetValues<Key>())
                if (key != Key.None)
                    if (key.ToString().ToLower().StartsWith(input.ToLower()))
                        list.Add(key.ToString());
            if(useContain)
                foreach (Key key in System.Enum.GetValues<Key>())
                    if (key != Key.None)
                        if (key.ToString().ToLower().Contains(input.ToLower()))
                            if(!list.Contains(key.ToString()))
                                listTwo.Add(key.ToString());
            list.Sort();
            listTwo.Sort();
            list.AddRange(listTwo);
            return list;
        }
        public static bool TryParseVector3(this SR2ECommand cmd, string inputX, string inputY, string inputZ, out Vector3 value)
        {
            value = Vector3.zero;
            try { value = new Vector3(float.Parse(inputX),float.Parse(inputY),float.Parse(inputZ)); }
            catch { return cmd.SendNotValidVector3(inputX,inputY,inputZ); }
            return true;
        }
        public static bool TryParseFloat(this SR2ECommand cmd, string input, out float value, float min, bool inclusive, float max)
        {
            value = 0;
            try { value = float.Parse(input); }
            catch { return cmd.SendNotValidFloat(input); }
            if (inclusive)
            {
                if (value < min) return cmd.SendNotFloatAtLeast(input, min);
            }
            else if (value <= min) return cmd.SendNotFloatAbove(input,min);
            if (value >= max) return cmd.SendNotFloatUnder(input, max);
            return true;
        }
        public static bool TryParseFloat(this SR2ECommand cmd, string input, out float value, float min, bool inclusive)
        {
            value = 0;
            try { value = float.Parse(input); }
            catch { return cmd.SendNotValidFloat(input); }
            if (inclusive)
            {
                if (value < min) return cmd.SendNotFloatAtLeast(input, min);
            }
            else if (value <= min) return cmd.SendNotFloatAbove(input,min);
            return true;
        }
        public static bool TryParseFloat(this SR2ECommand cmd, string input, out float value, float max)
        {
            value = 0;
            try { value = float.Parse(input); }
            catch { return cmd.SendNotValidFloat(input); }
            if (value >= max) return cmd.SendNotFloatUnder(input, max);
            return true;
        }
        public static bool TryParseFloat(this SR2ECommand cmd, string input, out float value)
        {
            value = 0;
            try { value = float.Parse(input); }
            catch { return cmd.SendNotValidFloat(input); }
            return true;
        }
        public static bool TryParseInt(this SR2ECommand cmd, string input, out int value, int min, bool inclusive, int max)
        {
            value = 0;
            try { value = int.Parse(input); }
            catch { return cmd.SendNotValidInt(input); }
            if (inclusive)
            {
                if (value < min) return cmd.SendNotIntAtLeast(input, min);
            }
            else if (value <= min) return cmd.SendNotIntAbove(input,min);
            if (value >= max) return cmd.SendNotIntUnder(input, max);
            return true;
        }
        public static bool TryParseInt(this SR2ECommand cmd, string input, out int value, int min, bool inclusive)
        {
            value = 0;
            try { value = int.Parse(input); }
            catch { return cmd.SendNotValidInt(input); }
            if (inclusive)
            {
                if (value < min) return cmd.SendNotIntAtLeast(input, min);
            }
            else if (value <= min) return cmd.SendNotIntAbove(input,min);
            return true;
        }
        public static bool TryParseInt(this SR2ECommand cmd, string input, out int value, int max)
        {
            value = 0;
            try { value = int.Parse(input); }
            catch { return cmd.SendNotValidInt(input); }
            if (value >= max) return cmd.SendNotIntUnder(input, max);
            return true;
        }
        public static bool TryParseInt(this SR2ECommand cmd, string input, out int value)
        {
            value = 0;
            try { value = int.Parse(input); }
            catch { return cmd.SendNotValidInt(input); }
            return true;
        }
        public static bool TryParseBool(this SR2ECommand cmd, string input, out bool value)
        {
            value = false;
            if (input.ToLower() != "true" && input.ToLower() != "false") cmd.SendNotValidBool(input);
            if (input.ToLower() == "true") value = true;
            return true;
        }
        public static bool TryParseTrool(this SR2ECommand cmd, string input, out Trool value)
        {
            value = Trool.False;
            if (input.ToLower() != "true" && input.ToLower() != "false" && input.ToLower() != "toggle") cmd.SendNotValidTrool(input);
            if (input.ToLower() == "true") value = Trool.True;
            if (input.ToLower() == "toggle") value = Trool.Toggle;
            return true;
        }
        public static bool TryParseKeyCode(this SR2ECommand cmd, string input, out Key value)
        {
            string keyToParse = input;
            if (input.ToCharArray().Length == 1)
                if (int.TryParse(input, out int ignored))
                    keyToParse = "Digit" + input;
            Key key;
            if (Key.TryParse(keyToParse, true, out key)) { value = key; return true; }
            value = Key.None;
            cmd.SendNotValidKeyCode(input);
            return false;
        }
        public static bool IsBetween(this string[] list, uint min, int max)
        {
            if (list == null)
            {
                if (min > 0) return false;
            }
            else 
            {
                if (list.Length < min) return false;
                if(max!=-1) if (list.Length > max) return false;
            }

            return true;
        }
        public static bool isGadget(this IdentifiableType type) => type.ReferenceId.StartsWith("GadgetDefinition");

        public static string getName(this IdentifiableType type)
        {
            try
            {
                string itemName = "";
                string name = type.LocalizedName.GetLocalizedString();
                if (name.Contains(" ")) itemName = "'" + name + "'";
                else itemName = name;
                return itemName;
            }
            catch
            { return type.name; }
        }
        public static TripleDictionary<GameObject, ParticleSystemRenderer, string> FXLibrary = new TripleDictionary<GameObject, ParticleSystemRenderer, string>();
        public static TripleDictionary<string, ParticleSystemRenderer, GameObject> FXLibraryReversable = new TripleDictionary<string, ParticleSystemRenderer, GameObject>();

        
        public static float4 changeValue(this float4 float4, int index, float value)
        {
            return new float4(index == 0 ? value : float4[0],
                index == 1 ? value : float4[1],
                index == 2 ? value : float4[2],
                index == 3 ? value : float4[3]
            );
        }

        public static bool isAnyMenuOpen
        {
            get
            {
                for (int i = 0; i < SR2EEntryPoint.SR2EStuff.transform.childCount; i++)
                    if (SR2EEntryPoint.SR2EStuff.transform.GetChild(i).name.Contains("(Clone)"))
                    {
                        if(SR2EEntryPoint.SR2EStuff.transform.GetChild(i).gameObject.activeSelf)
                            return true;
                    }
                return false;
            }
        }

        public static void CloseOpenMenu()
        {
            SR2EMenu menu = getOpenMenu;
            if(menu!=null)
                menu.Close();
        }
        public static SR2EMenu getOpenMenu
        {
            get
            {
                for (int i = 0; i < SR2EEntryPoint.SR2EStuff.transform.childCount; i++)
                    if (SR2EEntryPoint.SR2EStuff.transform.GetChild(i).name.Contains("(Clone)"))
                    {
                        if (SR2EEntryPoint.SR2EStuff.transform.GetChild(i).gameObject.activeSelf)
                            return SR2EEntryPoint.SR2EStuff.transform.GetChild(i).gameObject.GetComponent<SR2EMenu>();
                    }
                return null;
            }
        }

        public static void TryHideMenus()
        {
            if (SR2EEntryPoint.mainMenuLoaded)
            {
                try
                {
                    var ui = Object.FindObjectOfType<MainMenuLandingRootUI>();
                    ui.gameObject.SetActive(false);
                    ui.enabled = false;
                    ui.Close(true, null);
                }
                catch (Exception e)
                {
                    SR2ELogManager.SendError(e.ToString());
                }
            }

            if (inGame)
            {
                try
                {
                    Object.FindObjectOfType<PauseMenuRoot>().Close();
                }
                catch (Exception e)
                {
                    SR2ELogManager.SendError(e.ToString());
                }
            }
        }

        public static void TryPauseAndHide()
        {
            if (Object.FindObjectOfType<PauseMenuRoot>())
            {
                TryHideMenus();
                TryPauseGame(false);
                if(inGame) HudUI.Instance.transform.GetChild(0).gameObject.SetActive(false);
            }
            else
            {
                TryPauseGame();
                TryHideMenus();
            }
        }

        public static void TryPauseGame(bool usePauseMenu = true)
        {
            try { SystemContext.Instance.SceneLoader.TryPauseGame(); } catch { }
            if (usePauseMenu) try { Object.FindObjectOfType<PauseMenuDirector>().PauseGame(); } catch { }
        }

        public static void TryUnPauseGame(bool usePauseMenu = true, bool usePauseMenuElse = true)
        {
            try { SystemContext.Instance.SceneLoader.UnpauseGame(); } catch { }
            if(usePauseMenu) try { Object.FindObjectOfType<PauseMenuDirector>().UnPauseGame(); } catch { }
            else if(usePauseMenuElse) try { if (Object.FindObjectOfType<PauseMenuRoot>() != null) Object.FindObjectOfType<PauseMenuDirector>().PauseGame(); } catch { }
        }
        public static void TryUnHideMenus()
        {
            if (SR2EEntryPoint.mainMenuLoaded)
            {
                foreach (UIPrefabLoader loader in Object.FindObjectsOfType<UIPrefabLoader>())
                    if (loader.gameObject.name == "UIActivator" && loader.uiPrefab.name == "MainMenu" &&
                        loader.parentTransform.name == "MainMenuRoot")
                    {
                        loader.Start();
                        break;
                    }
            }

            if (inGame) HudUI.Instance.transform.GetChild(0).gameObject.SetActive(true);
        }

        public static float CustomTimeScale = 1f;
        public static void TryDisableSR2Input()
        {
            Object.FindObjectOfType<InputSystemUIInputModule>().actionsAsset.Disable();
        }
        public static void TryEnableSR2Input()
        {
            Object.FindObjectOfType<InputSystemUIInputModule>().actionsAsset.Enable();
        }

        internal static GameObject menuBlock;
        internal static Transform popUpBlock;
        internal static List<SR2EPopUp> openPopUps = new List<SR2EPopUp>();
        internal static SR2EMenuTheme GetTheme(this SR2EMenu menu)
        {
            try
            {
                var methodInfo = menu.GetType().GetMethod(nameof(SR2EMenu.GetMenuIdentifier), BindingFlags.Static | BindingFlags.Public);
                var result = methodInfo.Invoke(null, null);
                if (result is MenuIdentifier identifier)
                {
                    SR2ESaveManager.data.themes.TryAdd(identifier.saveKey, identifier.defaultTheme);
                    SR2EMenuTheme currentTheme = SR2ESaveManager.data.themes[identifier.saveKey];
                    List<SR2EMenuTheme> validThemes = getValidThemes(identifier.saveKey);
                    if (validThemes.Count == 0) return SR2EMenuTheme.Default;
                    if(!validThemes.Contains(currentTheme)) currentTheme = validThemes.First();
                    return currentTheme;
                }

            }catch (Exception e) {}

            return SR2EMenuTheme.Default;
        }
        internal static void OpenPopUpBlock(SR2EPopUp popUp)
        {
            if (popUpBlock.transform.GetParent() != popUp.transform.GetParent()) return;
            var instance = GameObject.Instantiate(popUpBlock, popUpBlock);
            instance.gameObject.SetActive(true);
            instance.SetSiblingIndex(popUp.transform.GetSiblingIndex()-1);
            popUp.block = instance;
        }
        internal static Dictionary<Action, int> actionCounter = new Dictionary<Action, int>();
        public static void ExecuteInTicks(Action action, int ticks)
        {
            if (action == null) return;
            actionCounter.Add(new Action(action),ticks);
        }
        public static void ExecuteInSeconds(Action action, float seconds)
        {
            MelonCoroutines.Start(waitForSeconds(seconds, action));
        }
        static System.Collections.IEnumerator waitForSeconds(float seconds, Action action)
        {
            yield return new WaitForSeconds(seconds);
            try { action.Invoke(); }catch (Exception e) { MelonLogger.Error(e); }
        }
        public static LayerMask defaultMask
        {
            get
            {
                LayerMask mask = ~0;
                mask &= ~(1 << Layers.GadgetPlacement);
                return mask;
            }
        }
        public static readonly Dictionary<Branch, string> BRANCHES = new()
        {
            { Branch.Release, "release" },
            { Branch.Beta, "beta" },
            { Branch.Alpha, "alpha" },
            { Branch.Developer, "dev" },
        };
    }
}
