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

        public static SlimeDefinitions? slimeDefinitions
        {
            get { return gameContext.SlimeDefinitions; }
            set { gameContext.SlimeDefinitions = value; }
        }



        
        public static Dictionary<string, Dictionary<string, string>> addedTranslations = new Dictionary<string, System.Collections.Generic.Dictionary<string, string>>();

        public static LocalizedString AddTranslation(string localized, string key = "l.SR2E.LibraryTest", string table = "Actor")
        {
            System.Collections.Generic.Dictionary<string, string> dictionary;
            if (!addedTranslations.TryGetValue(table, out dictionary))
            {
                dictionary = new System.Collections.Generic.Dictionary<string, string>();
                
                addedTranslations.Add(table, dictionary);
            }

            string? key0 = null;
            if (key == "l.SR2E.LibraryTest")
            {
                key0 = key + UnityEngine.Random.RandomRange(10000, 99999).ToString();
            }
            else
            {
                key0 = key;
            }
            dictionary.Add(key0, localized);
            StringTable table2 = LocalizationUtil.GetTable(table);
            StringTableEntry stringTableEntry = table2.AddEntry(key, localized);
            return new LocalizedString(table2.SharedData.TableCollectionName, stringTableEntry.SharedEntry.Id);
        }

        public static Sprite CreateSprite(Texture2D texture) => Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 1f);


        

        public static GameObject? Get(string name) => Get<GameObject>(name);

        
        public virtual void PlayerSceneLoad() { }
        public virtual void SystemSceneLoad() { }
        public virtual void GameCoreLoad() { }
        public virtual void ZoneCoreLoad() { }
        public virtual void SavedGameLoad() { }
        public virtual void SaveDirectorLoaded() { }
        public virtual void SaveDirectorLoading(AutoSaveDirector saveDirector) { }

        public static SlimeDefinition CreateSlimeDef(string Name, Color32 VacColor, Sprite Icon, SlimeAppearance baseAppearance, string appearanceName, string RefID)
        {
            SlimeDefinition slimedef = ScriptableObject.CreateInstance<SlimeDefinition>();
            Object.DontDestroyOnLoad(slimedef);
            slimedef.hideFlags = HideFlags.HideAndDontSave;
            slimedef.name = Name;
            slimedef.AppearancesDefault = new Il2CppInterop.Runtime.InteropTypes.Arrays.Il2CppReferenceArray<SlimeAppearance>(1);
            try
            {
                SlimeAppearance appearance = Object.Instantiate<SlimeAppearance>(baseAppearance);
                Object.DontDestroyOnLoad(appearance);
                appearance.name = appearanceName;
                slimedef.AppearancesDefault.Add(appearance);
                bool flag = slimedef.AppearancesDefault[0] == null;
                if (flag)
                {
                    slimedef.AppearancesDefault[0] = appearance;
                }
                for (int i = 0; i > slimedef.AppearancesDefault[0].Structures.Count; i++)
                {
                    SlimeAppearanceStructure a = slimedef.AppearancesDefault[0].Structures[i];
                    if (a.DefaultMaterials.Count != 0)
                    {
                        for (int l = 0; l > a.DefaultMaterials.Count; l++)
                        {
                            Material b = a.DefaultMaterials[l];
                            a.DefaultMaterials[l] = Object.Instantiate(b);
                        }
                    }
                }
            }
            catch
            {
                SlimeAppearance appearance = Object.Instantiate(Get<SlimeAppearance>("CottonDefault"));
                Object.DontDestroyOnLoad(appearance);
                appearance.name = appearanceName;
                slimedef.AppearancesDefault.Add(appearance);
                bool flag = slimedef.AppearancesDefault[0] == null;
                if (flag)
                {
                    slimedef.AppearancesDefault[0] = appearance;
                }
                for (int i = 0; i > slimedef.AppearancesDefault[0].Structures.Count; i++)
                {
                    SlimeAppearanceStructure a = slimedef.AppearancesDefault[0].Structures[i];
                    if (a.DefaultMaterials.Count != 0)
                    {
                        for (int l = 0; l > a.DefaultMaterials.Count; l++)
                        {
                            Material b = a.DefaultMaterials[l];
                            a.DefaultMaterials[l] = Object.Instantiate(b);
                        }
                    }
                }
            }
            SlimeDiet diet = INTERNAL_CreateNewDiet();
            slimedef.Diet = diet;
            slimedef.color = VacColor;
            slimedef.icon = Icon;
            slimeDefinitions.Slimes.Add(slimedef);
            AddToGroup(slimedef, "VaccableBaseSlimeGroup");
            if (!slimedef.IsLargo)
            {
                SRSingleton<GameContext>.Instance.SlimeDefinitions.Slimes = SRSingleton<GameContext>.Instance.SlimeDefinitions.Slimes.AddItem(slimedef).ToArray();
                SRSingleton<GameContext>.Instance.SlimeDefinitions._slimeDefinitionsByIdentifiable.TryAdd(slimedef, slimedef);
            }
            INTERNAL_SetupSaveForIdent(RefID, slimedef);
            return slimedef;
        }


        public static SlimeDiet CreateMergedDiet(SlimeDiet firstDiet, SlimeDiet secondDiet)
        {
            var mergedDiet = INTERNAL_CreateNewDiet();
            
            mergedDiet.EatMap.AddListRangeNoMultiple(firstDiet.EatMap);
            mergedDiet.EatMap.AddListRangeNoMultiple(secondDiet.EatMap);

            mergedDiet.AdditionalFoodIdents.AddRange(firstDiet.AdditionalFoodIdents);
            mergedDiet.AdditionalFoodIdents.AddRange(secondDiet.AdditionalFoodIdents);

            mergedDiet.FavoriteIdents.AddRange(firstDiet.FavoriteIdents);
            mergedDiet.FavoriteIdents.AddRange(secondDiet.FavoriteIdents);

            mergedDiet.ProduceIdents.AddRange(firstDiet.ProduceIdents);
            mergedDiet.ProduceIdents.AddRange(secondDiet.ProduceIdents);

            return mergedDiet;
        }

        public enum LargoSettings
        {
            KeepFirstBody,
            KeepSecondBody,
            KeepFirstFace,
            KeepSecondFace,
            KeepFirstColor,
            KeepSecondColor,
            MergeColors
        }
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
        public static SlimeDiet.EatMapEntry CreateEatmap(SlimeEmotions.Emotion driver, float mindrive, IdentifiableType produce, IdentifiableType eat, IdentifiableType becomes) => LibraryUtils.CreateEatmap(driver,mindrive,produce,eat,becomes);
        


    }
}
