using System.Reflection;
using Il2CppNewtonsoft.Json.Utilities;
using SR2E.Expansion;

[assembly: AssemblyTitle(SR2E.BuildInfo.Name)]
[assembly: AssemblyDescription(SR2E.BuildInfo.Description)]
[assembly: AssemblyCompany(null)]
[assembly: AssemblyProduct(SR2E.BuildInfo.Name)]
[assembly: AssemblyCopyright($"Created by {SR2E.BuildInfo.Author}")]
[assembly: AssemblyTrademark(null)]
[assembly: VerifyLoaderVersion(0,7,1, true)]
[assembly: AssemblyVersion(SR2E.BuildInfo.CodeVersion)]
[assembly: MelonPriority(-10000)]
[assembly: AssemblyFileVersion(SR2E.BuildInfo.CodeVersion)]
[assembly: MelonInfo(typeof(SR2E.SR2EEntryPoint), SR2E.BuildInfo.Name, SR2E.BuildInfo.CodeVersion, SR2E.BuildInfo.Author, SR2E.BuildInfo.DownloadLink)]
[assembly: MelonGame("MonomiPark", "SlimeRancher2")]
[assembly: MelonColor(255, 35, 255, 35)]
[assembly: AssemblyMetadata("display_version",SR2E.BuildInfo.DisplayVersion)]
[assembly: AssemblyMetadata("contributors",SR2E.BuildInfo.Contributors)]
[assembly: AssemblyMetadata("source_code",SR2E.BuildInfo.SourceCode)]
[assembly: AssemblyMetadata("nexus",SR2E.BuildInfo.Nexus)]
[assembly: HarmonyDontPatchAll()]