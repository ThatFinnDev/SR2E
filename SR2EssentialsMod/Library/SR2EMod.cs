using Il2CppInterop.Runtime.Injection;
using Il2CppMonomiPark.SlimeRancher.Persist;
using Il2CppMonomiPark.SlimeRancher.Script.Util;
using Il2CppMonomiPark.SlimeRancher.UI;
using SR2E;
using SR2E.Library.Storage;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using UnityEngine.Localization;
using UnityEngine.Localization.Tables;


namespace SR2E.Library
{
    public class SR2EMod : MelonMod
    {
        public Semver.SemVersion version
        {
            get
            {
                return Info.Version;
            }
        }
        public static GameV04? Save;
        
        
        public virtual void PlayerSceneLoad()
        {
        }
        public virtual void SystemSceneLoad()
        {
        }
        public virtual void GameCoreLoad()
        {
        }
        public virtual void ZoneCoreLoad()
        {
        }
        public virtual void SavedGameLoad()
        {
        }
        public virtual void SaveDirectorLoaded()
        {
        }

        
        
        
        public static SlimeDiet.EatMapEntry CreateEatmap(SlimeEmotions.Emotion driver, float mindrive, IdentifiableType produce, IdentifiableType eat, IdentifiableType becomes) => LibraryUtils.CreateEatmap(driver,mindrive,produce,eat,becomes);

        
        /*
        internal static void AddPageToPediaEntry(PediaEntry pedia, string pageText, PediaPage template)
        {
            AddTranslation(pageText, INTERNAL_CreatePediaPageKey(template.name.ToLower(), pedia), "PediaPage");
        }

        internal static string INTERNAL_CreatePediaPageKey(string prefix, PediaEntry pediaEntryName)
        {
            return "m." + prefix + "." + pediaEntryName.name.ToLower().Replace(" ", "_") + ".page." + 1.ToString();
        }

        internal static string INTERNAL_CreatePediaKey(string prefix, IdentifiableType identifiableType)
        {
            return "m." + prefix + "." + identifiableType.localizationSuffix;
        }

        internal static string INTERNAL_CreatePediaKey(string prefix, string suffix)
        {
            return "m." + prefix + "." + suffix;
        }

        public static void MakeSellable(IdentifiableType ident, float marketValue, float marketSaturation)
        {
            if (marketData.ContainsKey(ident))
            {
                MelonLogger.Error("Failed to make object sellable: The object is already sellable");
                return;
            }
            marketPlortEntries.Add(new MarketUI.PlortEntry { identType = ident });
            marketData.Add(ident, new ModdedMarketData(marketSaturation, marketValue));
        }
        */
        


    }
}
