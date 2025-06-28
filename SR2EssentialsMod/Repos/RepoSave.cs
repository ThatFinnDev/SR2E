namespace SR2E.Repos;

[System.Serializable]
public class RepoSave
{
    public string identifier;
    public string url;

    public RepoSave()
    {
        
    }
    public RepoSave(string identifier, string url)
    {
        this.identifier = identifier;
        this.url = url;
    }
}