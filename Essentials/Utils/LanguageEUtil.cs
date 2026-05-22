using Starlight.Managers;
using UnityEngine.Localization;

namespace Starlight.Utils;

public static class LanguageEUtil
{
    public static string Translation(string key) => StarlightLanguageManager.Translation(key);
    public static string Translation(string key, params object[] args) => StarlightLanguageManager.Translation(key, args);
    public static string Tr(string key) => StarlightLanguageManager.Translation(key);
    public static string Tr(string key, params object[] args) => StarlightLanguageManager.Translation(key, args);
    
    public static void LoadLanguage(string code) => StarlightLanguageManager.LoadLanguage(code);
    
    public static void AddLanguages(string csvText) => StarlightLanguageManager.AddLanguages(csvText);
    public static LocalizedString AddTranslation(string localized, string key = null, string table = "Actor") => StarlightLanguageManager.AddTranslation(localized, key, table);
    public static LocalizedString AddTranslationFromStarlight(string starlightTranslationID, string key = null, string table = "Actor") => StarlightLanguageManager.AddTranslationFromStarlight(starlightTranslationID, key, table);
    
    public static void SetTranslation(string localized, string key, string table) => StarlightLanguageManager.SetTranslation(localized, key, table);
    public static void SetTranslationFromStarlight(string starlightTranslationID, string key, string table) => StarlightLanguageManager.SetTranslationFromStarlight(starlightTranslationID, key, table);
}