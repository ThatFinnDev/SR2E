using System;

namespace SR2E.Repos;

[System.Serializable]
public class RepoMod
{
    public string name;
    public string author;
    public string coauthor;
    public string description;
    public string company;
    public string trademark;
    public string team;
    public string copyright;
    public string github_repository;
    public string header_url;
    public string icon_url;
    public byte colorR;
    public byte colorG;
    public byte colorB;
    public byte colorA;
    public bool universal;
    public bool expansion;
    public bool plugin;
    public List<RepoModVersion> versions = new List<RepoModVersion>();

    public RepoModVersion getLatestVersion(string branch)
    {
        RepoModVersion latestVersion = null;
        foreach (var version in versions)
        {
            if (branch == version.branch)
            {
                if(latestVersion == null)
                    latestVersion = version;
                else
                {
                    DateTime dateNew = DateTime.Parse(version.release_date, null, System.Globalization.DateTimeStyles.AdjustToUniversal);
                    DateTime dateOld = DateTime.Parse(latestVersion.release_date, null, System.Globalization.DateTimeStyles.AdjustToUniversal);
                    if(dateNew>dateOld)
                        latestVersion = version;
                }
            }
        }

        return latestVersion;
    }
}