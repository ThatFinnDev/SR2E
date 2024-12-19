using System.Reflection;
using System.Runtime.CompilerServices;
using MelonLoader;
using SR2E.Addons;

[assembly: AssemblyTitle(SR2EExampleAddon.BuildInfo.Name)]
[assembly: AssemblyDescription(SR2EExampleAddon.BuildInfo.Description)]
[assembly: AssemblyCompany(SR2EExampleAddon.BuildInfo.Company)]
[assembly: AssemblyProduct(SR2EExampleAddon.BuildInfo.Name)]
[assembly: AssemblyCopyright($"Created by {SR2EExampleAddon.BuildInfo.Author}")]
[assembly: AssemblyTrademark(SR2EExampleAddon.BuildInfo.Company)]
[assembly: VerifyLoaderVersion(0,6,2, true)]
[assembly: AssemblyVersion(SR2EExampleAddon.BuildInfo.Version)]
[assembly: MelonPriority(-100)]
[assembly: AssemblyFileVersion(SR2EExampleAddon.BuildInfo.Version)]
[assembly: MelonInfo(typeof(SR2EExampleAddon.AddonEntryPoint), SR2EExampleAddon.BuildInfo.Name, SR2EExampleAddon.BuildInfo.Version, SR2EExampleAddon.BuildInfo.Author, SR2EExampleAddon.BuildInfo.DownloadLink)]
[assembly: MelonGame("MonomiPark", "SlimeRancher2")]
[assembly: MelonColor(255, 35, 255, 35)]
[assembly: SR2EAddon()]
// Create and Setup a MelonGame Attribute to mark a Melon as Universal or Compatible with specific Games.
// If no MelonGame Attribute is found or any of the Values for any MelonGame Attribute on the Melon is null or empty it will be assumed the Melon is Universal.
// Values for MelonGame Attribute can be found in the Game's app.info file or printed at the top of every log directly beneath the Unity version.
