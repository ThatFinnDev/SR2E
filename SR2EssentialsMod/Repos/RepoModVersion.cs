namespace SR2E.Repos;

[System.Serializable]
public class RepoModVersion
{
    public List<string> ml_ver;
    public List<string> sr2_ver;
    public string sr2_compile_ver;
    public string release_date;
    public string github_tag;
    public string download_url;
    public string branch = "release";
}