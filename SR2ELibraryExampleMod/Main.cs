using UnityEngine;
using Il2Cpp;
using Il2CppInterop.Runtime.Injection;
using System.Reflection;
using Il2CppMonomiPark.SlimeRancher.Weather.Activity;
using SR2E.Library;
using static SR2E.Library.LibraryUtils;
using Il2CppMonomiPark.SlimeRancher.Weather;
using SR2E;
using Il2CppMonomiPark.SlimeRancher.World;
using Object = UnityEngine.Object;

namespace NullSlime
{
    public class SlimeMain : SR2EMod
    {
        public static Texture2D LoadImage(string filename)
        {
            Assembly executingAssembly = Assembly.GetExecutingAssembly();
            Stream manifestResourceStream = executingAssembly.GetManifestResourceStream(executingAssembly.GetName().Name + "." + filename + ".png");
            byte[] array = new byte[manifestResourceStream.Length];
            manifestResourceStream.Read(array, 0, array.Length);
            Texture2D texture2D = new Texture2D(1, 1);
            ImageConversion.LoadImage(texture2D, array);
            texture2D.filterMode = FilterMode.Bilinear;
            return texture2D;
        }

        public static Color32 vacColor_null = new Color32(84, 84, 84, 255);
        public static Color32 topColor_null = new Color32(181, 181, 181, 255);
        public static Color32 middleColor_null = new Color32(125, 125, 125, 255);
        public static Color32 bottomColor_null = new Color32(59, 59, 59, 255);
        public static GameObject slimePrefab_null;
        public static GameObject plortPrefab_null;
        public static SlimeDefinition slimeDefinition_null;
        public static IdentifiableType plortDefinition_null;
        public static SpawnActorActivity spawnerActivity_null;

        public static Color32 vacColor_data = new Color32(8, 191, 57, 255);
        public static Color32 topColor_data = new Color32(81, 232, 122, 255);
        public static Color32 middleColor_data = new Color32(62, 158, 88, 255);
        public static Color32 bottomColor_data = new Color32(7, 115, 36, 255);
        public static GameObject slimePrefab_data;
        public static GameObject plortPrefab_data;
        public static SlimeDefinition slimeDefinition_data;
        public static IdentifiableType plortDefinition_data;
        public static SpawnActorActivity spawnerActivity_data;
        private static Material data_mat;

        int CraftFoodGroupID;

        public override void OnEarlyInitializeMelon()
        {
            CraftFoodGroupID = Enum.GetNames<SlimeEat.FoodGroup>().Length + 1;
            EnumInjector.RegisterEnumInIl2Cpp<SlimeEat.FoodGroup>();
        }
        public override void SaveDirectorLoading(AutoSaveDirector saveDirector)
        {

            slimeDefinition_null = CreateSlimeDef("Null", vacColor_null, null, Get<SlimeAppearance>("SaberDefault"), "NullDefault", "SlimeDefinition.NullSlime");
            plortDefinition_null = CreatePlortType("Null", vacColor_null, null, "IdentifiableType.NullPlort", 25f, 80f);
            plortPrefab_null = CreatePrefab("plortNull", Get<GameObject>("plortPhosphor"));
            slimePrefab_null = CreatePrefab("slimeNull", Get<GameObject>("slimeSaber"));

            slimeDefinition_data = CreateSlimeDef("Data", vacColor_data, null, Get<SlimeAppearance>("PinkDefault"), "DataDefault", "SlimeDefinition.DataSlime");
            plortDefinition_data = CreatePlortType("Data", vacColor_data, null, "IdentifiableType.DataPlort", 18f, 67f);
            plortPrefab_data = CreatePrefab("plortData", Get<GameObject>("plortYolky"));
            slimePrefab_data = CreatePrefab("slimeData", Get<GameObject>("slimePink"));
        }
        public override void SaveDirectorLoaded()
        {
            slimeDefinition_null.AppearancesDefault[0]._structures[0].DefaultMaterials[0] = Object.Instantiate(slimeDefinition_null.AppearancesDefault[0]._structures[0].DefaultMaterials[0]);
            plortDefinition_null.SetObjectPrefab(plortPrefab_null);
            slimeDefinition_null.SetObjectPrefab(slimePrefab_null);
            slimePrefab_null.SetObjectIdent(slimeDefinition_null);
            plortPrefab_null.SetObjectIdent(plortDefinition_null);
            slimeDefinition_null.AddProduceIdent(plortDefinition_null);


            data_mat = Object.Instantiate(GetSlime("Tabby").AppearancesDefault[0]._structures[0].DefaultMaterials[0]);
            slimeDefinition_data.AppearancesDefault[0]._structures[0].DefaultMaterials[0] = data_mat;
            plortDefinition_data.SetObjectPrefab(plortPrefab_data);
            slimeDefinition_data.SetObjectPrefab(slimePrefab_data);
            slimePrefab_data.SetObjectIdent(slimeDefinition_data);
            plortPrefab_data.SetObjectIdent(plortDefinition_data);
            slimeDefinition_data.AddProduceIdent(plortDefinition_data);

            Localization();
            Coloring();
            Images();
            Grouping();
            Eatmaps();
            SetupSpawning();

            slimeDefinition_data.properties.Properties[2].DefaultValue = float.NegativeInfinity;
            slimeDefinition_data.properties.Properties[3].DefaultValue = float.PositiveInfinity;
        }

        public static void Localization()
        {


            slimeDefinition_null.localizedName = AddTranslation("Null Slime", "l.nullSlime");
            slimeDefinition_null.localizationSuffix = "null_slime";
            plortDefinition_null.localizedName = AddTranslation("Null Plort", "l.nullPlort");
            plortDefinition_null.localizationSuffix = "null_plort";

            slimeDefinition_data.localizedName = AddTranslation("Data Slime", "l.dataSlime");
            slimeDefinition_data.localizationSuffix = "data_slime";
            plortDefinition_data.localizedName = AddTranslation("Data Plort", "l.dataPlort");
            plortDefinition_data.localizationSuffix = "data_plort";
        }

        public static void Coloring()
        {
            slimeDefinition_null.AppearancesDefault [0]._splatColor = bottomColor_null;
            slimeDefinition_null.AppearancesDefault [0].SetAppearanceVacColor(vacColor_null);
            slimeDefinition_null.SetSlimeColor(topColor_null, middleColor_null, bottomColor_null, middleColor_null, 0, 0, false, 0);
            SetPlortColor(topColor_null, middleColor_null, bottomColor_null, plortPrefab_null);

            slimeDefinition_data.AppearancesDefault [0]._splatColor = bottomColor_null;
            slimeDefinition_data.AppearancesDefault [0].SetAppearanceVacColor(vacColor_data);
            slimeDefinition_data.SetSlimeColor(topColor_data, middleColor_data, bottomColor_data, middleColor_data, 0, 0, false, 0);
            SetPlortColor(topColor_data, middleColor_data, bottomColor_data, plortPrefab_data);
        }

        public static void Images()
        {
            slimeDefinition_null.AppearancesDefault[0]._structures[0].DefaultMaterials[0].SetTexture("_StripeTexture", LoadImage("body_stripes_null"));
            plortDefinition_null.icon = LoadImage("iconPlortNull").ConvertToSprite();
            slimeDefinition_null.icon = LoadImage("iconSlimeNull").ConvertToSprite();
            slimeDefinition_null.AppearancesDefault[0]._icon = LoadImage("iconSlimeNull").ConvertToSprite();
            
            plortDefinition_data.icon = LoadImage("iconPlortData").ConvertToSprite();
            slimeDefinition_data.icon = LoadImage("iconSlimeData").ConvertToSprite();
            slimeDefinition_data.AppearancesDefault[0]._icon = LoadImage("iconSlimeData").ConvertToSprite();
            data_mat.SetTexture("_StripeTexture", LoadImage("body_stripes_data"));
            plortDefinition_data.prefab.GetComponent<MeshRenderer>().material.SetTexture("_StripeTexture", LoadImage("dataPlort_falloff_map"));
        }

        public static void Grouping()
        {

            plortDefinition_null.AddToGroup("PlortGroup");
            slimeDefinition_null.AddToGroup("SlimesGroup");
            slimeDefinition_null.AddToGroup("VaccableBaseSlimeGroup");
            slimeDefinition_null.AddToGroup("VaccableNonLiquids");

            plortDefinition_data.AddToGroup("PlortGroup");
            slimeDefinition_data.AddToGroup("SlimesGroup");
            slimeDefinition_data.AddToGroup("VaccableBaseSlimeGroup");
            slimeDefinition_data.AddToGroup("VaccableNonLiquids");
        }

        public static void Eatmaps()
        {
            var craftGroup = Get<IdentifiableTypeGroup>("CraftGroup");
            craftGroup.isFood = true;
            slimeDefinition_null.Diet.MajorFoodIdentifiableTypeGroups = slimeDefinition_null.Diet.MajorFoodIdentifiableTypeGroups.Add(craftGroup);
            slimeDefinition_null.Diet.MajorFoodGroups = new Il2CppInterop.Runtime.InteropTypes.Arrays.Il2CppStructArray<SlimeEat.FoodGroup>(0);
            slimeDefinition_null.Diet.EatMap = new Il2CppSystem.Collections.Generic.List<SlimeDiet.EatMapEntry>(0);

            foreach (var item in Get<IdentifiableTypeGroup>("CraftGroup").GetAllMembersArray())
            {
                slimeDefinition_null.AddExtraEatIdent(item);
            }
            slimeDefinition_null.AddExtraEatIdent(plortDefinition_data);
            slimeDefinition_null.SetFavoriteProduceCount(10);
            slimeDefinition_null.AddFavorite(plortDefinition_data);
            slimeDefinition_null.RefreshEatmap();


            slimeDefinition_null.AddFavorite(GetSlime("Tarr"));
            slimeDefinition_data.AddSlimeFoodIdentGroup(slimes);
            slimeDefinition_data.RefreshEatmap();
        }

        public static void SetupSpawning()
        {
            var slimeRainFields = WeatherState("Slime Rain State Fields");
            spawnerActivity_null = new SpawnActorActivity()
            {
                name = "Null Slime Rain",
                ActorType = slimeDefinition_null,
                IntensityDivisor = 0.3f,
                _intensity = 0.2f,
                SecondsBetweenSpawns = new Il2Cpp.Range()
                {
                    Max = 35,
                    Min = 30,
                },
                SpawnStrategy = slimeRainFields.Activities[0].Activity.Cast<SpawnActorActivity>().SpawnStrategy,
            };
            var activity = new WeatherStateDefinition.ActivityIntensityMapping()
            {
                Activity = spawnerActivity_null,
                Intensity = 1
            };
            slimeRainFields.Activities.Add(activity);

            var slimeRainValley = WeatherState("Slime Rain State Valley");
            spawnerActivity_data = new SpawnActorActivity()
            {
                name = "Data Slime Rain",
                ActorType = slimeDefinition_data,
                IntensityDivisor = 0.4f,
                _intensity = 0.3f,
                SecondsBetweenSpawns = new Il2Cpp.Range()
                {
                    Max = 25,
                    Min = 22,
                },
                SpawnStrategy = slimeRainValley.Activities[0].Activity.Cast<SpawnActorActivity>().SpawnStrategy,
            };
            var activity2 = new WeatherStateDefinition.ActivityIntensityMapping()
            {
                Activity = spawnerActivity_data,
                Intensity = 1
            };
            slimeRainValley.Activities.Add(activity2);
        }

    }
}