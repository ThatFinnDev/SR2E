using UnityEngine;
using Il2Cpp;
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
        private void LogPlortSold(int amount, IdentifiableType id)
        {
            if (id == plortDefinition)
            {
                SR2Console.SendMessage($"Callback test: {amount}");
            }
        }
        private void LogZoneEnter(ZoneDefinition zone)
        {
            SR2Console.SendMessage($"Callback test: {zone.name}");
        }
        public override void OnEarlyInitializeMelon()
        {
            Callbacks.onPlortSold += LogPlortSold;
            Callbacks.onZoneEnter += LogZoneEnter;
        }

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


        public static Color32 vacColor = new Color32(84, 84, 84, 255);
        public static Color32 topColor = new Color32(181, 181, 181, 255);
        public static Color32 middleColor = new Color32(125, 125, 125, 255);
        public static Color32 bottomColor = new Color32(59, 59, 59, 255);
        public static GameObject slimePrefab;
        public static GameObject plortPrefab;
        public static SlimeDefinition slimeDefinition;
        public static IdentifiableType plortDefinition;
        public static SpawnActorActivity spawnerActivity;

        

        public override void SaveDirectorLoading(AutoSaveDirector saveDirector)
        {
            slimeDefinition = CreateSlimeDef("Null", vacColor, null, Get<SlimeAppearance>("SaberDefault"), "NullDefault", "SlimeDefinition.NullSlime");
            plortDefinition = CreatePlortType("Null", vacColor, null, "IdentifiableType.NullPlort", 15f, 60f);
            plortPrefab = CreatePrefab("plortNull", Get<GameObject>("plortPhosphor"));
            slimePrefab = CreatePrefab("slimeNull", Get<GameObject>("slimeSaber"));
        }
        public override void SaveDirectorLoaded()
        {
            slimeDefinition.AppearancesDefault[0]._structures[0].DefaultMaterials[0] = Object.Instantiate(slimeDefinition.AppearancesDefault[0]._structures[0].DefaultMaterials[0]);
            plortDefinition.SetObjectPrefab(plortPrefab);
            slimeDefinition.SetObjectPrefab(slimePrefab);
            slimePrefab.SetObjectIdent(slimeDefinition);
            plortPrefab.SetObjectIdent(plortDefinition);
            slimeDefinition.AddProduceIdent(plortDefinition);

            Localization();
            Coloring();
            Images();
            Grouping();
            Eatmaps();
            SetupSpawning();
        }

        public static void Localization()
        {
            slimeDefinition.localizedName = AddTranslation("Null Slime", "l.nullSlime");
            slimeDefinition.localizationSuffix = "null_slime";
            plortDefinition.localizedName = AddTranslation("Null Plort", "l.nullPlort");
            plortDefinition.localizationSuffix = "null_plort";
        }

        public static void Coloring()
        {
            slimeDefinition.AppearancesDefault [0]._splatColor = bottomColor;
            slimeDefinition.AppearancesDefault [0].SetAppearanceVacColor(vacColor);
            slimeDefinition.SetSlimeColor(topColor, middleColor, bottomColor, middleColor, 0, 0, false, 0);
            SetPlortColor(topColor, middleColor, bottomColor, plortPrefab);
        }

        public static void Images()
        {
            slimeDefinition.AppearancesDefault[0]._structures[0].DefaultMaterials[0].SetTexture("_StripeTexture", LoadImage("body_stripes_null"));
            plortDefinition.icon = LoadImage("iconPlortNull").ConvertToSprite();
            slimeDefinition.icon = LoadImage("iconSlimeNull").ConvertToSprite();
            slimeDefinition.AppearancesDefault[0]._icon = LoadImage("iconSlimeNull").ConvertToSprite();
        }

        public static void Grouping()
        {

            plortDefinition.AddToGroup("PlortGroup");
            slimeDefinition.AddToGroup("SlimesGroup");
            slimeDefinition.AddToGroup("VaccableBaseSlimeGroup");
            slimeDefinition.AddToGroup("VaccableNonLiquids");
        }

        public static void Eatmaps()
        {

            slimeDefinition.Diet.MajorFoodIdentifiableTypeGroups.Add(Get<IdentifiableTypeGroup>("CraftGroup"));
            slimeDefinition.Diet.MajorFoodGroups = new Il2CppInterop.Runtime.InteropTypes.Arrays.Il2CppStructArray<SlimeEat.FoodGroup>(0);
            slimeDefinition.Diet.EatMap = new Il2CppSystem.Collections.Generic.List<SlimeDiet.EatMapEntry>(0);

            foreach (var item in Get<IdentifiableTypeGroup>("CraftGroup").GetAllMembersArray())
            {
                slimeDefinition.Diet.EatMap.Add(slimeDefinition.CreateEatmap(SlimeEmotions.Emotion.HUNGER, 0.1F, plortDefinition, item));
            }
        }

        public static void SetupSpawning()
        {
            var slimeRainFields = WeatherState("Slime Rain State Fields");
            spawnerActivity = new SpawnActorActivity()
            {
                name = "Null Slime Rain",
                ActorType = slimeDefinition,
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
                Activity = spawnerActivity,
                Intensity = 1
            };
            slimeRainFields.Activities.Add(activity);
        }

    }
}