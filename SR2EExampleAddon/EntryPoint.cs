using MelonLoader;
using UnityEngine;

namespace SR2EExampleAddon
{
    public static class BuildInfo
    {
        public const string Name = "SR2EExampleAddon"; // Name of the Addon.  (MUST BE SET)
        public const string Description = "Test Addon for SR2E"; // Description for the Addon.  (Set as null if none)
        public const string Author = "ThatFinn"; // Author of the Addon.  (MUST BE SET)
        public const string Company = null; // Company that made the Addon.  (Set as null if none)
        public const string Version = "1.0.0"; // Version of the Addon.  (MUST BE SET)
        public const string DownloadLink = "https://www.nexusmods.com/slimerancher2/mods/60"; // Download Link for the Addon.  (Set as null if none)
    }

    public class SR2EEntryPoint : MelonMod
    {
        public override void OnInitializeMelon()
        {
        }
    }

}