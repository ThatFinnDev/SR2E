using System;
using System.IO;
using System.Reflection;
using Il2CppMonomiPark.SlimeRancher.Script.Util;
using Microsoft.VisualBasic.FileIO;
using UnityEngine.Localization;
using UnityEngine.Localization.Tables;

namespace CottonLibrary;
/*
public static partial class Library
{
    
    internal struct ModdedLocalizedText
    {
        public object[] parameters;
        public string table;
        public string srKey;
        public LocalizedString str;
    }

    internal static Dictionary<string, List<Dictionary<string, string>>> languages = new Dictionary<string, List<Dictionary<string, string>>>();
    static Dictionary<string, string> loadedLanguage = new Dictionary<string, string>();
    static Dictionary<string, string> defaultLang = null;
    internal static Dictionary<string, LocalizedString> loadedLocalizedStrings = new Dictionary<string, LocalizedString>();
    internal static Dictionary<string, ModdedLocalizedText> moddedLocalizedStrings = new Dictionary<string, ModdedLocalizedText>();
        
    /// <summary>
    /// Loads a string from the modded language csv.
    /// </summary>
    /// <param name="key">The key for the text</param>
    /// <returns>The text from the language selected in-game.</returns>
    public static string LoadLocalizedText(string key)
    {
        if (String.IsNullOrWhiteSpace(key) || !loadedLanguage.ContainsKey(key)) return key;
        return loadedLanguage[key];
    }
    
    /// <summary>
    /// Loads a string from the modded language csv.
    /// </summary>
    /// <param name="args">The arguments for the string.</param>
    /// <param name="key">The key for the text</param>
    /// <returns>The text from the language selected in-game.</returns>
    public static string LoadLocalizedText(string key, params object[] args)
    {
        if (String.IsNullOrWhiteSpace(key) || !loadedLanguage.ContainsKey(key)) return key;
        int i = 1;
        string translatedRaw = loadedLanguage[key];

        foreach (object obj in args)
        {
            translatedRaw = translatedRaw.Replace($"${i}", obj.ToString());
            i++;
        }
    
        return translatedRaw;
    }

    /// <summary>
    /// Creates a LocalizedString from the <c>LoadLocalizedText</c> function.
    /// </summary>
    /// <param name="key">The key for the text</param>
    /// <param name="table">The table to store the LocalizedString in</param>
    /// <returns>A new LocalizedString</returns>
    public static LocalizedString CreateLocalizedString(string key, string table = "Actor"){
        return CreateLocalizedString(key, table, new object[0]);
    }

    /// <summary>
    /// Creates a LocalizedString from the <c>LoadLocalizedText</c> function.
    /// </summary>
    /// <param name="key">The key for the text</param>
    /// <param name="table">The table to store the LocalizedString in</param>
    /// <param name="args">The arguments for the LocalizedString.</param>
    /// <returns>A new LocalizedString</returns>
    public static LocalizedString CreateLocalizedString(string key, string table = "Actor", params object[] args){

            LocalizedString localizedString = CreateStaticString(LoadLocalizedText(key, args), key, table);
            
            loadedLocalizedStrings.TryAdd(key,localizedString);
            moddedLocalizedStrings.TryAdd(key, new ModdedLocalizedText(){
                str = localizedString,
                table = table,
                srKey = key,
                parameters = args
            });
            
            return localizedString;
    }

    /// <summary>
    /// Add a localization table.
    /// </summary>
    /// <param name="csvFile">The <c>.CSV</c> file containing the translations</param>
    public static void AddLanguages(string csvFile)
    {
        var newLanguages = new Dictionary<string, Dictionary<string, string>>();
        var codeIndexes = new List<string>(){};
        Assembly executingAssembly = Assembly.GetCallingAssembly();
        Stream manifestResourceStream =
            executingAssembly.GetManifestResourceStream(executingAssembly.GetName().Name + "." + csvFile +
                                                        ".csv");
        using (TextFieldParser csvParser = new TextFieldParser(manifestResourceStream))
        {
            csvParser.CommentTokens = new string[] { "#" };
            csvParser.SetDelimiters(new string[] { "," });
            csvParser.HasFieldsEnclosedInQuotes = true;
            
            bool firstLine = true;
            while (!csvParser.EndOfData)
            {
                string[] parts = csvParser.ReadFields();
                if (firstLine)
                {
                    firstLine = false;
                    if (parts == null) return; if (parts.Length < 1) return;
                    bool isKeys = true;
                    foreach (string code in parts)
                        if (isKeys) isKeys = false;
                        else
                        {
                            if (!newLanguages.ContainsKey(code)) newLanguages[code] = new Dictionary<string, string>();
                            codeIndexes.Add(code);
                        }
                }
                else
                {
                    if (parts == null) continue; if (parts.Length < 1) continue;
                    bool isKey = true;
                    string key = parts[0];
                    int i = 0;
                    foreach (string translation in parts)
                        if (isKey) isKey = false;
                        else
                        {
                            if(codeIndexes.Count>i) newLanguages[codeIndexes[i]][key] = translation.Replace("\\n", "\n");
                            i++;
                        }
                }
            }
        }
        foreach (var newLanguage in newLanguages)
        {
            var langCode=newLanguage.Key;
            if (!languages.ContainsKey(langCode)) languages.Add(langCode, new List<Dictionary<string, string>>());
            languages[langCode].Add(newLanguage.Value);
        }
    }

    internal static void LoadLanguage(string code)
    {
        loadedLanguage = new Dictionary<string, string>();
        if (defaultLang == null)
        {
            defaultLang = new Dictionary<string, string>();

            if (languages.TryGetValue("en", out var language))
                foreach (var languageDicts in language)
                foreach (var translation in languageDicts)
                    defaultLang[translation.Key] = translation.Value;
        }

        loadedLanguage = new Dictionary<string, string>(defaultLang);
        
        if (code != "en")
            if (languages.ContainsKey(code))
                foreach (var languageDicts in languages[code])
                    foreach (var translation in languageDicts) 
                        loadedLanguage[translation.Key] = translation.Value;
    }

    /// <summary>
    /// Renamed from <c>AddTranslation</c> in 0.3.0
    /// </summary>
    /// <param name="localized">The text to put in the localized string</param>
    /// <param name="key">The localized string's key</param>
    /// <param name="table">The localization table</param>
    /// <returns>A <c>LocalizedString</c> with your text.   </returns>
    public static LocalizedString CreateStaticString(string localized, string key = "l.Empty",
        string table = "Actor")
    {
        StringTable table2 = LocalizationUtil.GetTable(table);

        StringTableEntry stringTableEntry = table2.AddEntry(key, localized);
        LocalizedString result =
            new LocalizedString(table2.SharedData.TableCollectionName, stringTableEntry.SharedEntry.Id);

        return result;
    }
}
*/