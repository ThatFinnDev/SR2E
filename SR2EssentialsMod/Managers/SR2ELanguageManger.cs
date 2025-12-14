using System;
using System.IO;
using Il2CppMonomiPark.SlimeRancher.Script.Util;
using Microsoft.VisualBasic.FileIO;
using UnityEngine.Localization;
using UnityEngine.Localization.Tables;

namespace SR2E.Managers;

public static class SR2ELanguageManger
{
    internal static Dictionary<string, Dictionary<string, string>> addedTranslations = new ();
    internal static Dictionary<string, LocalizedString> sr2etosrlanguage = new ();
    internal static Dictionary<string, (string, string, LocalizedString)> sr2eReplaceOnLanguageChange = new ();
    
    static Dictionary<string, List<Dictionary<string, string>>> languages = new ();
    static Dictionary<string, string> loadedLanguage = new ();
    static Dictionary<string, string> defaultLang = null;
    
    
    public static string translation(string key)
    {
        if (string.IsNullOrEmpty(key) || !loadedLanguage.ContainsKey(key)) return key;
        return loadedLanguage[key];
    }
    
    public static string translation(string key, params object[] args)
    {
        if (string.IsNullOrEmpty(key) || !loadedLanguage.ContainsKey(key)) return key;
        int i = 1;
        string translatedRaw = loadedLanguage[key];
        // somebody optimize this, use for loop. i just couldn't care enough right now.
        foreach (object obj in args)
        {
            translatedRaw = translatedRaw.Replace($"${i}", obj.ToString());
            i++;
        }
    
        return translatedRaw;
    }

    public static void AddLanguages(string CVSText)
    {
        var newLanguages = new Dictionary<string, Dictionary<string, string>>();
        var codeIndexes = new List<string>(){};
        MemoryStream stream = new MemoryStream();
        var cvsBytes = System.Text.Encoding.Default.GetBytes(CVSText);
        stream.Write(cvsBytes,0,cvsBytes.Length);
        stream.Seek(0, SeekOrigin.Begin);
        using (TextFieldParser csvParser = new TextFieldParser(stream))
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
        SR2EEntryPoint.CheckFallBackFont();
    }
    public static void LoadLanguage(string code)
    {
        loadedLanguage = new Dictionary<string, string>();
        if (defaultLang == null)
        {
            defaultLang = new Dictionary<string, string>();
            foreach (var languageDicts in languages[DEFAULT_LANGUAGECODE.Get()])
                foreach (var translation in languageDicts) 
                    defaultLang[translation.Key] = translation.Value;
        }
        loadedLanguage = new Dictionary<string, string>(defaultLang);
        if (code != DEFAULT_LANGUAGECODE.Get()) if (languages.ContainsKey(code))
            foreach (var languageDicts in languages[code]) 
                foreach (var translation in languageDicts)
                    loadedLanguage[translation.Key] = translation.Value;
        SR2EEntryPoint.CheckFallBackFont();
    }
    
    
    
    public static LocalizedString AddTranslation(string localized, string key = null, string table = "Actor")
    {
        if (!InjectTranslations.HasFlag())
        { 
            var tutorial = LocalizationUtil.GetTable("Tutorial");
            foreach (var pair in tutorial.m_TableEntries) return new LocalizedString(tutorial.SharedData.TableCollectionName, pair.Value.SharedEntry.Id);
        }
        StringTable table2 = LocalizationUtil.GetTable(table);


        StringTableEntry existing = null;
        if (string.IsNullOrWhiteSpace(key))
        {
            while (true)
            {
                key = "r."+MiscEUtil.GetRandomString(20);
                StringTableEntry curr = null;
                try
                {
                    curr = table2.GetEntry(key);
                } catch { }
                if (curr == null) break;
            }
        }
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
    public static LocalizedString AddTranslationFromSR2E(string sr2eTranslationID, string key = null, string table = "Actor")
    {
        LocalizedString localizedString = AddTranslation(translation(sr2eTranslationID), key, table);
            
        sr2etosrlanguage.TryAdd(sr2eTranslationID,localizedString);
        sr2eReplaceOnLanguageChange.TryAdd(sr2eTranslationID, (key, table, localizedString));
            
        return localizedString;
    }
        
    public static void SetTranslation(string localized, string key, string table)
    {
        if (!InjectTranslations.HasFlag()) return;
            
        StringTable table2 = LocalizationUtil.GetTable(table);
            
        table2.GetEntry(key).Value = localized;
    }
    public static void SetTranslationFromSR2E(string sr2eTranslationID, string key, string table) => SetTranslation(translation(sr2eTranslationID), key, table);
        
    
    
    
}