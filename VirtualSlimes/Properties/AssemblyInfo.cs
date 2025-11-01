using System.Reflection;
using System.Runtime.CompilerServices;
using MelonLoader;
using SR2E.Expansion;

[assembly: AssemblyTitle(VirtualSlime.BuildInfo.Name)]
[assembly: AssemblyDescription(VirtualSlime.BuildInfo.Description)]
[assembly: AssemblyCompany(VirtualSlime.BuildInfo.Company)]
[assembly: AssemblyProduct(VirtualSlime.BuildInfo.Name)]
[assembly: AssemblyCopyright($"Created by {VirtualSlime.BuildInfo.Author}")]
[assembly: AssemblyTrademark(VirtualSlime.BuildInfo.Company)]
[assembly: VerifyLoaderVersion(0,6,2, true)]
[assembly: AssemblyVersion(VirtualSlime.BuildInfo.Version)]
[assembly: MelonPriority(-100)]
[assembly: AssemblyFileVersion(VirtualSlime.BuildInfo.Version)]
[assembly: MelonInfo(typeof(VirtualSlime.SlimeMain), VirtualSlime.BuildInfo.Name, VirtualSlime.BuildInfo.Version, VirtualSlime.BuildInfo.Author, VirtualSlime.BuildInfo.DownloadLink)]
[assembly: MelonGame("MonomiPark", "SlimeRancher2")]
[assembly: MelonColor(255, 35, 255, 35)]
[assembly: AssemblyMetadata("co_authors",VirtualSlime.BuildInfo.CoAuthors)]
[assembly: AssemblyMetadata("contributors",VirtualSlime.BuildInfo.Contributors)]
[assembly: AssemblyMetadata("source_code",VirtualSlime.BuildInfo.SourceCode)]
[assembly: AssemblyMetadata("nexus",VirtualSlime.BuildInfo.Nexus)]
[assembly: SR2EExpansion(VirtualSlime.BuildInfo.UsePrism)]
