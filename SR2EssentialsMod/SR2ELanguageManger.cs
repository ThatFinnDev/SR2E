using System;
using System.Reflection;

namespace SR2E;

public static class SR2ELanguageManger
{
    internal const string defaultLanguageCode = "en";
    internal static Dictionary<string, List<string>> languages = new Dictionary<string, List<string>>();
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

    public static void AddLanguage(string code, string text)
    {
        if (languages.ContainsKey(code)) languages[code].Add(text);
        else languages.Add(code, new List<string> { text });
    }
    public static void LoadLanguage(string code)
    {
        if (!languages.ContainsKey(code)) return;

        loadedLanguage = new Dictionary<string, string>();
        foreach (string language in languages[code])
            foreach (string line in language.Split("\n"))
            {
                if (String.IsNullOrWhiteSpace(line)) continue;
                string[] split = line.Split("=", 2);
                string key = split[0];
                if (!loadedLanguage.ContainsKey(key))
                    loadedLanguage.Add(key, split[1].Replace("\\n", "\n").Replace("<equals>", "="));
            }
        

    }
}