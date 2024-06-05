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
    public static string translation(string key, object replace1)
    {
        if (String.IsNullOrEmpty(key) || !loadedLanguage.ContainsKey(key)) return key;
        return loadedLanguage[key].Replace("$1",replace1.ToString());
    }
    public static string translation(string key, object replace1, object replace2)
    {
        if (String.IsNullOrEmpty(key) || !loadedLanguage.ContainsKey(key)) return key;
        return loadedLanguage[key].Replace("$1",replace1.ToString()).Replace("$2",replace2.ToString());
    }
    public static string translation(string key, object replace1, object replace2, object replace3)
    {
        if (String.IsNullOrEmpty(key) || !loadedLanguage.ContainsKey(key)) return key;
        return loadedLanguage[key].Replace("$1",replace1.ToString()).Replace("$2",replace2.ToString()).Replace("$3",replace3.ToString());
    }
    public static string translation(string key, object replace1, object replace2, object replace3, object replace4)
    {
        if (String.IsNullOrEmpty(key) || !loadedLanguage.ContainsKey(key)) return key;
        return loadedLanguage[key].Replace("$1",replace1.ToString()).Replace("$2",replace2.ToString()).Replace("$3",replace3.ToString()).Replace("$4",replace4.ToString());
    }
    public static string translation(string key, object replace1, object replace2, object replace3, object replace4, object replace5)
    {
        if (String.IsNullOrEmpty(key) || !loadedLanguage.ContainsKey(key)) return key;
        return loadedLanguage[key].Replace("$1",replace1.ToString()).Replace("$2",replace2.ToString()).Replace("$3",replace3.ToString()).Replace("$4",replace4.ToString()).Replace("$5",replace5.ToString());
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
                loadedLanguage.Add(key,split[1].Replace("\\n", "\n"));
        }
                
    }
}