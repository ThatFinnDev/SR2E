using System;
using Il2CppSystem.IO;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppInterop.Runtime.InteropTypes;
using System.Linq;
using System.Reflection;
using Il2CppMonomiPark.SlimeRancher.Damage;
using Il2CppMonomiPark.SlimeRancher.Persist;
using Il2CppMonomiPark.SlimeRancher.Script.Util;
using Il2CppMonomiPark.SlimeRancher.UI;
using Il2CppMonomiPark.SlimeRancher.UI.MainMenu;
using UnityEngine.Localization;
using UnityEngine.Localization.Tables;
using Il2CppMonomiPark.SlimeRancher.Weather;
using SR2E.Patches;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.Playables;

namespace SR2E.Library
{
    public static class LibraryUtils
    {
        public static WeatherStateDefinition[] weatherStateDefinitions => Resources.FindObjectsOfTypeAll<WeatherStateDefinition>();

        internal static List<SR2EMod> mods = new List<SR2EMod>();


        internal static Dictionary<IdentifiableType, ModdedMarketData> marketData = new Dictionary<IdentifiableType, ModdedMarketData>(0);

        internal static Dictionary<MarketUI.PlortEntry, bool> marketPlortEntries = new Dictionary<MarketUI.PlortEntry, bool>();
        internal static List<IdentifiableType> removeMarketPlortEntries = new List<IdentifiableType>();
        internal static GameObject rootOBJ;
        internal static Dictionary<SlimeDefinition, LargoSettings>? LargoData = new Dictionary<SlimeDefinition, LargoSettings>(0);

        public static IdentifiableTypeGroup? slimes;
        public static IdentifiableTypeGroup? plorts;
        public static IdentifiableTypeGroup? largos;
        public static IdentifiableTypeGroup? baseSlimes;
        public static IdentifiableTypeGroup? food;
        public static IdentifiableTypeGroup? meat;
        public static IdentifiableTypeGroup? veggies;
        public static IdentifiableTypeGroup? fruits;
        public static IdentifiableTypeGroup? crafts;
        public static GameObject? player;

        // public enum VanillaPediaEntryCategories { TUTORIAL, SLIMES, RESOURCES, WORLD, RANCH, SCIENCE, WEATHER }
        public static SystemContext systemContext => SystemContext.Instance;
        public static GameContext gameContext => GameContext.Instance;
        public static SceneContext sceneContext => SceneContext.Instance;

        public static SlimeDefinition CreateSlimeDef(string Name, Color32 VacColor, Sprite Icon, SlimeAppearance baseAppearance, string appearanceName, string RefID)
        {
            SlimeDefinition slimedef = ScriptableObject.CreateInstance<SlimeDefinition>();
            Object.DontDestroyOnLoad(slimedef);
            slimedef.hideFlags = HideFlags.HideAndDontSave;
            slimedef.name = Name;
            slimedef.AppearancesDefault = new Il2CppReferenceArray<SlimeAppearance>(1);

            SlimeAppearance appearance = Object.Instantiate(baseAppearance);
            Object.DontDestroyOnLoad(appearance);
            appearance.name = appearanceName;
            slimedef.AppearancesDefault = slimedef.AppearancesDefault.Add(appearance);
            if (slimedef.AppearancesDefault[0] == null)
            {
                slimedef.AppearancesDefault[0] = appearance;
            }
            for (int i = 0; i < slimedef.AppearancesDefault[0].Structures.Count - 1; i++)
            {
                SlimeAppearanceStructure a = slimedef.AppearancesDefault[0].Structures[i];
                var a2 = new SlimeAppearanceStructure(a);
                slimedef.AppearancesDefault[0].Structures[i] = a2;
                if (a.DefaultMaterials.Count != 0)
                {
                    for (int l = 0; l < a2.DefaultMaterials.Count - 1; l++)
                    {
                        Material b = a.DefaultMaterials[l];
                        a.DefaultMaterials[l] = Object.Instantiate(b);
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
            slimedef.properties = Get<SlimeDefinition>("Pink").properties;
            return slimedef;
        }



        public static Il2CppReferenceArray<SlimeAppearanceStructure> MergeStructures(SlimeAppearance slime1, SlimeAppearance slime2, LargoSettings settings)
        {
            var newStructures = new List<SlimeAppearanceStructure>(0);
            SlimeAppearance.Palette firstColor = slime1._colorPalette;
            SlimeAppearance.Palette secondColor = slime2._colorPalette;
            foreach (var structure in slime1.Structures)
            {
                if (structure.Element.Type == SlimeAppearanceElement.ElementType.FACE || structure.Element.Type == SlimeAppearanceElement.ElementType.FACE_ATTACH)
                {
                    if ((settings & LargoSettings.KeepFirstFace) != 0)
                    {
                        if (structure != null && !newStructures.Contains(structure) && structure.DefaultMaterials.Length != 0)
                        {
                            var newStructure = new SlimeAppearanceStructure(structure);
                            newStructures.Add(newStructure);
                            var mat = Object.Instantiate(structure.DefaultMaterials[0]);
                            newStructure.DefaultMaterials[0] = mat;

                            try
                            {
                                if ((settings & LargoSettings.KeepFirstColor) != 0)
                                {
                                    mat.SetColor("_TopColor", firstColor.Top);
                                    mat.SetColor("_MiddleColor", firstColor.Middle);
                                    mat.SetColor("_BottomColor", firstColor.Bottom);
                                    mat.SetColor("_SpecColor", firstColor.Middle);
                                }
                                else if ((settings & LargoSettings.KeepSecondColor) != 0)
                                {
                                    mat.SetColor("_TopColor", secondColor.Top);
                                    mat.SetColor("_MiddleColor", secondColor.Middle);
                                    mat.SetColor("_BottomColor", secondColor.Bottom);
                                    mat.SetColor("_SpecColor", secondColor.Middle);
                                }
                                else if ((settings & LargoSettings.MergeColors) != 0)
                                {
                                    var top = Color.Lerp(firstColor.Top, secondColor.Top, 0.5f);
                                    var middle = Color.Lerp(firstColor.Middle, secondColor.Middle, 0.5f);
                                    var bottom = Color.Lerp(firstColor.Bottom, secondColor.Bottom, 0.5f);
                                    mat.SetColor("_TopColor", top);
                                    mat.SetColor("_MiddleColor", middle);
                                    mat.SetColor("_BottomColor", bottom);
                                    mat.SetColor("_SpecColor", middle);
                                }
                            }
                            catch { }
                        }
                    }
                }
                else if (structure.Element.Type == SlimeAppearanceElement.ElementType.BODY)
                {
                    if ((settings & LargoSettings.KeepFirstBody) != 0)
                    {
                        if (structure != null && !newStructures.Contains(structure) && structure.DefaultMaterials.Length != 0)
                        {
                            var newStructure = new SlimeAppearanceStructure(structure);
                            newStructures.Add(newStructure);
                            var mat = Object.Instantiate(structure.DefaultMaterials[0]);
                            newStructure.DefaultMaterials[0] = mat;

                            try
                            {
                                if ((settings & LargoSettings.KeepFirstColor) != 0)
                                {
                                    mat.SetColor("_TopColor", firstColor.Top);
                                    mat.SetColor("_MiddleColor", firstColor.Middle);
                                    mat.SetColor("_BottomColor", firstColor.Bottom);
                                    mat.SetColor("_SpecColor", firstColor.Middle);
                                }
                                else if ((settings & LargoSettings.KeepSecondColor) != 0)
                                {
                                    mat.SetColor("_TopColor", secondColor.Top);
                                    mat.SetColor("_MiddleColor", secondColor.Middle);
                                    mat.SetColor("_BottomColor", secondColor.Bottom);
                                    mat.SetColor("_SpecColor", secondColor.Middle);
                                }
                                else if ((settings & LargoSettings.MergeColors) != 0)
                                {
                                    var top = Color.Lerp(firstColor.Top, secondColor.Top, 0.5f);
                                    var middle = Color.Lerp(firstColor.Middle, secondColor.Middle, 0.5f);
                                    var bottom = Color.Lerp(firstColor.Bottom, secondColor.Bottom, 0.5f);
                                    mat.SetColor("_TopColor", top);
                                    mat.SetColor("_MiddleColor", middle);
                                    mat.SetColor("_BottomColor", bottom);
                                    mat.SetColor("_SpecColor", middle);
                                }
                            }
                            catch { }
                        }
                    }
                }
                else if (structure != null && !newStructures.Contains(structure) && structure.DefaultMaterials.Length != 0)
                {
                    var newStructure = new SlimeAppearanceStructure(structure);
                    newStructures.Add(newStructure);
                    var mat = Object.Instantiate(structure.DefaultMaterials[0]);
                    structure.DefaultMaterials[0] = mat;
                    try
                    {
                        if ((settings & LargoSettings.KeepFirstColor) != 0)
                        {
                            mat.SetColor("_TopColor", firstColor.Top);
                            mat.SetColor("_MiddleColor", firstColor.Middle);
                            mat.SetColor("_BottomColor", firstColor.Bottom);
                            mat.SetColor("_SpecColor", firstColor.Middle);
                        }
                        else if ((settings & LargoSettings.KeepSecondColor) != 0)
                        {
                            mat.SetColor("_TopColor", secondColor.Top);
                            mat.SetColor("_MiddleColor", secondColor.Middle);
                            mat.SetColor("_BottomColor", secondColor.Bottom);
                            mat.SetColor("_SpecColor", secondColor.Middle);
                        }
                        else if ((settings & LargoSettings.MergeColors) != 0)
                        {
                            var top = Color.Lerp(firstColor.Top, secondColor.Top, 0.5f);
                            var middle = Color.Lerp(firstColor.Middle, secondColor.Middle, 0.5f);
                            var bottom = Color.Lerp(firstColor.Bottom, secondColor.Bottom, 0.5f);
                            mat.SetColor("_TopColor", top);
                            mat.SetColor("_MiddleColor", middle);
                            mat.SetColor("_BottomColor", bottom);
                            mat.SetColor("_SpecColor", middle);
                        }
                    }
                    catch { }
                }
            }
            foreach (var structure in slime2.Structures)
            {
                if (structure.Element.Type == SlimeAppearanceElement.ElementType.FACE || structure.Element.Type == SlimeAppearanceElement.ElementType.FACE_ATTACH)
                {
                    if ((settings & LargoSettings.KeepSecondFace) != 0)
                    {
                        if (structure != null && !newStructures.Contains(structure) && structure.DefaultMaterials.Length != 0)
                        {
                            var newStructure = new SlimeAppearanceStructure(structure);
                            newStructures.Add(newStructure);
                            var mat = Object.Instantiate(structure.DefaultMaterials[0]);
                            newStructure.DefaultMaterials[0] = mat;

                            try
                            {
                                if ((settings & LargoSettings.KeepFirstColor) != 0)
                                {
                                    mat.SetColor("_TopColor", firstColor.Top);
                                    mat.SetColor("_MiddleColor", firstColor.Middle);
                                    mat.SetColor("_BottomColor", firstColor.Bottom);
                                    mat.SetColor("_SpecColor", firstColor.Middle);
                                }
                                else if ((settings & LargoSettings.KeepSecondColor) != 0)
                                {
                                    mat.SetColor("_TopColor", secondColor.Top);
                                    mat.SetColor("_MiddleColor", secondColor.Middle);
                                    mat.SetColor("_BottomColor", secondColor.Bottom);
                                    mat.SetColor("_SpecColor", secondColor.Middle);
                                }
                                else if ((settings & LargoSettings.MergeColors) != 0)
                                {
                                    var top = Color.Lerp(firstColor.Top, secondColor.Top, 0.5f);
                                    var middle = Color.Lerp(firstColor.Middle, secondColor.Middle, 0.5f);
                                    var bottom = Color.Lerp(firstColor.Bottom, secondColor.Bottom, 0.5f);
                                    mat.SetColor("_TopColor", top);
                                    mat.SetColor("_MiddleColor", middle);
                                    mat.SetColor("_BottomColor", bottom);
                                    mat.SetColor("_SpecColor", middle);
                                }
                            }
                            catch { }
                        }
                    }
                }
                else if (structure.Element.Type == SlimeAppearanceElement.ElementType.BODY)
                {
                    if ((settings & LargoSettings.KeepSecondBody) != 0)
                    {
                        if (!newStructures.Contains(structure))
                        {
                            var newStructure = new SlimeAppearanceStructure(structure);
                            newStructures.Add(newStructure);
                            var mat = Object.Instantiate(structure.DefaultMaterials[0]);
                            newStructure.DefaultMaterials[0] = mat;

                            try
                            {
                                if ((settings & LargoSettings.KeepFirstColor) != 0)
                                {
                                    mat.SetColor("_TopColor", firstColor.Top);
                                    mat.SetColor("_MiddleColor", firstColor.Middle);
                                    mat.SetColor("_BottomColor", firstColor.Bottom);
                                    mat.SetColor("_SpecColor", firstColor.Middle);
                                }
                                else if ((settings & LargoSettings.KeepSecondColor) != 0)
                                {
                                    mat.SetColor("_TopColor", secondColor.Top);
                                    mat.SetColor("_MiddleColor", secondColor.Middle);
                                    mat.SetColor("_BottomColor", secondColor.Bottom);
                                    mat.SetColor("_SpecColor", secondColor.Middle);
                                }
                                else if ((settings & LargoSettings.MergeColors) != 0)
                                {
                                    var top = Color.Lerp(firstColor.Top, secondColor.Top, 0.5f);
                                    var middle = Color.Lerp(firstColor.Middle, secondColor.Middle, 0.5f);
                                    var bottom = Color.Lerp(firstColor.Bottom, secondColor.Bottom, 0.5f);
                                    mat.SetColor("_TopColor", top);
                                    mat.SetColor("_MiddleColor", middle);
                                    mat.SetColor("_BottomColor", bottom);
                                    mat.SetColor("_SpecColor", middle);
                                }
                            }
                            catch { }
                        }

                    }
                }
                else if (structure != null && !newStructures.Contains(structure) && structure.DefaultMaterials.Length != 0)
                {

                    var newStructure = new SlimeAppearanceStructure(structure);
                    newStructures.Add(newStructure);
                    var mat = Object.Instantiate(structure.DefaultMaterials[0]);
                    newStructure.DefaultMaterials[0] = mat;
                    try
                    {
                        if ((settings & LargoSettings.KeepFirstColor) != 0)
                        {
                            mat.SetColor("_TopColor", firstColor.Top);
                            mat.SetColor("_MiddleColor", firstColor.Middle);
                            mat.SetColor("_BottomColor", firstColor.Bottom);
                            mat.SetColor("_SpecColor", firstColor.Middle);
                        }
                        else if ((settings & LargoSettings.KeepSecondColor) != 0)
                        {
                            mat.SetColor("_TopColor", secondColor.Top);
                            mat.SetColor("_MiddleColor", secondColor.Middle);
                            mat.SetColor("_BottomColor", secondColor.Bottom);
                            mat.SetColor("_SpecColor", secondColor.Middle);
                        }
                        else if ((settings & LargoSettings.MergeColors) != 0)
                        {
                            var top = Color.Lerp(firstColor.Top, secondColor.Top, 0.5f);
                            var middle = Color.Lerp(firstColor.Middle, secondColor.Middle, 0.5f);
                            var bottom = Color.Lerp(firstColor.Bottom, secondColor.Bottom, 0.5f);
                            mat.SetColor("_TopColor", top);
                            mat.SetColor("_MiddleColor", middle);
                            mat.SetColor("_BottomColor", bottom);
                            mat.SetColor("_SpecColor", middle);
                        }
                    }
                    catch { }
                }


            }
            return new Il2CppReferenceArray<SlimeAppearanceStructure>(newStructures.ToArray());
        }

        public static SlimeDefinition CreateCompleteLargo(SlimeDefinition slimeOne, SlimeDefinition slimeTwo, LargoSettings settings)
        {
            SlimeDefinition pinkRock = Get<SlimeDefinition>("PinkRock");
            if (slimeOne.IsLargo || slimeTwo.IsLargo)
                return null;
            slimeOne.CanLargofy = true;
            slimeTwo.CanLargofy = true;

            SlimeDefinition slimedef = Object.Instantiate(pinkRock);
            slimedef.BaseSlimes = new[] { slimeOne, slimeTwo };
            slimedef.SlimeModules = new[] { Get<GameObject>("moduleSlime" + slimeOne.name), Get<GameObject>("moduleSlime" + slimeTwo.name) };


            slimedef.localizationSuffix = slimeOne.name.ToLower() + "_" + slimeTwo.name.ToLower() + "_largo";
            slimedef.referenceId = "SlimeDefinition." + slimeOne.name + slimeTwo.name;
            slimedef.localizedName = AddTranslation(slimeOne.name + " " + slimeTwo.name + " Largo", "l." + slimedef.localizationSuffix);

            slimedef.FavoriteToyIdents = new Il2CppReferenceArray<ToyDefinition>(0);

            Object.DontDestroyOnLoad(slimedef);
            slimedef.hideFlags = HideFlags.HideAndDontSave;
            slimedef.name = slimeOne.name + slimeTwo.name; ;
            slimedef.Name = slimeOne.name + " " + slimeTwo.name; ;

            slimedef.prefab = Object.Instantiate(pinkRock.prefab, rootOBJ.transform);
            slimedef.prefab.name = $"slime{slimeOne.name + slimeTwo.name}";
            slimedef.prefab.GetComponent<Identifiable>().identType = slimedef;
            slimedef.prefab.GetComponent<SlimeEat>().SlimeDefinition = slimedef;
            slimedef.prefab.GetComponent<SlimeAppearanceApplicator>().SlimeDefinition = slimedef;
            slimedef.prefab.GetComponent<PlayWithToys>().SlimeDefinition = slimedef;
            slimedef.prefab.GetComponent<ReactToToyNearby>().SlimeDefinition = slimedef;
            slimedef.prefab.GetComponent<SlimeVarietyModules>().BaseModule = slimedef.BaseModule;
            slimedef.prefab.GetComponent<SlimeVarietyModules>().SlimeModules = slimedef.SlimeModules;
            slimedef.prefab.RemoveComponent<RockSlimeRoll>();
            slimedef.prefab.RemoveComponent<DamagePlayerOnTouch>();

            SlimeAppearance appearance = Object.Instantiate(pinkRock.AppearancesDefault[0]);
            slimedef.AppearancesDefault[0] = appearance;
            Object.DontDestroyOnLoad(appearance);
            appearance.name = slimeOne.AppearancesDefault[0].name + slimeTwo.AppearancesDefault[0].name;

            appearance._dependentAppearances = new[] { slimeOne.AppearancesDefault[0], slimeTwo.AppearancesDefault[0] };
            appearance._structures = MergeStructures(appearance._dependentAppearances[0], appearance._dependentAppearances[1], settings);
            slimedef.Diet = MergeDiet(slimeOne.Diet, slimeTwo.Diet);
            SlimeDefinition tarr = Get<SlimeDefinition>("Tarr");/*
            slimeOne.Diet.EatMap.Add(CreateEatmap(SlimeEmotions.Emotion.AGITATION, 0.5f, null, 
               slimeTwo.Diet.ProduceIdents[0],slimedef));
            slimeTwo.Diet.EatMap.Add(CreateEatmap(SlimeEmotions.Emotion.AGITATION, 0.5f, null, 
                slimeOne.Diet.ProduceIdents[0],slimedef));
            foreach (SlimeDiet.EatMapEntry entry in slimedef.Diet.EatMap)
                if (entry.EatsIdent.IsPlort)
                    if (entry.EatsIdent.ValidatableName == slimeOne.Diet.ProduceIdents[0].ValidatableName || entry.EatsIdent.ValidatableName == slimeTwo.Diet.ProduceIdents[0].ValidatableName)
                        slimedef.Diet.EatMap.Remove(entry);
            foreach (SlimeDiet.EatMapEntry entry in slimedef.Diet.EatMap)
                entry.BecomesIdent = tarr;
            
            slimedef.SetProduceIdent(slimeOne.Diet.ProduceIdents[0],0);
            slimedef.SetProduceIdent(slimeTwo.Diet.ProduceIdents[0],1);*/
            slimedef.RefreshEatmap();

            slimeDefinitions.Slimes.Add(slimedef);
            slimeDefinitions._slimeDefinitionsByIdentifiable.TryAdd(slimedef, slimedef);
            slimeDefinitions._largoDefinitionByBaseDefinitions.TryAdd(new SlimeDefinitions.SlimeDefinitionPair() { SlimeDefinition1 = slimeOne, SlimeDefinition2 = slimeTwo }, slimedef);
            mainAppearanceDirector.RegisterDependentAppearances(slimedef, slimedef.AppearancesDefault[0]);
            mainAppearanceDirector.UpdateChosenSlimeAppearance(slimedef, slimedef.AppearancesDefault[0]);

            AddToGroup(slimedef, "LargoGroup");
            AddToGroup(slimedef, "SlimesGroup");
            INTERNAL_SetupSaveForIdent(slimedef.referenceId, slimedef);
            return slimedef;
        }

        public static WeatherStateDefinition getWeatherStateByName(string name)
        {
            foreach (WeatherStateDefinition state in weatherStateDefinitions)
                try
                {
                    if (state.name.ToUpper().Replace(" ", "") == name.ToUpper())
                        return state;
                }
                catch (System.Exception ignored)
                { }
            return null;
        }

        public static Il2CppArrayBase<WeatherStateDefinition> weatherStates => GameContext.Instance.AutoSaveDirector.weatherStates.items.ToArray();
        public static WeatherStateDefinition WeatherState(string name) => weatherStates.FirstOrDefault((WeatherStateDefinition x) => x.name == name);

        public static void AddStructure(this SlimeAppearance appearance, SlimeAppearanceStructure structure)
        {
            appearance.Structures.Add(structure);
        }
        public static SlimeDiet MergeDiet(this SlimeDiet firstDiet, SlimeDiet secondDiet)
        {
            var mergedDiet = INTERNAL_CreateNewDiet();

            mergedDiet.EatMap.AddListRangeNoMultiple(firstDiet.EatMap);
            mergedDiet.EatMap.AddListRangeNoMultiple(secondDiet.EatMap);

            mergedDiet.AdditionalFoodIdents = mergedDiet.AdditionalFoodIdents.AddRangeNoMultiple(firstDiet.AdditionalFoodIdents);
            mergedDiet.AdditionalFoodIdents = mergedDiet.AdditionalFoodIdents.AddRangeNoMultiple(secondDiet.AdditionalFoodIdents);

            mergedDiet.FavoriteIdents = mergedDiet.FavoriteIdents.AddRangeNoMultiple(firstDiet.FavoriteIdents);
            mergedDiet.FavoriteIdents = mergedDiet.FavoriteIdents.AddRangeNoMultiple(secondDiet.FavoriteIdents);

            List<SlimeEat.FoodGroup> foodGroups = new List<SlimeEat.FoodGroup>();
            foreach (SlimeEat.FoodGroup foodGroup in firstDiet.MajorFoodGroups)
                if (!foodGroups.Contains(foodGroup))
                    foodGroups.Add(foodGroup);
            foreach (SlimeEat.FoodGroup foodGroup in secondDiet.MajorFoodGroups)
                if (!foodGroups.Contains(foodGroup))
                    foodGroups.Add(foodGroup); ;
            mergedDiet.MajorFoodGroups = new Il2CppStructArray<SlimeEat.FoodGroup>(foodGroups.ToArray());

            mergedDiet.MajorFoodIdentifiableTypeGroups = mergedDiet.MajorFoodIdentifiableTypeGroups.AddRangeNoMultiple(firstDiet.MajorFoodIdentifiableTypeGroups);
            mergedDiet.MajorFoodIdentifiableTypeGroups = mergedDiet.MajorFoodIdentifiableTypeGroups.AddRangeNoMultiple(secondDiet.MajorFoodIdentifiableTypeGroups);

            mergedDiet.ProduceIdents = mergedDiet.ProduceIdents.AddRangeNoMultiple(firstDiet.ProduceIdents);
            mergedDiet.ProduceIdents = mergedDiet.ProduceIdents.AddRangeNoMultiple(secondDiet.ProduceIdents);

            return mergedDiet;
        }
        public static void SwitchSlimeAppearances(this SlimeDefinition slimeOneDef, SlimeDefinition slimeTwoDef)
        {
            var appearanceOne = slimeOneDef.AppearancesDefault[0]._structures; slimeOneDef.AppearancesDefault[0]._structures = slimeTwoDef.AppearancesDefault[0]._structures; slimeTwoDef.AppearancesDefault[0]._structures = appearanceOne;
            var appearanceSplatOne = slimeOneDef.AppearancesDefault[0]._splatColor; slimeOneDef.AppearancesDefault[0]._splatColor = slimeTwoDef.AppearancesDefault[0]._splatColor; slimeTwoDef.AppearancesDefault[0]._splatColor = appearanceSplatOne;

            var colorPalate = slimeOneDef.AppearancesDefault[0]._colorPalette; slimeOneDef.AppearancesDefault[0]._colorPalette = slimeTwoDef.AppearancesDefault[0]._colorPalette; slimeTwoDef.AppearancesDefault[0]._colorPalette = colorPalate;

            var structureIcon = slimeOneDef.AppearancesDefault[0]._icon; slimeOneDef.AppearancesDefault[0]._icon = slimeTwoDef.AppearancesDefault[0]._icon; slimeTwoDef.AppearancesDefault[0]._icon = structureIcon;
            var icon = slimeOneDef.icon; slimeOneDef.icon = slimeTwoDef.icon; slimeTwoDef.icon = icon;

            var debugIcon = slimeOneDef.debugIcon; slimeOneDef.debugIcon = slimeTwoDef.debugIcon; slimeTwoDef.debugIcon = debugIcon;

        }

        public enum LargoSettings { KeepFirstBody, KeepSecondBody, KeepFirstFace, KeepSecondFace, KeepFirstColor, KeepSecondColor, MergeColors }
        public static SlimeDefinitions? slimeDefinitions { get { return gameContext.SlimeDefinitions; } /*set { gameContext.SlimeDefinitions = value; }*/ }
        private static SlimeAppearanceDirector _mainAppearanceDirector;
        public static SlimeAppearanceDirector mainAppearanceDirector
        {
            get
            {
                if (_mainAppearanceDirector == null) _mainAppearanceDirector = Get<SlimeAppearanceDirector>("MainSlimeAppearanceDirector");
                return _mainAppearanceDirector;
            }
            set { _mainAppearanceDirector = value; }
        }



        public static Dictionary<string, Dictionary<string, string>> addedTranslations = new Dictionary<string, System.Collections.Generic.Dictionary<string, string>>();

        public static LocalizedString AddTranslation(string localized, string key = "l.SR2ELibraryTest", string table = "Actor")
        {
            StringTable table2 = LocalizationUtil.GetTable(table);

            System.Collections.Generic.Dictionary<string, string> dictionary;
            if (!addedTranslations.TryGetValue(table, out dictionary))
            {
                dictionary = new System.Collections.Generic.Dictionary<string, string>();

                addedTranslations.Add(table, dictionary);
            }

            string? key0 = null;

            if (key == "l.SR2E.LibraryTest")
            {
                key0 = $"{key}.{UnityEngine.Random.RandomRange(10000, 99999)}.{UnityEngine.Random.RandomRange(10, 99)}";
                while (table2.GetEntry(key0) != null)
                {
                    key0 = $"{key}.{UnityEngine.Random.RandomRange(10000, 99999)}.{UnityEngine.Random.RandomRange(10, 99)}";
                }

            }
            else
                key0 = key;

            dictionary.Add(key0, localized);
            StringTableEntry stringTableEntry = table2.AddEntry(key, localized);
            return new LocalizedString(table2.SharedData.TableCollectionName, stringTableEntry.SharedEntry.Id);
        }
        

        public static IdentifiableType GetIdentifiableType(this GameObject obj)
        {
            var comp = obj.GetComponent<IdentifiableActor>();

            if (comp != null)
            {
                return comp.identType;
            }
            return null;
        }
        public static SlimeDiet.EatMapEntry CreateEatmap(SlimeEmotions.Emotion driver, float mindrive, IdentifiableType produce, IdentifiableType eat, IdentifiableType becomes)
        {
            var eatmap = new SlimeDiet.EatMapEntry
            {
                EatsIdent = eat,
                ProducesIdent = produce,
                BecomesIdent = becomes,
                Driver = driver,
                MinDrive = mindrive
            };
            return eatmap;
        }
        public static SlimeDiet.EatMapEntry CreateEatmap(SlimeEmotions.Emotion driver, float mindrive, IdentifiableType produce, IdentifiableType eat)
        {
            var eatmap = new SlimeDiet.EatMapEntry
            {
                EatsIdent = eat,
                ProducesIdent = produce,
                Driver = driver,
                MinDrive = mindrive
            };
            return eatmap;
        }
        public static void ModifyEatmap(this SlimeDiet.EatMapEntry eatmap, SlimeEmotions.Emotion driver, float mindrive, IdentifiableType produce, IdentifiableType eat, IdentifiableType becomes)
        {
            eatmap.EatsIdent = eat;
            eatmap.BecomesIdent = becomes;
            eatmap.ProducesIdent = produce;
            eatmap.Driver = driver;
            eatmap.MinDrive = mindrive;
        }
        public static void ModifyEatmap(this SlimeDiet.EatMapEntry eatmap, SlimeEmotions.Emotion driver, float mindrive, IdentifiableType produce, IdentifiableType eat)
        {
            eatmap.EatsIdent = eat;
            eatmap.ProducesIdent = produce;
            eatmap.Driver = driver;
            eatmap.MinDrive = mindrive;
        }
        public static void AddProduceIdent(this SlimeDefinition slimedef, IdentifiableType ident)
        {
            slimedef.Diet.ProduceIdents = slimedef.Diet.ProduceIdents.Add(ident);
        }
        public static void SetProduceIdent(this SlimeDefinition slimedef, IdentifiableType ident, int index)
        {
            slimedef.Diet.ProduceIdents[index] = ident;
        }
        public static void AddExtraEatIdent(this SlimeDefinition slimedef, IdentifiableType ident)
        {
            slimedef.Diet.AdditionalFoodIdents = slimedef.Diet.AdditionalFoodIdents.Add(ident);

        }
        public static void SetFavoriteProduceCount(this SlimeDefinition slimedef, int count)
        {
            slimedef.Diet.FavoriteProductionCount = count;
        }
        public static void AddFavorite(this SlimeDefinition slimedef, IdentifiableType id)
        {
            slimedef.Diet.FavoriteIdents = slimedef.Diet.FavoriteIdents.Add(id);
        }
        public static void AddEatmapToSlime(this SlimeDefinition slimedef, SlimeDiet.EatMapEntry eatmap)
        {
            slimedef.Diet.EatMap.Add(eatmap);
        }
        public static void SetStructColor(this SlimeAppearanceStructure structure, int id, Color color)
        {
            structure.DefaultMaterials[0].SetColor(id, color);
        }
        public static void RefreshEatmap(this SlimeDefinition def)
        {
            def.Diet.RefreshEatMap(slimeDefinitions, def);
        }
        public static void ChangeSlimeFoodGroup(this SlimeDefinition def, SlimeEat.FoodGroup FG, int index)
        {
            def.Diet.MajorFoodGroups[index] = FG;
        }
        public static void AddSlimeFoodGroup(this SlimeDefinition def, SlimeEat.FoodGroup FG)
        {
            var list = new List<SlimeEat.FoodGroup>();
            foreach (var item in def.Diet.MajorFoodGroups)
                list.Add(item);

            def.Diet.MajorFoodGroups = new Il2CppStructArray<SlimeEat.FoodGroup>(list.ToArray());
        }
        public static void ChangeSlimeFoodIdentGroup(this SlimeDefinition def, IdentifiableTypeGroup FG, int index)
        {
            def.Diet.MajorFoodIdentifiableTypeGroups[index] = FG;
        }
        public static void AddSlimeFoodIdentGroup(this SlimeDefinition def, IdentifiableTypeGroup FG)
        {
            def.Diet.MajorFoodIdentifiableTypeGroups = def.Diet.MajorFoodIdentifiableTypeGroups.Add(FG);
            def.Diet.MajorFoodIdentifiableTypeGroups = def.Diet.MajorFoodIdentifiableTypeGroups.Add(FG);
        }
        public static GameObject SpawnActor(this GameObject obj, Vector3 pos)
        {
            return SRBehaviour.InstantiateActor(obj,
                SRSingleton<SceneContext>.Instance.RegionRegistry.CurrentSceneGroup, pos, Quaternion.identity,
                false, SlimeAppearance.AppearanceSaveSet.NONE, SlimeAppearance.AppearanceSaveSet.NONE);
        }
        public static GameObject SpawnActor(this GameObject obj, Vector3 pos, Vector3 rot)
        {
            return SRBehaviour.InstantiateActor(obj,
                SRSingleton<SceneContext>.Instance.RegionRegistry.CurrentSceneGroup, pos, Quaternion.Euler(rot),
                false, SlimeAppearance.AppearanceSaveSet.NONE, SlimeAppearance.AppearanceSaveSet.NONE);
        }
        public static GameObject SpawnActor(this GameObject obj, Vector3 pos, Quaternion rot)
        {
            return SRBehaviour.InstantiateActor(obj,
                SRSingleton<SceneContext>.Instance.RegionRegistry.CurrentSceneGroup, pos, rot,
                false, SlimeAppearance.AppearanceSaveSet.NONE, SlimeAppearance.AppearanceSaveSet.NONE);
        }
        public static GameObject SpawnDynamic(this GameObject obj, Vector3 pos)
        {
            return SRBehaviour.InstantiateDynamic(obj, pos, Quaternion.identity, false);
        }
        public static T? Get<T>(string name) where T : Object { return Resources.FindObjectsOfTypeAll<T>().FirstOrDefault((T x) => x.name == name); }
        public static void AddToGroup(this IdentifiableType type, string groupName)
        {
            var group = Get<IdentifiableTypeGroup>(groupName);
            group.memberTypes.Add(type);
        }


        public static IdentifiableTypeGroup MakeNewGroup(IdentifiableType[] types, string groupName, IdentifiableTypeGroup[] subGroups = null)
        {
            var group = new IdentifiableTypeGroup();
            var typesList = new Il2CppSystem.Collections.Generic.List<IdentifiableType>();
            foreach (var type in types)
            {
                try
                {
                    typesList.Add(type);
                }
                catch
                {
                }
            }

            var subGroupsList = new Il2CppSystem.Collections.Generic.List<IdentifiableTypeGroup>();
            foreach (var subGroup in subGroups)
            {
                try
                {
                    subGroupsList.Add(subGroup);
                }
                catch
                {
                }
            }

            group.memberTypes = typesList;
            group.memberGroups = subGroupsList;
            return group;
        }


        internal static SlimeDiet INTERNAL_CreateNewDiet()
        {
            var diet = new SlimeDiet();

            diet.ProduceIdents = new Il2CppInterop.Runtime.InteropTypes.Arrays.Il2CppReferenceArray<IdentifiableType>(0);
            diet.FavoriteProductionCount = 2;
            diet.MajorFoodGroups = new Il2CppInterop.Runtime.InteropTypes.Arrays.Il2CppStructArray<SlimeEat.FoodGroup>(0);
            diet.EatMap = new Il2CppSystem.Collections.Generic.List<SlimeDiet.EatMapEntry>(0);
            diet.FavoriteIdents = new Il2CppInterop.Runtime.InteropTypes.Arrays.Il2CppReferenceArray<IdentifiableType>(0);
            diet.AdditionalFoodIdents = new Il2CppInterop.Runtime.InteropTypes.Arrays.Il2CppReferenceArray<IdentifiableType>(0);
            diet.MajorFoodIdentifiableTypeGroups = new Il2CppInterop.Runtime.InteropTypes.Arrays.Il2CppReferenceArray<IdentifiableTypeGroup>(0);
            diet.BecomesOnTarrifyIdentifiableType = Get<IdentifiableType>("Tarr");
            diet.EdiblePlortIdentifiableTypeGroup = Get<IdentifiableTypeGroup>("EdiblePlortFoodGroup");
            return diet;
        }

        public static GameObject CreatePrefab(string Name, GameObject baseObject)
        {
            var obj = CopyObject(baseObject);
            UnityEngine.Object.DontDestroyOnLoad(obj);
            obj.name = Name;
            obj.transform.parent = rootOBJ.transform;
            var components = obj.GetComponents<Behaviour>();
            foreach (var component in components)
            {
                component.enabled = true;
            }
            return obj;
        }

        public static void SetObjectPrefab(this IdentifiableType Object, GameObject prefab)
        {
            Object.prefab = prefab;
        }
        public static void SetObjectIdent(this GameObject prefab, IdentifiableType Object)
        {
            if (Object is SlimeDefinition)
            {
                prefab.GetComponent<SlimeEat>().SlimeDefinition = (SlimeDefinition)Object;
                prefab.GetComponent<SlimeAppearanceApplicator>().SlimeDefinition = (SlimeDefinition)Object;
            }

            prefab.GetComponent<IdentifiableActor>().identType = Object;
        }

        public static IdentifiableType CreatePlortType(string Name, Color32 VacColor, Sprite Icon, string RefID, float marketValue, float marketSaturation)
        {
            var plort = ScriptableObject.CreateInstance<IdentifiableType>();
            Object.DontDestroyOnLoad(plort);
            plort.hideFlags = HideFlags.HideAndDontSave;
            plort.name = Name + "Plort";
            plort.color = VacColor;
            plort.icon = Icon;
            plort.IsPlort = true;
            MakeSellable(plort, marketValue, marketSaturation);
            AddToGroup(plort, "VaccableBaseSlimeGroup");
            INTERNAL_SetupSaveForIdent(RefID, plort);
            return plort;
        }
        internal static void INTERNAL_SetupSaveForIdent(string RefID, IdentifiableType ident)
        {
            ident.referenceId = RefID;
            //GameContext.Instance.AutoSaveDirector.SavedGame.identifiableTypeLookup.Add(RefID, ident);
            GameContext.Instance.AutoSaveDirector.identifiableTypes.memberTypes.Add(ident);
            //GameContext.Instance.AutoSaveDirector.SavedGame.identifiableTypeToPersistenceId._primaryIndex.AddString(RefID);
            //GameContext.Instance.AutoSaveDirector.SavedGame.identifiableTypeToPersistenceId._reverseIndex.Add(RefID, GameContext.Instance.AutoSaveDirector.SavedGame.identifiableTypeToPersistenceId._reverseIndex.Count);
            if (ident is SlimeDefinition)
            {
                SRSingleton<GameContext>.Instance.SlimeDefinitions.Slimes.Add(ident.Cast<SlimeDefinition>());
                SRSingleton<GameContext>.Instance.SlimeDefinitions._slimeDefinitionsByIdentifiable.TryAdd(ident, ident.Cast<SlimeDefinition>());
            }
        }
        public static void SetPlortColor(Color32 Top, Color32 Middle, Color32 Bottom, GameObject Prefab)
        {
            var material = Prefab.GetComponent<MeshRenderer>().material;
            material.SetColor("_TopColor", Top);
            material.SetColor("_MiddleColor", Middle);
            material.SetColor("_BottomColor", Bottom);
        }
        public static void MakeSellable(this IdentifiableType ident, float marketValue, float marketSaturation, bool hideInMarket = false)
        {
            if (marketData.ContainsKey(ident))
            {
                MelonLogger.Error("Failed to make object sellable: The object is already sellable");
                return;
            }

            if (removeMarketPlortEntries.Contains(ident))
                removeMarketPlortEntries.Remove(ident);
            marketPlortEntries.Add(new MarketUI.PlortEntry { identType = ident }, hideInMarket);
            marketData.Add(ident, new ModdedMarketData(marketSaturation, marketValue));
        }

        public static bool isSellable(this IdentifiableType ident)
        {
            bool returnBool = false;
            List<string> sellableByDefault = new List<string> { "PinkPlort", "CottonPlort", "PhosphorPlort", "TabbyPlort", "AnglerPlort", "RockPlort", "HoneyPlort", "BoomPlort", "PuddlePlort", "FirePlort", "BattyPlort", "CrystalPlort", "HunterPlort", "FlutterPlort", "RingtailPlort", "SaberPlort", "YolkyPlort", "TanglePlort", "DervishPlort", "GoldPlort" };
            if (removeMarketPlortEntries.Count != 0)
                foreach (string sellable in sellableByDefault)
                    if (sellable == ident.name)
                    {
                        returnBool = true;
                        foreach (IdentifiableType removed in removeMarketPlortEntries)
                            if (ident == removed)
                                returnBool = false;
                    }

            if (marketData.ContainsKey(ident))
                return true;
            foreach (var keyPair in marketPlortEntries)
            {
                MarketUI.PlortEntry entry = keyPair.Key;
                if (entry.identType == ident)
                    return true;
            }
            return returnBool;
        }
        public static void MakeNOTSellable(this IdentifiableType ident)
        {
            removeMarketPlortEntries.Add(ident);
            foreach (var keyPair in marketPlortEntries)
            {
                MarketUI.PlortEntry entry = keyPair.Key;
                if (entry.identType == ident)
                {
                    marketPlortEntries.Remove(entry);
                    break;
                }
            }

            if (marketData.ContainsKey(ident))
                marketData.Remove(ident);
        }

        public static Il2CppArrayBase<IdentifiableType> GetAllMembersArray(this IdentifiableTypeGroup group)
        {
            return group.GetAllMembers().ToArray();
        }
        public static SlimeDefinition GetSlime(string name)
        {
            foreach (IdentifiableType type in slimes.GetAllMembersArray())
                if (type.name.ToUpper() == name.ToUpper())
                    return type.Cast<SlimeDefinition>();

            return null;
        }

        public static IdentifiableType GetPlort(string name)
        {
            foreach (IdentifiableType type in plorts.GetAllMembersArray())
                if (type.name.ToUpper() == name.ToUpper())
                    return type;

            return null;
        }
        public static IdentifiableType GetCraft(string name)
        {
            foreach (IdentifiableType type in crafts.GetAllMembersArray())
                if (type.name.ToUpper() == name.ToUpper())
                    return type;

            return null;
        }
        public static void SetSlimeColor(this SlimeDefinition slimedef, Color32 Top, Color32 Middle, Color32 Bottom, Color32 Spec, int index, int index2, bool isSS, int structure)
        {
            Material mat = null;
            if (isSS == true)
            {
                mat = slimedef.AppearancesDynamic.ToArray()[index].Structures[structure].DefaultMaterials[index2];
            }
            else
            {
                mat = slimedef.AppearancesDefault[index].Structures[structure].DefaultMaterials[index2];
            }
            mat.SetColor("_TopColor", Top);
            mat.SetColor("_MiddleColor", Middle);
            mat.SetColor("_BottomColor", Bottom);
            mat.SetColor("_SpecColor", Spec);
        }

        public static Sprite ConvertToSprite(this Texture2D texture)
        {
            return Sprite.Create(texture, new Rect(0f, 0f, (float)texture.width, (float)texture.height), new Vector2(0.5f, 0.5f), 1f);
        }
        public static GameObject CopyObject(this GameObject obj) => Object.Instantiate(obj, rootOBJ.transform);



        public static Il2CppReferenceArray<T> Add<T>(this Il2CppReferenceArray<T> array, T obj) where T : Il2CppObjectBase
        {
            var list = new Il2CppSystem.Collections.Generic.List<T>();
            foreach (var item in array)
            {
                list.Add(item);
            }

            list.Add(obj);

            array = list.ToArray().Cast<Il2CppReferenceArray<T>>();
            return array;
        }

        public static Il2CppReferenceArray<T> AddRange<T>(this Il2CppReferenceArray<T> array, Il2CppReferenceArray<T> obj) where T : Il2CppObjectBase
        {
            var list = new Il2CppSystem.Collections.Generic.List<T>();
            foreach (var item in array)
            {
                list.Add(item);
            }
            foreach (var item in obj)
            {
                list.Add(item);
            }
            array = list.ToArray().Cast<Il2CppReferenceArray<T>>();
            return array;
        }
        public static Il2CppReferenceArray<T> AddRangeNoMultiple<T>(this Il2CppReferenceArray<T> array, Il2CppReferenceArray<T> obj) where T : Il2CppObjectBase
        {
            var list = new Il2CppSystem.Collections.Generic.List<T>();
            foreach (var item in array)
            {
                if (!list.Contains(item))
                    list.Add(item);
            }
            foreach (var item in obj)
            {
                if (!list.Contains(item))
                    list.Add(item);
            }
            array = list.ToArray().Cast<Il2CppReferenceArray<T>>();
            return array;
        }
        public static void AddListRange<T>(this Il2CppSystem.Collections.Generic.List<T> list, Il2CppSystem.Collections.Generic.List<T> obj) where T : Il2CppObjectBase
        {
            foreach (var iobj in obj)
            {
                list.Add(iobj);
            }
        }
        public static void AddListRangeNoMultiple<T>(this Il2CppSystem.Collections.Generic.List<T> list, Il2CppSystem.Collections.Generic.List<T> obj) where T : Il2CppObjectBase
        {
            foreach (var iobj in obj)
            {
                if (!list.Contains(iobj))
                {
                    list.Add(iobj);
                }
            }
        }
        public static GameObject? Get(string name) => Get<GameObject>(name);
        public static void AddString(this Il2CppStringArray array, string obj)
        {
            var s = new string[0];
            array = HarmonyLib.CollectionExtensions.AddToArray<string>(s, obj);
        }
        public static void MakePrefab(this GameObject obj)
        {
            UnityEngine.Object.DontDestroyOnLoad(obj);
            obj.transform.parent = rootOBJ.transform;
        }
        public static GameV04? Save => gameContext.AutoSaveDirector.SavedGame.gameState;
        public static void SetSlimeMatTopColor(this Material mat, Color color) => mat.SetColor("_TopColor", color);
        public static void SetSlimeMatMiddleColor(this Material mat, Color color) => mat.SetColor("_MiddleColor", color);

        public static void SetSlimeMatBottomColor(this Material mat, Color color) => mat.SetColor("_BottomColor", color);

        public static void SetSlimeMatColors(this Material material, Color32 Top, Color32 Middle, Color32 Bottom, Color32 Specular)
        {
            material.SetColor("_TopColor", Top);
            material.SetColor("_MiddleColor", Middle);
            material.SetColor("_BottomColor", Bottom);
            material.SetColor("_SpecColor", Specular);
        }
        public static void SetSlimeMatColors(this Material material, Color32 Top, Color32 Middle, Color32 Bottom)
        {
            material.SetColor("_TopColor", Top);
            material.SetColor("_MiddleColor", Middle);
            material.SetColor("_BottomColor", Bottom);
        }


        public static void SetAppearanceVacColor(this SlimeAppearance appearance, Color color)
        {
            appearance._colorPalette = new SlimeAppearance.Palette()
            {
                Top = appearance._colorPalette.Top,
                Middle = appearance._colorPalette.Middle,
                Bottom = appearance._colorPalette.Bottom,
                Ammo = color
            };
        }

        public static SlimeDefinition GetBaseSlime(string name)
        {
            foreach (IdentifiableType type in baseSlimes.GetAllMembersArray())
                if (type.name.ToUpper() == name.ToUpper())
                    return type.Cast<SlimeDefinition>();
            return null;
        }
        public static SlimeDefinition GetFruit(string name)
        {
            foreach (IdentifiableType type in fruits.GetAllMembersArray())
                if (type.name.ToUpper() == name.ToUpper())
                    return type.Cast<SlimeDefinition>();
            return null;
        }
        public static SlimeDefinition GetFood(string name)
        {
            foreach (IdentifiableType type in food.GetAllMembersArray())
                if (type.name.ToUpper() == name.ToUpper())
                    return type.Cast<SlimeDefinition>();
            return null;
        }
        public static SlimeDefinition GetMeat(string name)
        {
            foreach (IdentifiableType type in meat.GetAllMembersArray())
                if (type.name.ToUpper() == name.ToUpper())
                    return type.Cast<SlimeDefinition>();
            return null;
        }
        public static SlimeDefinition GetVeggie(string name)
        {
            foreach (IdentifiableType type in veggies.GetAllMembersArray())
                if (type.name.ToUpper() == name.ToUpper())
                    return type.Cast<SlimeDefinition>();
            return null;
        }
        public static SlimeDefinition GetLargo(string name)
        {
            foreach (IdentifiableType type in largos.GetAllMembersArray())
                if (type.name.ToUpper() == name.ToUpper())
                    return type.Cast<SlimeDefinition>();
            return null;
        }




        internal static Sprite LoadSprite(string fileName) => ConvertToSprite(LoadImage(fileName));

        internal static Texture2D LoadImage(string filename)
        {
            Assembly executingAssembly = Assembly.GetExecutingAssembly();
            System.IO.Stream manifestResourceStream = executingAssembly.GetManifestResourceStream(executingAssembly.GetName().Name + "." + filename + ".png");
            byte[] array = new byte[manifestResourceStream.Length];
            manifestResourceStream.Read(array, 0, array.Length);
            Texture2D texture2D = new Texture2D(1, 1);
            ImageConversion.LoadImage(texture2D, array);
            texture2D.filterMode = FilterMode.Bilinear;
            return texture2D;
        }


        internal static Dictionary<MelonPreferences_Entry, System.Action> entriesWithoutWarning = new Dictionary<MelonPreferences_Entry, Action>();
        public static void disableWarning(this MelonPreferences_Entry entry) => entriesWithoutWarning.Add(entry, null);
        public static void disableWarning(this MelonPreferences_Entry entry, System.Action action) => entriesWithoutWarning.Add(entry, action);

        public static bool IsInsideRange(int number, int rangeMin, int rangeMax) => (number >= rangeMin && number <= rangeMax);

        public static GameObject GetConsoleObject()
        {
            return SR2Console.transform.getObjRec<GameObject>("consoleMenu");
        }

        public static bool RemoveComponent<T>(this GameObject obj) where T : Component
        {
            try
            {
                T comp = obj.GetComponent<T>();
                var check = comp.gameObject;
                Object.Destroy(comp);
                return true;
            }
            catch { return false; }
        }
        public static bool RemoveComponent<T>(this Transform obj) where T : Component
        {
            try
            {
                T comp = obj.GetComponent<T>();
                var check = comp.gameObject;
                Object.Destroy(comp);
                return true;
            }
            catch { return false; }
        }
        internal static Gadget RaycastForGadget()
        {
            if (Physics.Raycast(new Ray(Camera.main.transform.position, Camera.main.transform.forward), out var hit))
            {
                Transform currentParent = hit.collider.transform.parent;

                for (int i = 0; i < 10 && currentParent != null; i++)
                {
                    Gadget gadgetComponent = currentParent.GetComponent<Gadget>();

                    if (gadgetComponent != null)
                    {
                        return gadgetComponent;
                    }

                    currentParent = currentParent.parent;
                }

                return null;
            }
            return null;
        }
        internal static KeyCode KeyToKeyCode(Key key)
        {
            switch (key)
            {
                case Key.Space:
                    return KeyCode.Space;
                case Key.None:
                    return KeyCode.None;
                case Key.Enter:
                    return KeyCode.Return;
                case Key.Tab:
                    return KeyCode.Tab;
                case Key.Backquote:
                    return KeyCode.BackQuote;
                case Key.Quote:
                    return KeyCode.Quote;
                case Key.Semicolon:
                    return KeyCode.Semicolon;
                case Key.Comma:
                    return KeyCode.Comma;
                case Key.Period:
                    return KeyCode.Period;
                case Key.Slash:
                    return KeyCode.Slash;
                case Key.Backslash:
                    return KeyCode.Backslash;
                case Key.LeftBracket:
                    return KeyCode.LeftBracket;
                case Key.RightBracket:
                    return KeyCode.RightBracket;
                case Key.Minus:
                    return KeyCode.Minus;
                case Key.Equals:
                    return KeyCode.Equals;
                case Key.A:
                    return KeyCode.A;
                case Key.B:
                    return KeyCode.B;
                case Key.C:
                    return KeyCode.C;
                case Key.D:
                    return KeyCode.D;
                case Key.E:
                    return KeyCode.E;
                case Key.F:
                    return KeyCode.F;
                case Key.G:
                    return KeyCode.G;
                case Key.H:
                    return KeyCode.H;
                case Key.I:
                    return KeyCode.I;
                case Key.J:
                    return KeyCode.J;
                case Key.K:
                    return KeyCode.K;
                case Key.L:
                    return KeyCode.L;
                case Key.M:
                    return KeyCode.M;
                case Key.N:
                    return KeyCode.N;
                case Key.O:
                    return KeyCode.O;
                case Key.P:
                    return KeyCode.P;
                case Key.Q:
                    return KeyCode.Q;
                case Key.R:
                    return KeyCode.R;
                case Key.S:
                    return KeyCode.S;
                case Key.T:
                    return KeyCode.T;
                case Key.U:
                    return KeyCode.U;
                case Key.V:
                    return KeyCode.V;
                case Key.W:
                    return KeyCode.W;
                case Key.X:
                    return KeyCode.X;
                case Key.Y:
                    return KeyCode.Y;
                case Key.Z:
                    return KeyCode.Z;
                case Key.Digit1:
                    return KeyCode.Alpha1;
                case Key.Digit2:
                    return KeyCode.Alpha2;
                case Key.Digit3:
                    return KeyCode.Alpha3;
                case Key.Digit4:
                    return KeyCode.Alpha4;
                case Key.Digit5:
                    return KeyCode.Alpha5;
                case Key.Digit6:
                    return KeyCode.Alpha6;
                case Key.Digit7:
                    return KeyCode.Alpha7;
                case Key.Digit8:
                    return KeyCode.Alpha8;
                case Key.Digit9:
                    return KeyCode.Alpha9;
                case Key.Digit0:
                    return KeyCode.Alpha0;
                case Key.LeftShift:
                    return KeyCode.LeftShift;
                case Key.RightShift:
                    return KeyCode.RightShift;
                case Key.LeftAlt:
                    return KeyCode.LeftAlt;
                case Key.RightAlt:
                    return KeyCode.RightAlt;
                case Key.LeftCtrl:
                    return KeyCode.LeftControl;
                case Key.RightCtrl:
                    return KeyCode.RightControl;
                case Key.LeftWindows:
                    return KeyCode.LeftWindows;
                case Key.RightWindows:
                    return KeyCode.RightWindows;
                //Disabled to allow exiting key choose
                //case Key.Escape:
                //    return KeyCode.Escape;
                case Key.LeftArrow:
                    return KeyCode.LeftArrow;
                case Key.RightArrow:
                    return KeyCode.RightArrow;
                case Key.UpArrow:
                    return KeyCode.UpArrow;
                case Key.DownArrow:
                    return KeyCode.DownArrow;
                case Key.Backspace:
                    return KeyCode.Backspace;
                case Key.PageDown:
                    return KeyCode.PageDown;
                case Key.PageUp:
                    return KeyCode.PageUp;
                case Key.Home:
                    return KeyCode.Home;
                case Key.End:
                    return KeyCode.End;
                case Key.Insert:
                    return KeyCode.Insert;
                case Key.Delete:
                    return KeyCode.Delete;
                case Key.CapsLock:
                    return KeyCode.CapsLock;
                case Key.NumLock:
                    return KeyCode.Numlock;
                case Key.PrintScreen:
                    return KeyCode.Print;
                case Key.Pause:
                    return KeyCode.Pause;
                case Key.NumpadEnter:
                    return KeyCode.KeypadEnter;
                case Key.NumpadDivide:
                    return KeyCode.KeypadDivide;
                case Key.NumpadMultiply:
                    return KeyCode.KeypadMultiply;
                case Key.NumpadPlus:
                    return KeyCode.KeypadPlus;
                case Key.NumpadMinus:
                    return KeyCode.KeypadMinus;
                case Key.NumpadPeriod:
                    return KeyCode.KeypadPeriod;
                case Key.NumpadEquals:
                    return KeyCode.KeypadEquals;
                case Key.Numpad0:
                    return KeyCode.Keypad0;
                case Key.Numpad1:
                    return KeyCode.Keypad1;
                case Key.Numpad2:
                    return KeyCode.Keypad2;
                case Key.Numpad3:
                    return KeyCode.Keypad3;
                case Key.Numpad4:
                    return KeyCode.Keypad4;
                case Key.Numpad5:
                    return KeyCode.Keypad5;
                case Key.Numpad6:
                    return KeyCode.Keypad6;
                case Key.Numpad7:
                    return KeyCode.Keypad7;
                case Key.Numpad8:
                    return KeyCode.Keypad8;
                case Key.Numpad9:
                    return KeyCode.Keypad9;
                case Key.F1:
                    return KeyCode.F1;
                case Key.F2:
                    return KeyCode.F2;
                case Key.F3:
                    return KeyCode.F3;
                case Key.F4:
                    return KeyCode.F4;
                case Key.F5:
                    return KeyCode.F5;
                case Key.F6:
                    return KeyCode.F6;
                case Key.F7:
                    return KeyCode.F7;
                case Key.F8:
                    return KeyCode.F8;
                case Key.F9:
                    return KeyCode.F9;
                case Key.F10:
                    return KeyCode.F10;
                case Key.F11:
                    return KeyCode.F11;
                case Key.F12:
                    return KeyCode.F12;
                default:
                    return KeyCode.None;
            }
        }
        public static Il2CppSystem.Type il2cppTypeof(this Type type)
        {
            string typeName = type.AssemblyQualifiedName;

            if (typeName.ToLower().StartsWith("il2cpp"))
            {
                typeName = typeName.Substring("il2cpp".Length);
            }

            Il2CppSystem.Type il2cppType = Il2CppSystem.Type.GetType(typeName);

            return il2cppType;
        }

        public static T getObjRec<T>(this GameObject obj, string name) where T : class
        {
            var transform = obj.transform;

            List<GameObject> totalChildren = getAllChildren(transform);
            for (int i = 0; i < totalChildren.Count; i++)
                if (totalChildren[i].name == name)
                {
                    if (typeof(T) == typeof(GameObject))
                        return totalChildren[i] as T;
                    if (typeof(T) == typeof(Transform))
                        return totalChildren[i].transform as T;
                    if (totalChildren[i].GetComponent<T>() != null)
                        return totalChildren[i].GetComponent<T>();
                }
            return null;
        }

        public static List<GameObject> getAllChildren(this GameObject obj)
        {
            var container = obj.transform;
            List<GameObject> allChildren = new List<GameObject>();
            for (int i = 0; i < container.childCount; i++)
            {
                var child = container.GetChild(i);
                allChildren.Add(child.gameObject);
                allChildren.AddRange(getAllChildren(child));
            }
            return allChildren;
        }

        public static T[] getAllChildrenOfType<T>(this GameObject obj) where T : Component
        {
            List<T> children = new List<T>();
            foreach (var child in obj.getAllChildren())
            {
                if (child.GetComponent<T>() != null)
                {
                    children.Add(child.GetComponent<T>());
                }
            }
            return children.ToArray();
        }

        public static T[] getAllChildrenOfType<T>(this Transform obj) where T : Component
        {
            List<T> children = new List<T>();
            foreach (var child in obj.getAllChildren())
            {
                if (child.GetComponent<T>() != null)
                {
                    children.Add(child.GetComponent<T>());
                }
            }
            return children.ToArray();
        }

        public static T getObjRec<T>(this Transform transform, string name) where T : class
        {
            List<GameObject> totalChildren = getAllChildren(transform);
            for (int i = 0; i < totalChildren.Count; i++)
                if (totalChildren[i].name == name)
                {
                    if (typeof(T) == typeof(GameObject))
                        return totalChildren[i] as T;
                    if (typeof(T) == typeof(Transform))
                        return totalChildren[i].transform as T;
                    if (totalChildren[i].GetComponent<T>() != null)
                        return totalChildren[i].GetComponent<T>();
                }
            return null;
        }

        public static List<GameObject> getAllChildren(this Transform container)
        {
            List<GameObject> allChildren = new List<GameObject>();
            for (int i = 0; i < container.childCount; i++)
            {
                var child = container.GetChild(i);
                allChildren.Add(child.gameObject);
                allChildren.AddRange(getAllChildren(child));
            }
            return allChildren;
        }
        public static bool inGame
        {
            get
            {
                try
                {
                    if (SceneContext.Instance == null) return false;
                    if (SceneContext.Instance.PlayerState == null) return false;
                }
                catch
                { return false; }
                return true;
            }
        }

        public static IdentifiableType GetIdent(this GameObject obj)
        {
            try
            {
                return obj.GetComponent<IdentifiableActor>().identType;
            }
            catch
            {
                return null;
            }
        }

    }
}
