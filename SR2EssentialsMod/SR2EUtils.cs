global using static SR2E.Managers.SR2EInputManager;
using System;
using System.Diagnostics;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using System.Linq;
using System.Reflection;
using Il2CppMonomiPark.SlimeRancher;
using Il2CppMonomiPark.SlimeRancher.Damage;
using Il2CppMonomiPark.SlimeRancher.DataModel;
using Il2CppMonomiPark.SlimeRancher.Economy;
using Il2CppMonomiPark.SlimeRancher.Persist;
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
using UnityEngine.InputSystem;
using SR2E.Storage;
using Unity.Mathematics;
using UnityEngine.InputSystem.UI;
using UnityEngine.TextCore;
using UnityEngine.TextCore.LowLevel;

namespace SR2E
{
    public static partial class SR2EUtils
    {
        
        //
        // GET NAMES FOR DIFFERENT SR2 TYPES
        //

        
        
        
        
        
        
        
        internal static Dictionary<string, InputActionMap> actionMaps = new Dictionary<string, InputActionMap>();
        internal static Dictionary<string, InputAction> MainGameActions = new Dictionary<string, InputAction>();
        internal static Dictionary<string, InputAction> PausedActions = new Dictionary<string, InputAction>();
        internal static Dictionary<string, InputAction> DebugActions = new Dictionary<string, InputAction>();
        public static WeatherStateDefinition[] weatherStateDefinitions => Resources.FindObjectsOfTypeAll<WeatherStateDefinition>();

        internal static GameObject rootOBJ;

        public static GameObject? player;

        public static SystemContext systemContext => SystemContext.Instance;
        public static GameContext gameContext => GameContext.Instance;
        public static AutoSaveDirector autoSaveDirector => GameContext.Instance.AutoSaveDirector;


        public static SceneContext sceneContext => SceneContext.Instance;
        internal static Damage _killDamage;
        public static Damage killDamage => _killDamage;

        public static ICurrency toICurrency(this CurrencyDefinition currencyDefinition) => currencyDefinition.TryCast<ICurrency>();
        

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

        public static Il2CppArrayBase<WeatherStateDefinition> weatherStates => autoSaveDirector._configuration.WeatherStates.items.ToArray();
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
        public static T? Get<T>(string name) where T : Object => Resources.FindObjectsOfTypeAll<T>().FirstOrDefault((T x) => x.name == name);
        public static List<T> GetAll<T>() where T : Object => Resources.FindObjectsOfTypeAll<T>().ToList();


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
        public static GameObject? Get(string name) => Get<GameObject>(name);

        public static void MakePrefab(this GameObject obj)
        {
            UnityEngine.Object.DontDestroyOnLoad(obj);
            obj.transform.parent = rootOBJ.transform;
        }



        internal static Dictionary<MelonPreferences_Entry, System.Action> entriesWithActions = new Dictionary<MelonPreferences_Entry, Action>();
        public static void AddNullAction(this MelonPreferences_Entry entry) => entriesWithActions.Add(entry, null);
        public static void AddAction(this MelonPreferences_Entry entry, System.Action action) => entriesWithActions.Add(entry, action);
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

        internal static IdentifiableType[] identifiableTypes { get { return autoSaveDirector._configuration.IdentifiableTypes.GetAllMembers().ToArray().Where(identifiableType => !string.IsNullOrEmpty(identifiableType.ReferenceId)).ToArray(); } }
        internal static IdentifiableType[] vaccableTypes { get { return vaccableGroup.GetAllMembers().ToArray(); } }
        public static IdentifiableTypeGroup vaccableGroup;
        
        
        
        //
        // PARSING STRINGS INTO VALUES
        // EXLUSIVELY FOR SR2ECOMMANDS
        //
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
                    //SR2ELogManager.SendError(e.ToString());
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
                    //SR2ELogManager.SendError(e.ToString());
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

        internal static float _CustomTimeScale = 1f;
        public static float CustomTimeScale {
            get {
                return _CustomTimeScale;
            }
            set
            {
                _CustomTimeScale = value;
                SR2EEntryPoint.CheckForTime();
            }
        }
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
                    List<SR2EMenuTheme> validThemes = MenuUtil.GetValidThemes(identifier.saveKey);
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
            actionCounter.Add((Action)(() => { action.Invoke(); }),ticks);
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
        
        
        
        
        ///
        ///
        ///    ///
        ///    ///
        ///    /// OBSOLETE THINGS
        ///    ///
        ///    ///
        ///
        ///

        
        [Obsolete("Please use "+nameof(SpawnUtil)+"."+nameof(SpawnUtil.SpawnGadget),true)] public static GameObject SpawnGadget(GadgetDefinition def, Vector3 pos) => SpawnUtil.SpawnGadget(def, pos).GetGameObject();
        [Obsolete("Please use "+nameof(SpawnUtil)+"."+nameof(SpawnUtil.SpawnGadget),true)] public static GameObject SpawnGadget(GadgetDefinition def, Vector3 pos, Vector3 rot) => SpawnUtil.SpawnGadget(def, pos,rot).GetGameObject();
        [Obsolete("Please use "+nameof(SpawnUtil)+"."+nameof(SpawnUtil.SpawnGadget),true)] public static GameObject SpawnGadget(GadgetDefinition def, Vector3 pos, Quaternion rot) => SpawnUtil.SpawnGadget(def, pos,rot).GetGameObject();
        [Obsolete("Please use "+nameof(SpawnUtil)+"."+nameof(SpawnUtil.SpawnActor),true)] public static GameObject SpawnActor(IdentifiableType ident, Vector3 pos) => SpawnUtil.SpawnActor(ident, pos);
        [Obsolete("Please use "+nameof(SpawnUtil)+"."+nameof(SpawnUtil.SpawnActor),true)] public static GameObject SpawnActor(IdentifiableType ident, Vector3 pos, Vector3 rot) => SpawnUtil.SpawnActor(ident, pos,rot);
        [Obsolete("Please use "+nameof(SpawnUtil)+"."+nameof(SpawnUtil.SpawnActor),true)]  public static GameObject SpawnActor(IdentifiableType ident, Vector3 pos, Quaternion rot) => SpawnUtil.SpawnActor(ident, pos,rot);
        [Obsolete("Please use "+nameof(SpawnUtil)+"."+nameof(SpawnUtil.SpawnDynamic),true)] public static GameObject SpawnDynamic(GameObject obj, Vector3 pos, Quaternion rot)=>SpawnUtil.SpawnDynamic(obj, pos, rot);
        [Obsolete("Please use "+nameof(SpawnUtil)+"."+nameof(SpawnUtil.SpawnFX),true)] public static GameObject SpawnFX(GameObject fx, Vector3 pos) => SpawnUtil.SpawnFX(fx, pos);
        [Obsolete("Please use "+nameof(SpawnUtil)+"."+nameof(SpawnUtil.SpawnFX),true)] public static GameObject SpawnFX(GameObject fx, Vector3 pos, Quaternion rot) => SpawnUtil.SpawnFX(fx, pos,rot);
        
        [Obsolete("Please use "+nameof(MenuUtil)+"."+nameof(MenuUtil.CloseOpenMenu),true)] public static void CloseOpenMenu() => MenuUtil.CloseOpenMenu();
        [Obsolete("Please use "+nameof(MenuUtil)+"."+nameof(MenuUtil.isAnyMenuOpen),true)]  public static bool isAnyMenuOpen => MenuUtil.isAnyMenuOpen;
        [Obsolete("Please use "+nameof(MenuUtil)+"."+nameof(MenuUtil.GetOpenMenu),true)] public static SR2EMenu getOpenMenu => MenuUtil.GetOpenMenu();
        [Obsolete("Please use "+nameof(MenuUtil)+"."+nameof(MenuUtil.GetValidThemes),true)] public static List<SR2EMenuTheme> getValidThemes(string saveKey) => MenuUtil.GetValidThemes(saveKey);
        
        [Obsolete("Please use "+nameof(NamingUtil)+"."+nameof(NamingUtil.GetName),true)] public static string getName(this IdentifiableType type) => NamingUtil.GetName(type);
        
        
        [Obsolete("Please use "+nameof(ConvertUtil)+"."+nameof(ConvertUtil.Texture2DToSprite),true)] public static Sprite ConvertToSprite(this Texture2D texture) => ConvertUtil.Texture2DToSprite(texture);
        [Obsolete("Please use "+nameof(ConvertUtil)+"."+nameof(ConvertUtil.Base64ToTexture2D),true)] public static Texture2D Base64ToTexture2D(string base64) => ConvertUtil.Base64ToTexture2D(base64);
        
        
        [Obsolete("Please use "+nameof(CurrencyUtil)+"."+nameof(CurrencyUtil.SetCurrency),true)] public static bool SetCurrency(string referenceID, int amount) => CurrencyUtil.SetCurrency(referenceID, amount);
        [Obsolete("Please use "+nameof(CurrencyUtil)+"."+nameof(CurrencyUtil.SetCurrency),true)] public static bool SetCurrency(string referenceID,int amount, int amountEverCollected) => CurrencyUtil.SetCurrency(referenceID, amount,amountEverCollected);
        [Obsolete("Please use "+nameof(CurrencyUtil)+"."+nameof(CurrencyUtil.SetCurrencyEverCollected),true)] public static bool SetCurrencyEverCollected(string referenceID,int amountEverCollected) => CurrencyUtil.SetCurrencyEverCollected(referenceID, amountEverCollected);
        [Obsolete("Please use "+nameof(CurrencyUtil)+"."+nameof(CurrencyUtil.AddCurrency),true)] public static bool AddCurrency(string referenceID,int amount) => CurrencyUtil.AddCurrency(referenceID, amount);
        [Obsolete("Please use "+nameof(CurrencyUtil)+"."+nameof(CurrencyUtil.GetCurrency),true)] public static int GetCurrency(string referenceID) => CurrencyUtil.GetCurrency(referenceID);
        [Obsolete("Please use "+nameof(CurrencyUtil)+"."+nameof(CurrencyUtil.GetCurrencyEverCollected),true)] public static int GetCurrencyEverCollected(string referenceID) => CurrencyUtil.GetCurrencyEverCollected(referenceID);

        
        [Obsolete("Please use "+nameof(LookupUtil)+"."+nameof(LookupUtil.GetWeatherStateByName),true)] public static WeatherStateDefinition getWeatherStateByName(string name) => LookupUtil.GetWeatherStateByName(name);
        [Obsolete("Please use "+nameof(LookupUtil)+"."+nameof(LookupUtil.GetVaccableListByPartialName),true)] public static List<string> getVaccableListByPartialName(string input, bool useContain) => LookupUtil.GetVaccableListByPartialName(input, useContain);
        [Obsolete("Please use "+nameof(LookupUtil)+"."+nameof(LookupUtil.GetIdentListByPartialName),true)] public static List<string> getIdentListByPartialName(string input, bool includeNormal, bool includeGadget, bool useContain,bool includeStars = false) => LookupUtil.GetIdentListByPartialName(input,includeNormal,includeGadget,useContain,includeStars);
        [Obsolete("Please use "+nameof(LookupUtil)+"."+nameof(LookupUtil.GetKeyListByPartialName),true)] public static List<string> getKeyListByPartialName(string input, bool useContain) => LookupUtil.GetKeyListByPartialName(input,useContain);
    }
}
