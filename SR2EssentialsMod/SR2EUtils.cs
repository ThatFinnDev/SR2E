﻿global using static SR2E.Managers.SR2EInputManager;
using System;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppMonomiPark.SlimeRancher;
using Il2CppMonomiPark.SlimeRancher.Damage;
using Il2CppMonomiPark.SlimeRancher.Economy;
using UnityEngine.Localization;
using Il2CppMonomiPark.SlimeRancher.Weather;
using SR2E.Enums;
using SR2E.Managers;
using SR2E.Storage;
using UnityEngine.InputSystem;
using Unity.Mathematics;

namespace SR2E
{
    [Obsolete("Please use the new EUtil classes!")] 
    public static class SR2EUtils
    {
        [Obsolete("Please use "+nameof(SR2ELanguageManger)+"."+nameof(SR2ELanguageManger.AddTranslation),true)] public static LocalizedString AddTranslation(string localized, string key = "l.SR2ETest", string table = "Actor") => SR2ELanguageManger.AddTranslation(key, localized, table);
        [Obsolete("Please use "+nameof(SR2ELanguageManger)+"."+nameof(SR2ELanguageManger.AddTranslationFromSR2E),true)]public static LocalizedString AddTranslationFromSR2E(string sr2eTranslationID, string key = "l.SR2ETest", string table = "Actor") => SR2ELanguageManger.AddTranslationFromSR2E(sr2eTranslationID, key, table);
        [Obsolete("Please use "+nameof(SR2ELanguageManger)+"."+nameof(SR2ELanguageManger.SetTranslation),true)] public static void SetTranslation(string localized, string key = "l.SR2ETest", string table = "Actor") => SR2ELanguageManger.SetTranslation(localized,key, table);
        [Obsolete("Please use "+nameof(SR2ELanguageManger)+"."+nameof(SR2ELanguageManger.SetTranslationFromSR2E),true)] public static void SetTranslationFromSR2E(string sr2eTranslationID, string key = "l.SR2ETest", string table = "Actor") => SR2ELanguageManger.SetTranslationFromSR2E(sr2eTranslationID, key, table);
        [Obsolete("Please use "+nameof(SR2ELanguageManger)+"."+nameof(SR2ELanguageManger.addedTranslations),true)] public static Dictionary<string, Dictionary<string, string>> addedTranslations = new Dictionary<string, System.Collections.Generic.Dictionary<string, string>>();

        [Obsolete("Please use "+nameof(ContextShortcuts)+"."+nameof(ContextShortcuts.inGame),true)] public static SystemContext systemContext => ContextShortcuts.systemContext;
        [Obsolete("Please use "+nameof(ContextShortcuts)+"."+nameof(ContextShortcuts.inGame),true)] public static GameContext gameContext => ContextShortcuts.gameContext;
        [Obsolete("Please use "+nameof(ContextShortcuts)+"."+nameof(ContextShortcuts.inGame),true)] public static SceneContext sceneContext => ContextShortcuts.sceneContext;
        [Obsolete("Please use "+nameof(ContextShortcuts)+"."+nameof(ContextShortcuts.inGame),true)] public static Damage killDamage => ContextShortcuts.killDamage;
        [Obsolete("Please use "+nameof(ContextShortcuts)+"."+nameof(ContextShortcuts.inGame),true)] public static AutoSaveDirector autoSaveDirector => ContextShortcuts.autoSaveDirector;
        [Obsolete("Please use "+nameof(ContextShortcuts)+"."+nameof(ContextShortcuts.inGame),true)] public static bool inGame => ContextShortcuts.inGame;

        [Obsolete("Please use "+nameof(NativeEUtil)+"."+nameof(NativeEUtil.TryHideMenus),true)] public static void TryHideMenus() => NativeEUtil.TryHideMenus();
        [Obsolete("Please use "+nameof(NativeEUtil)+"."+nameof(NativeEUtil.TryPauseAndHide),true)] public static void TryPauseAndHide() => NativeEUtil.TryPauseAndHide();
        [Obsolete("Please use "+nameof(NativeEUtil)+"."+nameof(NativeEUtil.TryPauseGame),true)] public static void TryPauseGame(bool usePauseMenu = true) => NativeEUtil.TryPauseGame(usePauseMenu);
        [Obsolete("Please use "+nameof(NativeEUtil)+"."+nameof(NativeEUtil.TryUnPauseGame),true)] public static void TryUnPauseGame(bool usePauseMenu = true, bool usePauseMenuElse = true) => NativeEUtil.TryUnPauseGame(usePauseMenu,usePauseMenuElse);
        [Obsolete("Please use "+nameof(NativeEUtil)+"."+nameof(NativeEUtil.TryUnHideMenus),true)] public static void TryUnHideMenus() => NativeEUtil.TryUnHideMenus();
        [Obsolete("Please use "+nameof(NativeEUtil)+"."+nameof(NativeEUtil.CustomTimeScale),true)] public static float CustomTimeScale { get { return NativeEUtil.CustomTimeScale; } set { NativeEUtil.CustomTimeScale = value; } }
        [Obsolete("Please use "+nameof(NativeEUtil)+"."+nameof(NativeEUtil.TryDisableSR2Input),true)] public static void TryDisableSR2Input() => NativeEUtil.TryDisableSR2Input();
        [Obsolete("Please use "+nameof(NativeEUtil)+"."+nameof(NativeEUtil.TryEnableSR2Input),true)] public static void TryEnableSR2Input() => NativeEUtil.TryEnableSR2Input();
        
        [Obsolete("Please use "+nameof(MiscEUtil)+"."+nameof(MiscEUtil.AddNullAction),true)]  public static void AddNullAction(this MelonPreferences_Entry entry) => MiscEUtil.AddNullAction(entry);
        [Obsolete("Please use "+nameof(MiscEUtil)+"."+nameof(MiscEUtil.AddAction),true)] public static void AddAction(this MelonPreferences_Entry entry, System.Action action) => MiscEUtil.AddAction(entry,action);
        [Obsolete("Please use "+nameof(MiscEUtil)+"."+nameof(MiscEUtil.il2cppTypeof),true)] public static Il2CppSystem.Type il2cppTypeof(this Type type) => MiscEUtil.il2cppTypeof(type);
        [Obsolete("Please use "+nameof(MiscEUtil)+"."+nameof(MiscEUtil.changeValue),true)] public static float4 changeValue(this float4 float4, int index, float value) => MiscEUtil.changeValue(float4, index, value);
        [Obsolete("Please use "+nameof(MiscEUtil)+"."+nameof(MiscEUtil.defaultMask), true)] public static LayerMask defaultMask => MiscEUtil.defaultMask;
        [Obsolete("Please use "+nameof(MiscEUtil)+"."+nameof(MiscEUtil.BRANCHES), true)] public static readonly Dictionary<Branch,string> BRANCHES = MiscEUtil.BRANCHES;
        [Obsolete("Please use "+nameof(MiscEUtil)+"."+nameof(MiscEUtil.IsBetween), true)] public static bool IsBetween(this string[] list, uint min, int max)=>ContextShortcuts.IsBetween(list, min, max);
        
        [Obsolete("Please use SceneContext.Instance.player")] public static GameObject? player;
        
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
        
        [Obsolete("Please use "+nameof(MenuEUtil)+"."+nameof(MenuEUtil.GetMenu),true)] public static T GM<T>() where T : SR2EMenu => MenuEUtil.GetMenu<T>();
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
        [Obsolete("Please use "+nameof(LookupEUtil)+"."+nameof(LookupEUtil.GetWeatherStateDefinitionByName),true)] public static WeatherStateDefinition WeatherState(string name) => LookupEUtil.GetWeatherStateDefinitionByName(name);
        [Obsolete("Please use "+nameof(LookupEUtil)+"."+nameof(LookupEUtil.weatherStateDefinitions),true)] public static WeatherStateDefinition[] weatherStateDefinitions => LookupEUtil.weatherStateDefinitions;
        [Obsolete("Please use "+nameof(LookupEUtil)+"."+nameof(LookupEUtil.weatherStateDefinitions),true)] public static Il2CppArrayBase<WeatherStateDefinition> weatherStates => autoSaveDirector._configuration.WeatherStates.items.ToArray();
        [Obsolete("Please use "+nameof(LookupEUtil)+"."+nameof(LookupEUtil.vaccableGroup),true)] public static IdentifiableTypeGroup vaccableGroup { get { return LookupEUtil.vaccableGroup; } set { LookupEUtil.vaccableGroup = value; } }
        [Obsolete("Please use "+nameof(LookupEUtil)+"."+nameof(LookupEUtil.FXLibrary),true)] public static TripleDictionary<GameObject, ParticleSystemRenderer, string> FXLibrary { get { return LookupEUtil.FXLibrary; } set { LookupEUtil.FXLibrary = value; } }
        [Obsolete("Please use "+nameof(LookupEUtil)+"."+nameof(LookupEUtil.FXLibraryReversable),true)] public static TripleDictionary<string, ParticleSystemRenderer, GameObject> FXLibraryReversable { get { return LookupEUtil.FXLibraryReversable; } set { LookupEUtil.FXLibraryReversable = value; } }
        
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
        
        [Obsolete("Please use "+nameof(UnityEUtil)+"."+nameof(UnityEUtil.Get),true)] public static T? Get<T>(string name) where T : Object => UnityEUtil.Get<T>(name);
        [Obsolete("Please use "+nameof(UnityEUtil)+"."+nameof(UnityEUtil.GetAll),true)] public static List<T> GetAll<T>() where T : Object => UnityEUtil.GetAll<T>();
        [Obsolete("Please use "+nameof(UnityEUtil)+"."+nameof(UnityEUtil.Get),true)] public static GameObject? Get(string name) => UnityEUtil.Get<GameObject>(name);
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
        [Obsolete("Please use "+nameof(UnityEUtil)+"."+nameof(UnityEUtil.CopyObject),true)] public static GameObject CopyObject(GameObject obj) => UnityEUtil.CopyObject(obj);
        [Obsolete("Please use "+nameof(UnityEUtil)+"."+nameof(UnityEUtil.MakePrefab),true)]  public static void MakePrefab(GameObject obj) => UnityEUtil.MakePrefab(obj);
        
        [Obsolete("Was never used/never worked",true)] public static List<LocalizedString> createdTranslations = new List<LocalizedString>();


    }
}
