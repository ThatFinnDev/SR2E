using System.Reflection;
using System.Runtime.CompilerServices;
using MelonLoader;
using SR2E.Expansion;

[assembly: AssemblyTitle(PurpleCotton.BuildInfo.Name)]
[assembly: AssemblyDescription(PurpleCotton.BuildInfo.Description)]
[assembly: AssemblyCompany(PurpleCotton.BuildInfo.Company)]
[assembly: AssemblyProduct(PurpleCotton.BuildInfo.Name)]
[assembly: AssemblyCopyright($"Created by {PurpleCotton.BuildInfo.Author}")]
[assembly: AssemblyTrademark(PurpleCotton.BuildInfo.Company)]
[assembly: VerifyLoaderVersion(0,6,2, true)]
[assembly: AssemblyVersion(PurpleCotton.BuildInfo.Version)]
[assembly: MelonPriority(-100)]
[assembly: AssemblyFileVersion(PurpleCotton.BuildInfo.Version)]
[assembly: MelonInfo(typeof(PurpleCotton.SlimeMain), PurpleCotton.BuildInfo.Name, PurpleCotton.BuildInfo.Version, PurpleCotton.BuildInfo.Author, PurpleCotton.BuildInfo.DownloadLink)]
[assembly: MelonGame("MonomiPark", "SlimeRancher2")]
[assembly: MelonColor(255, 35, 255, 35)]
[assembly: AssemblyMetadata("co_authors",PurpleCotton.BuildInfo.CoAuthors)]
[assembly: AssemblyMetadata("contributors",PurpleCotton.BuildInfo.Contributors)]
[assembly: AssemblyMetadata("source_code",PurpleCotton.BuildInfo.SourceCode)]
[assembly: AssemblyMetadata("nexus",PurpleCotton.BuildInfo.Nexus)]
[assembly: SR2EExpansion(PurpleCotton.BuildInfo.UsePrism)]
