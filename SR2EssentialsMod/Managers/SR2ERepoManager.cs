using System.Net.Http;
using System.Threading;
using Newtonsoft.Json;

namespace SR2E.Managers;

using SR2E.Repos;
public static class SR2ERepoManager
{
    internal static List<Repo> repos = new List<Repo>();
    internal static void Start()
    {
        
        Thread fileThread = new Thread(StartSeperate);
        fileThread.Start();
    }

    static void StartSeperate()
    {
        
        List<RepoSave> repoSaves = new List<RepoSave>(){new RepoSave("official","https://api.sr2e.thatfinn.dev/")};
        repoSaves.AddRange(SR2ESaveManager.data.repos);
        foreach (RepoSave repoSave in repoSaves)
        {
            var repo = CheckRepo(repoSave);
            if(repo!=null)
                repos.Add(repo);
           
        }
    }
    static JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings
    {
        Error = (sender, args) =>
        {
            if(DebugLogging.HasFlag()) MelonLogger.Msg($"Error: {args.ErrorContext.Error.Message}");
            //if (args.ErrorContext.Member is string memberName && args.ErrorContext.Path.Contains(nameof(SR2ESaveData.fonts))) 
            //    ((Dictionary<string, SR2EMenuFont>)args.ErrorContext.OriginalObject)[memberName] = SR2EMenuFont.Default;
            //if (args.ErrorContext.Member is string memberName2 && args.ErrorContext.Path.Contains(nameof(SR2ESaveData.themes))) 
            //    ((Dictionary<string, SR2EMenuTheme>)args.ErrorContext.OriginalObject)[memberName2] = SR2EMenuTheme.Default;
            args.ErrorContext.Handled = true;
        }
    };
    static Repo CheckRepo(RepoSave repoSave)
    {
        try
        {
            using (HttpClient client = new HttpClient())
            {
                var response = client.GetStringAsync(repoSave.url).Result;
                
                try { return JsonConvert.DeserializeObject<Repo>(response, jsonSerializerSettings); }
                catch (Exception e) 
                { 
                    MelonLogger.Msg("SR2ERepo is broken"); 
                    MelonLogger.Msg(e);
                }
            }
        }
        catch (System.Exception e)
        {
            MelonLogger.Msg("Error fetching file: " + e.Message);
        }

        return null;
    }
}