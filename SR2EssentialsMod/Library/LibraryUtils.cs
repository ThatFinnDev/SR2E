using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppInterop.Runtime.InteropTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Il2CppMonomiPark.SlimeRancher.Persist;
using Il2CppMonomiPark.SlimeRancher.Script.Util;
using Il2CppMonomiPark.SlimeRancher.UI;
using Il2CppSystem.IO;
using UnityEngine.Localization;
using UnityEngine.Localization.Tables;
using Il2CppMonomiPark.SlimeRancher.Weather;

namespace SR2E.Library
{
    public static class LibraryUtils
    {
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
            slimedef.AppearancesDefault.Add(appearance);
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

            mergedDiet.AdditionalFoodIdents.AddRange(firstDiet.AdditionalFoodIdents);
            mergedDiet.AdditionalFoodIdents.AddRange(secondDiet.AdditionalFoodIdents);

            mergedDiet.FavoriteIdents.AddRange(firstDiet.FavoriteIdents);
            mergedDiet.FavoriteIdents.AddRange(secondDiet.FavoriteIdents);

            mergedDiet.ProduceIdents.AddRange(firstDiet.ProduceIdents);
            mergedDiet.ProduceIdents.AddRange(secondDiet.ProduceIdents);

            return mergedDiet;
        }
        public static void SwitchSlimeAppearances(this SlimeDefinition slimeOneDef, SlimeDefinition slimeTwoDef)
        {
            var appearanceOne = slimeOneDef.AppearancesDefault[0]._structures; slimeOneDef.AppearancesDefault[0]._structures = slimeTwoDef.AppearancesDefault[0]._structures; slimeTwoDef.AppearancesDefault[0]._structures = appearanceOne;

            var structureIcon = slimeOneDef.AppearancesDefault[0]._icon; slimeOneDef.AppearancesDefault[0]._icon = slimeTwoDef.AppearancesDefault[0]._icon; slimeTwoDef.AppearancesDefault[0]._icon = structureIcon;
            var icon = slimeOneDef.icon; slimeOneDef.icon = slimeTwoDef.icon; slimeTwoDef.icon = icon;

            var debugIcon = slimeOneDef.debugIcon; slimeOneDef.debugIcon = slimeTwoDef.debugIcon; slimeTwoDef.debugIcon = debugIcon;

        }

        public enum LargoSettings { KeepFirstBody, KeepSecondBody, KeepFirstFace, KeepSecondFace, KeepFirstColor, KeepSecondColor, MergeColors }
        public static SlimeDefinitions? slimeDefinitions { get { return gameContext.SlimeDefinitions; } set { gameContext.SlimeDefinitions = value; } }



        
        public static Dictionary<string, Dictionary<string, string>> addedTranslations = new Dictionary<string, System.Collections.Generic.Dictionary<string, string>>();

        public static LocalizedString AddTranslation(string localized, string key = "l.SR2ELibraryTest", string table = "Actor")
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

        public static IdentifiableType GetIdent(this GameObject obj)
        {
            var comp = obj.GetComponent<IdentifiableActor>();

            if (comp != null)
            {
                return comp.identType;
            }
            else { return null; }
        }
        public static SlimeDiet.EatMapEntry CreateEatmap(this SlimeDefinition def, SlimeEmotions.Emotion driver, float mindrive, IdentifiableType produce, IdentifiableType eat, IdentifiableType becomes)
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
        public static SlimeDiet.EatMapEntry CreateEatmap(this SlimeDefinition def, SlimeEmotions.Emotion driver, float mindrive, IdentifiableType produce, IdentifiableType eat)
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
            slimedef.Diet.ProduceIdents.Add(ident);
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
            def.Diet.MajorFoodGroups.AddItem(FG);
        }
        public static GameObject SpawnActor(this GameObject obj, Vector3 pos)
        {
            return SRBehaviour.InstantiateActor(obj,
                SRSingleton<SceneContext>.Instance.RegionRegistry.CurrentSceneGroup, pos, Quaternion.identity,
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

            diet.ProduceIdents = new Il2CppInterop.Runtime.InteropTypes.Arrays.Il2CppReferenceArray<IdentifiableType>(1);
            diet.FavoriteProductionCount = 2;
            diet.MajorFoodGroups = new Il2CppInterop.Runtime.InteropTypes.Arrays.Il2CppStructArray<SlimeEat.FoodGroup>(1);
            diet.EatMap = new Il2CppSystem.Collections.Generic.List<SlimeDiet.EatMapEntry>(1);
            diet.FavoriteIdents = new Il2CppInterop.Runtime.InteropTypes.Arrays.Il2CppReferenceArray<IdentifiableType>(1);
            diet.AdditionalFoodIdents = new Il2CppInterop.Runtime.InteropTypes.Arrays.Il2CppReferenceArray<IdentifiableType>(1);
            diet.MajorFoodIdentifiableTypeGroups = new Il2CppInterop.Runtime.InteropTypes.Arrays.Il2CppReferenceArray<IdentifiableTypeGroup>(1);
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
        public static void MakeSellable(this IdentifiableType ident, float marketValue, float marketSaturation, bool hideInMarket=false)
        {
            if (marketData.ContainsKey(ident))
            {
                MelonLogger.Error("Failed to make object sellable: The object is already sellable");
                return;
            }
            
            if (removeMarketPlortEntries.Contains(ident))
                removeMarketPlortEntries.Remove(ident);
            marketPlortEntries.Add(new MarketUI.PlortEntry { identType = ident },hideInMarket);
            marketData.Add(ident, new ModdedMarketData(marketSaturation, marketValue));
        }

        public static bool isSellable(this IdentifiableType ident)
        {
            bool returnBool = false;
            List<string> sellableByDefault = new List<string> { "PinkPlort", "CottonPlort", "PhosphorPlort", "TabbyPlort", "AnglerPlort", "RockPlort", "HoneyPlort", "BoomPlort", "PuddlePlort", "FirePlort", "BattyPlort", "CrystalPlort", "HunterPlort", "FlutterPlort", "RingtailPlort", "SaberPlort", "YolkyPlort", "TanglePlort", "DervishPlort", "GoldPlort" };
            if(removeMarketPlortEntries.Count!=0)
                foreach (string sellable in sellableByDefault)
                    if (sellable == ident.name)
                    {
                        returnBool = true;
                        foreach (IdentifiableType removed in removeMarketPlortEntries)
                            if (ident == removed)
                                return false;
                    }
            
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
        public static void SetSlimeColor(this SlimeDefinition slimedef, Color32 Top, Color32 Middle, Color32 Bottom, Color32 Spec,  int index, int index2, bool isSS, int structure)
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
        }/*
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
        */

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
    }
}
