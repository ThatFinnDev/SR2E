using System.Reflection;
using Il2CppNewtonsoft.Json.Utilities;
using SR2E.Expansion;

[assembly: AssemblyTitle(SR2E.BuildInfo.NAME)]
[assembly: AssemblyDescription(SR2E.BuildInfo.DESCRIPTION)]
[assembly: AssemblyCompany(null)]
[assembly: AssemblyProduct(SR2E.BuildInfo.NAME)]
[assembly: AssemblyCopyright($"Created by {SR2E.BuildInfo.AUTHOR}")]
[assembly: AssemblyTrademark(null)]
[assembly: VerifyLoaderVersion(0,7,1, true)]
[assembly: AssemblyVersion(SR2E.BuildInfo.CODE_VERSION)]
[assembly: MelonPriority(-10000)]
[assembly: AssemblyFileVersion(SR2E.BuildInfo.CODE_VERSION)]
[assembly: MelonInfo(typeof(SR2E.SR2EEntryPoint), SR2E.BuildInfo.NAME, SR2E.BuildInfo.CODE_VERSION, SR2E.BuildInfo.AUTHOR, SR2E.BuildInfo.DOWNLOAD_LINK)]
[assembly: MelonGame("MonomiPark", "SlimeRancher2")]
[assembly: MelonColor(255, 35, 255, 35)]
[assembly: SR2ECoAuthor(SR2E.BuildInfo.CO_AUTHORS)]
[assembly: SR2EDisplayVersion(SR2E.BuildInfo.DISPLAY_VERSION)]