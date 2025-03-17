using System.Reflection;
using SR2E.Expansion;

[assembly: AssemblyTitle(SR2E.BuildInfo.Name)]
[assembly: AssemblyDescription(SR2E.BuildInfo.Description)]
[assembly: AssemblyCompany(null)]
[assembly: AssemblyProduct(SR2E.BuildInfo.Name)]
[assembly: AssemblyCopyright($"Created by {SR2E.BuildInfo.Author}")]
[assembly: AssemblyTrademark(null)]
[assembly: VerifyLoaderVersion(0,7,0, true)]
[assembly: AssemblyVersion(SR2E.BuildInfo.CodeVersion)]
[assembly: MelonPriority(-10000)]
[assembly: AssemblyFileVersion(SR2E.BuildInfo.CodeVersion)]
[assembly: MelonInfo(typeof(SR2E.SR2EEntryPoint), SR2E.BuildInfo.Name, SR2E.BuildInfo.CodeVersion, SR2E.BuildInfo.Author, SR2E.BuildInfo.DownloadLink)]
[assembly: MelonGame("MonomiPark", "SlimeRancher2")]
[assembly: MelonColor(255, 35, 255, 35)]
[assembly: SR2ECoAuthor(SR2E.BuildInfo.CoAuthors)]
[assembly: SR2EDisplayVersion(SR2E.BuildInfo.DisplayVersion)]
// Create and Setup a MelonGame Attribute to mark a Melon as Universal or Compatible with specific Games.
// If no MelonGame Attribute is found or any of the Values for any MelonGame Attribute on the Melon is null or empty it will be assumed the Melon is Universal.
// Values for MelonGame Attribute can be found in the Game's app.info file or printed at the top of every log directly beneath the Unity version.
