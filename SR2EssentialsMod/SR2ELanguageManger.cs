using System;
using System.Reflection;

namespace SR2E;

public static class SR2ELanguageManger
{
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
    public static void LoadLanguage()
    {
        System.IO.Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("SR2E.en.txt");
        byte[] buffer = new byte[16 * 1024];
        System.IO.MemoryStream ms = new System.IO.MemoryStream();
        int read;
        while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
            ms.Write(buffer, 0, read);
        
        var language = System.Text.Encoding.Default.GetString(ms.ToArray()).Split("\n");

        foreach (string line in language)
        {
            if (String.IsNullOrWhiteSpace(line)) continue;
            string[] split = line.Split("=", 2);
            string key = split[0];
            if(!loadedLanguage.ContainsKey(key))
                loadedLanguage.Add(key,split[1].Replace("\\n", "\n").Replace("<equals>","="));
        }
                
    }
}