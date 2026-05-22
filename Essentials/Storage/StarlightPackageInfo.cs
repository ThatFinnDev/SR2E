using System.Reflection;
using Starlight.Enums;

namespace Starlight.Storage;

public record struct StarlightPackageInfo
{
    public string ID = null;
    public string Name = null;
    public string Description = null;
    public string Version = null;
    public string Author = null;
    public string[] CoAuthors = null;
    public string[] Contributors = null;
    public string[] Dependencies = null;
    public string SourceCode = null;
    public string Nexus = null;
    public string Discord = null;
    public bool UsePrism = false;
    public string IconPath = "Assets.icon.png";
    public ExpansionLoadTime LoadTime = ExpansionLoadTime.Startup;
    public ExpansionUnloadTime UnloadTime = ExpansionUnloadTime.Never;
    public MultiplayerRequirement MultiplayerRequirement = MultiplayerRequirement.ServerAndClient;

    public string GetDllName() => DLLName;
    public Assembly GetAssembly() => RunningAssembly;
    public int GetExpansionVersion() => ExpansionVersion;
    public Sprite GetIcon() => Icon;
    public PackageType GetPackageType() => Type;
    public dynamic GetEntrypoint() => MainClass;
    public bool HasOptionalFile() => MainClass;
    internal dynamic MainClass = null;
    internal string DLLName = null;
    internal Assembly RunningAssembly = null;
    internal int ExpansionVersion = 0;
    internal Sprite Icon;
    internal PackageType Type = PackageType.Expansion;
    internal bool HasOptionFile = false;
    
    public StarlightPackageInfo()
    {
        
    }
}