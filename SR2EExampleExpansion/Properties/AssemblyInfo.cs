﻿using System.Reflection;
using System.Runtime.CompilerServices;
using MelonLoader;
using SR2E.Expansion;

[assembly: AssemblyTitle(SR2EExampleExpansion.BuildInfo.Name)]
[assembly: AssemblyDescription(SR2EExampleExpansion.BuildInfo.Description)]
[assembly: AssemblyCompany(SR2EExampleExpansion.BuildInfo.Company)]
[assembly: AssemblyProduct(SR2EExampleExpansion.BuildInfo.Name)]
[assembly: AssemblyCopyright($"Created by {SR2EExampleExpansion.BuildInfo.Author}")]
[assembly: AssemblyTrademark(SR2EExampleExpansion.BuildInfo.Company)]
[assembly: VerifyLoaderVersion(0,6,2, true)]
[assembly: AssemblyVersion(SR2EExampleExpansion.BuildInfo.Version)]
[assembly: MelonPriority(-100)]
[assembly: AssemblyFileVersion(SR2EExampleExpansion.BuildInfo.Version)]
[assembly: MelonInfo(typeof(SR2EExampleExpansion.ExpansionEntryPoint), SR2EExampleExpansion.BuildInfo.Name, SR2EExampleExpansion.BuildInfo.Version, SR2EExampleExpansion.BuildInfo.Author, SR2EExampleExpansion.BuildInfo.DownloadLink)]
[assembly: MelonGame("MonomiPark", "SlimeRancher2")]
[assembly: MelonColor(255, 35, 255, 35)]
[assembly: SR2EExpansion()]
// Create and Setup a MelonGame Attribute to mark a Melon as Universal or Compatible with specific Games.
// If no MelonGame Attribute is found or any of the Values for any MelonGame Attribute on the Melon is null or empty it will be assumed the Melon is Universal.
// Values for MelonGame Attribute can be found in the Game's app.info file or printed at the top of every log directly beneath the Unity version.
