namespace SR2E.Repos;

[System.Serializable]
public class Repo
{
    public string header_url;
    public string icon_url;
    public string identifier;
    public string name;
    public string description;
    public List<RepoMod> mods = new List<RepoMod>();
}