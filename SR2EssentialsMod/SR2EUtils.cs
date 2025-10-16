global using static SR2E.Managers.SR2EInputManager;
using System;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using System.Linq;
using System.Reflection;
using Il2CppMonomiPark.SlimeRancher;
using Il2CppMonomiPark.SlimeRancher.Damage;
using Il2CppMonomiPark.SlimeRancher.Economy;
using Il2CppMonomiPark.SlimeRancher.Script.UI.Pause;
using Il2CppMonomiPark.SlimeRancher.Script.Util;
using Il2CppMonomiPark.SlimeRancher.UI;
using Il2CppMonomiPark.SlimeRancher.UI.MainMenu;
using UnityEngine.Localization;
using UnityEngine.Localization.Tables;
using Il2CppMonomiPark.SlimeRancher.Weather;
using Il2CppSystem.IO;
using SR2E.Enums;
using SR2E.Managers;
using UnityEngine.InputSystem;
using SR2E.Storage;
using Unity.Mathematics;
using UnityEngine.InputSystem.UI;

namespace SR2E
{
    public static class SR2EUtils
    {
        //Remove the comments after removing SR2EUtils from global using
        //
        //[Obsolete("Please use "+nameof(UnityEUtil)+"."+nameof(UnityEUtil.Get),true)] public static T? Get<T>(string name) where T : Object => UnityEUtil.Get<T>(name);
        //[Obsolete("Please use "+nameof(UnityEUtil)+"."+nameof(UnityEUtil.GetAll),true)] public static List<T> GetAll<T>() where T : Object => UnityEUtil.GetAll<T>();
        //[Obsolete("Please use "+nameof(UnityEUtil)+"."+nameof(UnityEUtil.Get),true)] public static GameObject? Get(string name) => UnityEUtil.Get<GameObject>(name);

        //[Obsolete("Please use "+nameof(SR2ELanguageManger)+"."+nameof(SR2ELanguageManger.AddTranslation),true)] public static LocalizedString AddTranslation(string localized, string key = "l.SR2ETest", string table = "Actor") => SR2ELanguageManger.AddTranslation(key, localized, table);
        //[Obsolete("Please use "+nameof(SR2ELanguageManger)+"."+nameof(SR2ELanguageManger.AddTranslationFromSR2E),true)]public static LocalizedString AddTranslationFromSR2E(string sr2eTranslationID, string key = "l.SR2ETest", string table = "Actor") => SR2ELanguageManger.AddTranslationFromSR2E(sr2eTranslationID, key, table);
        //[Obsolete("Please use "+nameof(SR2ELanguageManger)+"."+nameof(SR2ELanguageManger.SetTranslation),true)] public static void SetTranslation(string localized, string key = "l.SR2ETest", string table = "Actor") => SR2ELanguageManger.SetTranslation(localized,key, table);
        //[Obsolete("Please use "+nameof(SR2ELanguageManger)+"."+nameof(SR2ELanguageManger.SetTranslationFromSR2E),true)] public static void SetTranslationFromSR2E(string sr2eTranslationID, string key = "l.SR2ETest", string table = "Actor") => SR2ELanguageManger.SetTranslationFromSR2E(sr2eTranslationID, key, table);


        
        
        
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




        public static Il2CppArrayBase<WeatherStateDefinition> weatherStates => autoSaveDirector._configuration.WeatherStates.items.ToArray();
        public static WeatherStateDefinition WeatherState(string name) => weatherStates.FirstOrDefault((WeatherStateDefinition x) => x.name == name);


        public static Dictionary<string, Dictionary<string, string>> addedTranslations = new Dictionary<string, System.Collections.Generic.Dictionary<string, string>>();



        public static GameObject CopyObject(this GameObject obj) => Object.Instantiate(obj, rootOBJ.transform);

        public static void MakePrefab(this GameObject obj)
        {
            UnityEngine.Object.DontDestroyOnLoad(obj);
            obj.transform.parent = rootOBJ.transform;
        }



        internal static Dictionary<MelonPreferences_Entry, System.Action> entriesWithActions = new Dictionary<MelonPreferences_Entry, Action>();
        public static void AddNullAction(this MelonPreferences_Entry entry) => entriesWithActions.Add(entry, null);
        public static void AddAction(this MelonPreferences_Entry entry, System.Action action) => entriesWithActions.Add(entry, action);
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

        internal static void OpenPopUpBlock(SR2EPopUp popUp)
        {
            if (popUpBlock.transform.GetParent() != popUp.transform.GetParent()) return;
            var instance = GameObject.Instantiate(popUpBlock, popUpBlock);
            instance.gameObject.SetActive(true);
            instance.SetSiblingIndex(popUp.transform.GetSiblingIndex()-1);
            popUp.block = instance;
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

        [Obsolete("Please use "+nameof(EmbeddedResourceEUtil)+"."+nameof(EmbeddedResourceEUtil.LoadString),true)] public static string LoadTextFile(string name) => EmbeddedResourceEUtil.LoadString(name);
        
        
        [Obsolete("Please use "+nameof(SpawnEUtil)+"."+nameof(SpawnEUtil.SpawnGadget),true)] public static GameObject SpawnGadget(GadgetDefinition def, Vector3 pos) => SpawnEUtil.SpawnGadget(def, pos).GetGameObject();
        [Obsolete("Please use "+nameof(SpawnEUtil)+"."+nameof(SpawnEUtil.SpawnGadget),true)] public static GameObject SpawnGadget(GadgetDefinition def, Vector3 pos, Vector3 rot) => SpawnEUtil.SpawnGadget(def, pos,rot).GetGameObject();
        [Obsolete("Please use "+nameof(SpawnEUtil)+"."+nameof(SpawnEUtil.SpawnGadget),true)] public static GameObject SpawnGadget(GadgetDefinition def, Vector3 pos, Quaternion rot) => SpawnEUtil.SpawnGadget(def, pos,rot).GetGameObject();
        [Obsolete("Please use "+nameof(SpawnEUtil)+"."+nameof(SpawnEUtil.SpawnActor),true)] public static GameObject SpawnActor(IdentifiableType ident, Vector3 pos) => SpawnEUtil.SpawnActor(ident, pos);
        [Obsolete("Please use "+nameof(SpawnEUtil)+"."+nameof(SpawnEUtil.SpawnActor),true)] public static GameObject SpawnActor(IdentifiableType ident, Vector3 pos, Vector3 rot) => SpawnEUtil.SpawnActor(ident, pos,rot);
        [Obsolete("Please use "+nameof(SpawnEUtil)+"."+nameof(SpawnEUtil.SpawnActor),true)]  public static GameObject SpawnActor(IdentifiableType ident, Vector3 pos, Quaternion rot) => SpawnEUtil.SpawnActor(ident, pos,rot);
        [Obsolete("Please use "+nameof(SpawnEUtil)+"."+nameof(SpawnEUtil.SpawnDynamic),true)] public static GameObject SpawnDynamic(GameObject obj, Vector3 pos, Quaternion rot)=>SpawnEUtil.SpawnDynamic(obj, pos, rot);
        [Obsolete("Please use "+nameof(SpawnEUtil)+"."+nameof(SpawnEUtil.SpawnFX),true)] public static GameObject SpawnFX(GameObject fx, Vector3 pos) => SpawnEUtil.SpawnFX(fx, pos);
        [Obsolete("Please use "+nameof(SpawnEUtil)+"."+nameof(SpawnEUtil.SpawnFX),true)] public static GameObject SpawnFX(GameObject fx, Vector3 pos, Quaternion rot) => SpawnEUtil.SpawnFX(fx, pos,rot);
        
        [Obsolete("Please use "+nameof(MenuEUtil)+"."+nameof(MenuEUtil.CloseOpenMenu),true)] public static void CloseOpenMenu() => MenuEUtil.CloseOpenMenu();
        [Obsolete("Please use "+nameof(MenuEUtil)+"."+nameof(MenuEUtil.isAnyMenuOpen),true)]  public static bool isAnyMenuOpen => MenuEUtil.isAnyMenuOpen;
        [Obsolete("Please use "+nameof(MenuEUtil)+"."+nameof(MenuEUtil.GetOpenMenu),true)] public static SR2EMenu getOpenMenu => MenuEUtil.GetOpenMenu();
        [Obsolete("Please use "+nameof(MenuEUtil)+"."+nameof(MenuEUtil.GetValidThemes),true)] public static List<SR2EMenuTheme> getValidThemes(string saveKey) => MenuEUtil.GetValidThemes(saveKey);
        
        [Obsolete("Please use "+nameof(NamingEUtil)+"."+nameof(NamingEUtil.GetName),true)] public static string getName(IdentifiableType type) => NamingEUtil.GetName(type);
        
        
        [Obsolete("Please use "+nameof(ConvertEUtil)+"."+nameof(ConvertEUtil.Texture2DToSprite),true)] public static Sprite ConvertToSprite(Texture2D texture) => ConvertEUtil.Texture2DToSprite(texture);
        [Obsolete("Please use "+nameof(ConvertEUtil)+"."+nameof(ConvertEUtil.Base64ToTexture2D),true)] public static Texture2D Base64ToTexture2D(string base64) => ConvertEUtil.Base64ToTexture2D(base64);
        
        
        [Obsolete("Please use "+nameof(CurrencyEUtil)+"."+nameof(CurrencyEUtil.toICurrency),true)] public static ICurrency toICurrency(CurrencyDefinition currencyDefinition) => CurrencyEUtil.toICurrency(currencyDefinition);
        [Obsolete("Please use "+nameof(CurrencyEUtil)+"."+nameof(CurrencyEUtil.SetCurrency),true)] public static bool SetCurrency(string referenceID, int amount) => CurrencyEUtil.SetCurrency(referenceID, amount);
        [Obsolete("Please use "+nameof(CurrencyEUtil)+"."+nameof(CurrencyEUtil.SetCurrency),true)] public static bool SetCurrency(string referenceID,int amount, int amountEverCollected) => CurrencyEUtil.SetCurrency(referenceID, amount,amountEverCollected);
        [Obsolete("Please use "+nameof(CurrencyEUtil)+"."+nameof(CurrencyEUtil.SetCurrencyEverCollected),true)] public static bool SetCurrencyEverCollected(string referenceID,int amountEverCollected) => CurrencyEUtil.SetCurrencyEverCollected(referenceID, amountEverCollected);
        [Obsolete("Please use "+nameof(CurrencyEUtil)+"."+nameof(CurrencyEUtil.AddCurrency),true)] public static bool AddCurrency(string referenceID,int amount) => CurrencyEUtil.AddCurrency(referenceID, amount);
        [Obsolete("Please use "+nameof(CurrencyEUtil)+"."+nameof(CurrencyEUtil.GetCurrency),true)] public static int GetCurrency(string referenceID) => CurrencyEUtil.GetCurrency(referenceID);
        [Obsolete("Please use "+nameof(CurrencyEUtil)+"."+nameof(CurrencyEUtil.GetCurrencyEverCollected),true)] public static int GetCurrencyEverCollected(string referenceID) => CurrencyEUtil.GetCurrencyEverCollected(referenceID);


        [Obsolete("Please use " + nameof(LookupEUtil) + "." + nameof(LookupEUtil.isGadget), true)] public static bool isGadget(IdentifiableType type) => LookupEUtil.isGadget(type);
        [Obsolete("Please use "+nameof(LookupEUtil)+"."+nameof(LookupEUtil.GetWeatherStateDefinitionByName),true)] public static WeatherStateDefinition getWeatherStateByName(string name) => LookupEUtil.GetWeatherStateDefinitionByName(name);
        [Obsolete("Please use "+nameof(LookupEUtil)+"."+nameof(LookupEUtil.GetVaccableStringListByPartialName),true)] public static List<string> getVaccableListByPartialName(string input, bool useContain) => LookupEUtil.GetVaccableStringListByPartialName(input, useContain,MAX_AUTOCOMPLETE.Get());
        [Obsolete("Please use "+nameof(LookupEUtil)+"."+nameof(LookupEUtil.GetFilteredIdentifiableTypeStringListByPartialName),true)] public static List<string> getIdentListByPartialName(string input, bool includeNormal, bool includeGadget, bool useContain,bool includeStars = false) => LookupEUtil.GetFilteredIdentifiableTypeStringListByPartialName(input,useContain,MAX_AUTOCOMPLETE.Get(),includeStars);
        [Obsolete("Please use "+nameof(LookupEUtil)+"."+nameof(LookupEUtil.GetKeyStringListByPartialName),true)] public static List<string> getKeyListByPartialName(string input, bool useContain) => LookupEUtil.GetKeyStringListByPartialName(input,useContain,MAX_AUTOCOMPLETE.Get());
        
        
        
        [Obsolete("Please use " + nameof(SR2ECommand.TryParseVector3)+ "in this class", true)] public static bool TryParseVector3(SR2ECommand cmd, string inputX, string inputY, string inputZ, out Vector3 value) => cmd.TryParseVector3(inputX, inputY, inputZ, out value);
        [Obsolete("Please use " + nameof(SR2ECommand.TryParseFloat)+ "in this class", true)] public static bool TryParseFloat(SR2ECommand cmd, string input, out float value, float min, bool inclusive, float max) => cmd.TryParseFloat(input, out value, min, inclusive, max);
        [Obsolete("Please use " + nameof(SR2ECommand.TryParseFloat)+ "in this class", true)] public static bool TryParseFloat(SR2ECommand cmd, string input, out float value, float min, bool inclusive) => cmd.TryParseFloat(input, out value, min, inclusive);
        [Obsolete("Please use " + nameof(SR2ECommand.TryParseFloat)+ "in this class", true)] public static bool TryParseFloat(SR2ECommand cmd, string input, out float value, float max) => cmd.TryParseFloat(input, out value, max);
        [Obsolete("Please use " + nameof(SR2ECommand.TryParseFloat)+ "in this class", true)] public static bool TryParseFloat(SR2ECommand cmd, string input, out float value) => cmd.TryParseFloat(input, out value);
        [Obsolete("Please use " + nameof(SR2ECommand.TryParseInt)+ "in this class", true)] public static bool TryParseInt(SR2ECommand cmd, string input, out int value, int min, bool inclusive, int max) => cmd.TryParseInt(input, out value, min, inclusive, max);
        [Obsolete("Please use " + nameof(SR2ECommand.TryParseInt)+ "in this class", true)] public static bool TryParseInt(SR2ECommand cmd, string input, out int value, int min, bool inclusive) => cmd.TryParseInt(input, out value, min, inclusive);
        [Obsolete("Please use " + nameof(SR2ECommand.TryParseInt)+ "in this class", true)] public static bool TryParseInt(SR2ECommand cmd, string input, out int value, int max) => cmd.TryParseInt(input, out value, max);
        [Obsolete("Please use " + nameof(SR2ECommand.TryParseInt)+ "in this class", true)] public static bool TryParseInt(SR2ECommand cmd, string input, out int value) => cmd.TryParseInt(input,out value);
        [Obsolete("Please use " + nameof(SR2ECommand.TryParseBool)+ "in this class", true)] public static bool TryParseBool(SR2ECommand cmd, string input, out bool value) => cmd.TryParseBool(input, out value);
        [Obsolete("Please use " + nameof(SR2ECommand.TryParseTrool)+ "in this class", true)] public static bool TryParseTrool(SR2ECommand cmd, string input, out Trool value) => cmd.TryParseTrool(input, out value);
        [Obsolete("Please use " + nameof(SR2ECommand.TryParseKeyCode)+ "in this class", true)] public static bool TryParseKeyCode(SR2ECommand cmd, string input, out Key value) => cmd.TryParseKeyCode(input, out value);
        
        

        [Obsolete("Please use "+nameof(UnityEUtil)+"."+nameof(UnityEUtil.AddComponent),true)]  public static bool AddComponent<T>(Transform obj) where T : Component => UnityEUtil.AddComponent<T>(obj);
        [Obsolete("Please use "+nameof(UnityEUtil)+"."+nameof(UnityEUtil.HasComponent),true)] public static bool HasComponent<T>(Transform obj) where T : Component => UnityEUtil.HasComponent<T>(obj);
        [Obsolete("Please use "+nameof(UnityEUtil)+"."+nameof(UnityEUtil.HasComponent),true)] public static bool HasComponent<T>(GameObject obj) where T : Component => UnityEUtil.HasComponent<T>(obj);
        [Obsolete("Please use "+nameof(UnityEUtil)+"."+nameof(UnityEUtil.RemoveComponent),true)]  public static bool RemoveComponent<T>(Transform obj) where T : Component => UnityEUtil.RemoveComponent<T>(obj);
        [Obsolete("Please use "+nameof(UnityEUtil)+"."+nameof(UnityEUtil.RemoveComponent),true)] public static bool RemoveComponent<T>(GameObject obj) where T : Component => UnityEUtil.RemoveComponent<T>(obj);
        [Obsolete("Please use "+nameof(UnityEUtil)+"."+nameof(UnityEUtil.GetObjectRecursively),true)] public static T getObjRec<T>(GameObject obj, string name) where T : class => UnityEUtil.GetObjectRecursively<T>(obj,name);
        [Obsolete("Please use "+nameof(UnityEUtil)+"."+nameof(UnityEUtil.GetObjectRecursively),true)] public static T getObjRec<T>(Transform transform, string name) where T : class => UnityEUtil.GetObjectRecursively<T>(transform,name);
        [Obsolete("Please use "+nameof(UnityEUtil)+"."+nameof(UnityEUtil.GetChildren),true)] public static List<Transform> GetChildren(Transform obj) => UnityEUtil.GetChildren(obj);
        [Obsolete("Please use "+nameof(UnityEUtil)+"."+nameof(UnityEUtil.DestroyAllChildren),true)] public static void DestroyAllChildren(Transform obj) => UnityEUtil.DestroyAllChildren(obj);
        [Obsolete("Please use "+nameof(UnityEUtil)+"."+nameof(UnityEUtil.GetAllChildren),true)] public static List<GameObject> getAllChildren(GameObject obj) => UnityEUtil.GetAllChildren(obj);

        [Obsolete("Please use "+nameof(UnityEUtil)+"."+nameof(UnityEUtil.GetAllChildren),true)] public static List<GameObject> getAllChildren(Transform container) => UnityEUtil.GetAllChildren(container);
        [Obsolete("Please use "+nameof(UnityEUtil)+"."+nameof(UnityEUtil.GetAllChildrenOfType),true)] public static T[] getAllChildrenOfType<T>(GameObject obj) where T : Component => UnityEUtil.GetAllChildrenOfType<T>(obj);
        [Obsolete("Please use "+nameof(UnityEUtil)+"."+nameof(UnityEUtil.GetAllChildrenOfType),true)] public static T[] getAllChildrenOfType<T>(Transform obj) where T : Component => UnityEUtil.GetAllChildrenOfType<T>(obj);
        
        [Obsolete("Was never used/never worked",true)] public static List<LocalizedString> createdTranslations = new List<LocalizedString>();


    }
}
