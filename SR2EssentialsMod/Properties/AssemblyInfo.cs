using System.Reflection;

[assembly: AssemblyTitle(SR2E.BuildInfo.Name)]
[assembly: AssemblyDescription(SR2E.BuildInfo.Description)]
[assembly: AssemblyCompany(SR2E.BuildInfo.Company)]
[assembly: AssemblyProduct(SR2E.BuildInfo.Name)]
[assembly: AssemblyCopyright($"Created by {SR2E.BuildInfo.Author}")]
[assembly: AssemblyTrademark(SR2E.BuildInfo.Company)]
// [assembly: AssemblyVersion(SR2E.BuildInfo.Version)]
[assembly: MelonPriority(-10000)]
[assembly: AssemblyFileVersion(SR2E.BuildInfo.Version)]
[assembly: MelonInfo(typeof(SR2E.Library.SR2EMod), SR2E.BuildInfo.Name, SR2E.BuildInfo.Version, SR2E.BuildInfo.Author, SR2E.BuildInfo.DownloadLink)]
[assembly: MelonColor(255, 35, 255, 35)]
[assembly: MelonAdditionalDependencies("SR2E Library")]

// Create and Setup a MelonGame Attribute to mark a Melon as Universal or Compatible with specific Games.
// If no MelonGame Attribute is found or any of the Values for any MelonGame Attribute on the Melon is null or empty it will be assumed the Melon is Universal.
// Values for MelonGame Attribute can be found in the Game's app.info file or printed at the top of every log directly beneath the Unity version.
[assembly: MelonGame("MonomiPark", "SlimeRancher2")]