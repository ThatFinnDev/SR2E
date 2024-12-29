using System;
using System.IO;
using System.Reflection;
using Microsoft.VisualBasic.FileIO;

namespace SR2E;

public static class SR2ELanguageManger
{
    internal static Dictionary<string, List<Dictionary<string, string>>> languages = new Dictionary<string, List<Dictionary<string, string>>>();
    static Dictionary<string, string> loadedLanguage = new Dictionary<string, string>();
    public static string translation(string key)
    {
        if (String.IsNullOrEmpty(key) || !loadedLanguage.ContainsKey(key)) return key;
        return loadedLanguage[key];
    }
    
    public static string translation(string key, params object[] args)
    {
        if (String.IsNullOrEmpty(key) || !loadedLanguage.ContainsKey(key)) return key;
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
                    if (parts == null) return;
                    if (parts.Length < 1) return;
                    bool isKeys = true;
                    foreach (string code in parts)
                    {
                        if (isKeys) isKeys = false;
                        else
                        {
                            if (!newLanguages.ContainsKey(code)) newLanguages[code] = new Dictionary<string, string>();
                            codeIndexes.Add(code);
                        }
                    }
                }
                else
                {
                    if (parts == null) continue;
                    if (parts.Length < 1) continue;
                    bool isKey = true;
                    string key = parts[0];
                    int i = 0;
                    foreach (string translation in parts)
                    {
                        if (isKey) isKey = false;
                        else
                        {
                            newLanguages[codeIndexes[i]][key] = translation.Replace("\\n","\n");
                            i++;
                        }
                    }
                }
            }
        }
        foreach (var newLanguage in newLanguages)
        {
            var langCode=newLanguage.Key;
            if (!languages.ContainsKey(langCode)) languages.Add(langCode, new List<Dictionary<string, string>>() { newLanguage.Value });
            else languages[langCode].Add(newLanguage.Value);
        }
    }
    public static void LoadLanguage(string code)
    {
        if (!languages.ContainsKey(code)) return;
        loadedLanguage = new Dictionary<string, string>();
        foreach (var languageDicts in languages[code])
            foreach (var translation in languageDicts)
                loadedLanguage[translation.Key] = translation.Value;
            
    }
    
}