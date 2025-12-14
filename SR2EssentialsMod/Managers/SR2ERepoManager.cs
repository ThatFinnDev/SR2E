using System.Net.Http;
using System.Threading;
using Newtonsoft.Json;
using SR2E.Repos;

namespace SR2E.Managers;

internal static class SR2ERepoManager
{
    internal static Dictionary<string,Repo> repos = new Dictionary<string, Repo>();
    internal static void Start()
    {
        
        Thread fileThread = new Thread(StartSeperate);
        fileThread.Start();
    }

    static void StartSeperate()
    {
        
        List<RepoSave> repoSaves = new List<RepoSave>(){new RepoSave("official","https://api.sr2e.sr2.dev/repo")};
        if(UseMockRepo.HasFlag()) repoSaves.Add(new RepoSave("official_mock","https://api.sr2e.sr2.dev/mockrepo"));
        repoSaves.AddRange(SR2ESaveManager.data.repos);
        foreach (RepoSave repoSave in repoSaves)
        {
            var repo = CheckRepo(repoSave);
            repos.Add(repoSave.identifier,repo);
           
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

                try
                {
                    var repo = JsonConvert.DeserializeObject<Repo>(response, jsonSerializerSettings);
                    if (repo.identifier != repoSave.identifier)
                    {
                        MelonLogger.Msg("SR2ERepo identifier changed"); 
                        return null;
                    }
                    return repo;

                }
                catch (Exception e) 
                { 
                    MelonLogger.Error("Error fetching repo: "+repoSave.url);
                    MelonLogger.Msg("The json file is broken! Please contact the repo maintainer!"); 
                    MelonLogger.Msg(e);
                }
            }
        }
        catch (System.Exception e)
        {
                MelonLogger.Error("Error fetching repo: "+repoSave.url);
                MelonLogger.Error(e.Message);
                MelonLogger.Error("This is normal if you are not connected to the internet!");
        }

        return null;
    }
}