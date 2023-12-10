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

namespace SR2E.Library
{
    public static class LibraryUtils
    {
        internal static Dictionary<IdentifiableType, ModdedMarketData> marketData = new Dictionary<IdentifiableType, ModdedMarketData>(0);
        internal static Dictionary<MarketUI.PlortEntry, bool> marketPlortEntries = new Dictionary<MarketUI.PlortEntry, bool>();
        internal static List<IdentifiableType> removeMarketPlortEntries = new List<IdentifiableType>();
        internal static GameObject rootOBJ;
        internal static Dictionary<SlimeDefinition, LargoSettings>? LargoData = new Dictionary<SlimeDefinition, LargoSettings>(0);
        internal static Dictionary<string, Dictionary<string, string>> addedTranslations = new Dictionary<string, Dictionary<string, string>>();
        
        public static IdentifiableTypeGroup? slimes;
        public static IdentifiableTypeGroup? plorts;
        public static IdentifiableTypeGroup? largos;
        public static IdentifiableTypeGroup? baseSlimes;
        public static IdentifiableTypeGroup? food;
        public static IdentifiableTypeGroup? meat;
        public static IdentifiableTypeGroup? veggies;
        public static IdentifiableTypeGroup? fruits;
        public static GameObject? player;

       public enum VanillaPediaEntryCategories { TUTORIAL, SLIMES, RESOURCES, WORLD, RANCH, SCIENCE }
        public enum LargoSettings { KeepFirstBody, KeepSecondBody, KeepFirstFace, KeepSecondFace, KeepFirstColor, KeepSecondColor, MergeColors }
        
        public static SystemContext systemContext { get { return SystemContext.Instance; } }
        public static GameContext gameContext { get { return GameContext.Instance; } }
        public static SceneContext sceneContext { get { return SceneContext.Instance; } }
        public static SlimeDefinitions slimeDefinitions { get { return gameContext.SlimeDefinitions; } set { gameContext.SlimeDefinitions = value; } }
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
                slimedef.AppearancesDefault.AddItem(appearance);
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
                slimedef.AppearancesDefault.AddItem(appearance);
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
            slimeDefinitions.Slimes.AddItem(slimedef);
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

        public static SlimeDiet CreateMergedDiet(this SlimeDiet firstDiet, SlimeDiet secondDiet)
        {
            var mergedDiet = INTERNAL_CreateNewDiet();

            foreach (SlimeDiet.EatMapEntry entry in firstDiet.EatMap) if(!mergedDiet.EatMap.Contains(entry)) mergedDiet.EatMap.Add(entry);
            foreach (SlimeDiet.EatMapEntry entry in secondDiet.EatMap) if(!mergedDiet.EatMap.Contains(entry)) mergedDiet.EatMap.Add(entry);

            mergedDiet.AdditionalFoodIdents = HarmonyLib.CollectionExtensions.AddRangeToArray<IdentifiableType>(mergedDiet.AdditionalFoodIdents, firstDiet.AdditionalFoodIdents);
            mergedDiet.AdditionalFoodIdents = HarmonyLib.CollectionExtensions.AddRangeToArray<IdentifiableType>(mergedDiet.AdditionalFoodIdents, firstDiet.AdditionalFoodIdents);

            mergedDiet.FavoriteIdents = HarmonyLib.CollectionExtensions.AddRangeToArray<IdentifiableType>(mergedDiet.FavoriteIdents, firstDiet.FavoriteIdents);
            mergedDiet.FavoriteIdents = HarmonyLib.CollectionExtensions.AddRangeToArray<IdentifiableType>(mergedDiet.FavoriteIdents, firstDiet.FavoriteIdents);

            mergedDiet.ProduceIdents = HarmonyLib.CollectionExtensions.AddRangeToArray<IdentifiableType>(mergedDiet.ProduceIdents, firstDiet.ProduceIdents);
            mergedDiet.ProduceIdents = HarmonyLib.CollectionExtensions.AddRangeToArray<IdentifiableType>(mergedDiet.ProduceIdents, secondDiet.ProduceIdents);

            return mergedDiet;
        }
        public static void switchSlimeAppearances(this SlimeDefinition slimeOneDef, SlimeDefinition slimeTwoDef)
        {
            var appearanceOne = slimeOneDef.AppearancesDefault[0]._structures; slimeOneDef.AppearancesDefault[0]._structures = slimeTwoDef.AppearancesDefault[0]._structures; slimeTwoDef.AppearancesDefault[0]._structures = appearanceOne;

            var structureIcon = slimeOneDef.AppearancesDefault[0]._icon; slimeOneDef.AppearancesDefault[0]._icon = slimeTwoDef.AppearancesDefault[0]._icon; slimeTwoDef.AppearancesDefault[0]._icon = structureIcon;
            var icon = slimeOneDef.icon; slimeOneDef.icon = slimeTwoDef.icon; slimeTwoDef.icon = icon;

            var debugIcon = slimeOneDef.debugIcon; slimeOneDef.debugIcon = slimeTwoDef.debugIcon; slimeTwoDef.debugIcon = debugIcon;

        }

        


        
        
        
        public static IdentifiableType GetIdent(this GameObject obj)
        {
            var comp = obj.GetComponent<IdentifiableActor>();
            if (comp != null)
                return comp.identType;
            return null;
        }
        public static SlimeDiet.EatMapEntry CreateEatmap(this SlimeDefinition def, SlimeEmotions.Emotion driver, float minDrive, IdentifiableType produce, IdentifiableType eat, IdentifiableType becomes) =>
            new SlimeDiet.EatMapEntry { EatsIdent = eat, ProducesIdent = produce, BecomesIdent = becomes, Driver = driver, MinDrive = minDrive };
        

        public static SlimeDiet.EatMapEntry CreateEatmap(this SlimeDefinition def, SlimeEmotions.Emotion driver, float minDrive, IdentifiableType produce, IdentifiableType eat) => 
            new SlimeDiet.EatMapEntry { EatsIdent = eat, ProducesIdent = produce, Driver = driver, MinDrive = minDrive };
        
        public static void ModifyEatmap(this SlimeDiet.EatMapEntry eatmap, SlimeEmotions.Emotion driver, float mindrive, IdentifiableType produce, IdentifiableType eat, IdentifiableType becomes)
        { eatmap.EatsIdent = eat; eatmap.BecomesIdent = becomes; eatmap.ProducesIdent = produce; eatmap.Driver = driver; eatmap.MinDrive = mindrive; }
        public static void ModifyEatmap(this SlimeDiet.EatMapEntry eatmap, SlimeEmotions.Emotion driver, float mindrive, IdentifiableType produce, IdentifiableType eat)
        { eatmap.EatsIdent = eat; eatmap.ProducesIdent = produce; eatmap.Driver = driver; eatmap.MinDrive = mindrive; }
        public static void RefreshEatmap(this SlimeDefinition def) =>
            def.Diet.RefreshEatMap(slimeDefinitions, def);
        public static void AddEatmap(this SlimeDefinition slimeDef, SlimeDiet.EatMapEntry eatmap) =>
            slimeDef.Diet.EatMap.Add(eatmap);
        
        public static void AddProduceIdent(this SlimeDefinition slimeDef, IdentifiableType ident) =>
            slimeDef.Diet.ProduceIdents.AddItem(ident);
        public static void SetProduceIdent(this SlimeDefinition slimeDef, IdentifiableType ident, int index) => 
            slimeDef.Diet.ProduceIdents[index] = ident;
        public static void AddExtraEatIdent(this SlimeDefinition slimeDef, IdentifiableType ident) =>
            slimeDef.Diet.AdditionalFoodIdents.AddItem(ident);
        public static void SetFavoriteProduceCount(this SlimeDefinition slimeDef, int count) => 
            slimeDef.Diet.FavoriteProductionCount = count;
            
            
        public static void ChangeSlimeFoodGroup(this SlimeDefinition def, SlimeEat.FoodGroup FG, int index) =>
            def.Diet.MajorFoodGroups[index] = FG;
        public static void AddSlimeFoodGroup(this SlimeDefinition def, SlimeEat.FoodGroup FG) =>
            def.Diet.MajorFoodGroups.AddItem<SlimeEat.FoodGroup>(FG);
        
        public static void SetStructColor(this SlimeAppearanceStructure structure, int id, Color color) =>
            structure.DefaultMaterials[0].SetColor(id, color);
        public static void SetTopColor(this SlimeAppearanceStructure structure, Color color) =>
            structure.DefaultMaterials[0].SetColor("_TopColor", color);
        public static void SetMiddleColor(this SlimeAppearanceStructure structure, Color color) =>
            structure.DefaultMaterials[0].SetColor("_MiddleColor", color);
        public static void SetBottomColor(this SlimeAppearanceStructure structure, Color color) =>
            structure.DefaultMaterials[0].SetColor("_BottomColor", color);
            
        public static GameObject SpawnActor(this GameObject obj, Vector3 pos) =>
            SRBehaviour.InstantiateActor(obj, SRSingleton<SceneContext>.Instance.RegionRegistry.CurrentSceneGroup, pos, Quaternion.identity, false); 
        public static GameObject SpawnDynamic(this GameObject obj, Vector3 pos) => 
            SRBehaviour.InstantiateDynamic(obj, pos, Quaternion.identity, false);
            
        
        public static void AddToGroup(this IdentifiableType type, string groupName)
        {
            var group = Get<IdentifiableTypeGroup>(groupName);
            if(group!=null)
            { group.memberTypes.Add(type); return; }
            MelonLogger.Msg($"There is no group called {groupName}");
        }
        public static void AddToGroup(this IdentifiableType type, IdentifiableTypeGroup group) =>
            group.memberTypes.Add(type);
        public static IdentifiableTypeGroup MakeNewGroup(IdentifiableType[] types, string groupName, IdentifiableTypeGroup[] subGroups = null) 
        {
            var group = ScriptableObject.CreateInstance<IdentifiableTypeGroup>();

            var typesList = new Il2CppSystem.Collections.Generic.List<IdentifiableType>();
            if (types != null) 
                foreach (var type in types) 
                    try { typesList.Add(type); } catch { }
            

            var subGroupsList = new Il2CppSystem.Collections.Generic.List<IdentifiableTypeGroup>();
            if (subGroups != null) 
                foreach (var subGroup in subGroups) 
                    try { subGroupsList.Add(subGroup); }catch { }
            
            group.memberTypes = typesList;
            group.memberGroups = subGroupsList;
            group.hideFlags = HideFlags.HideAndDontSave;
            return group;
        }



        public static GameObject CreatePrefab(string Name, GameObject baseObject)
        {
            var obj = CopyObject(baseObject);
            UnityEngine.Object.DontDestroyOnLoad(obj);
            obj.name = Name;
            obj.transform.parent = rootOBJ.transform;
            var components = obj.GetComponents<Behaviour>();
            foreach (var component in components)
                component.enabled = true;
            return obj;
        }
        public static T? Get<T>(string name) where T : Object => Resources.FindObjectsOfTypeAll<T>().FirstOrDefault((T x) => x.name == name); 

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
        
        public static void SetColors(this Material material, Color32 Top, Color32 Middle, Color32 Bottom, Color32 Specular)
        {
            material.SetColor("_TopColor", Top);
            material.SetColor("_MiddleColor", Middle);
            material.SetColor("_BottomColor", Bottom);
            material.SetColor("_SpecColor", Specular);
        }
        public static void SetColors(this Material material, Color32 Top, Color32 Middle, Color32 Bottom)
        {
            material.SetColor("_TopColor", Top);
            material.SetColor("_MiddleColor", Middle);
            material.SetColor("_BottomColor", Bottom);
        }
        public static void ChangeSlimeColors(this SlimeDefinition slimedef, Color32 Top, Color32 Middle, Color32 Bottom, Color32 Spec)
        => slimedef.AppearancesDefault[0]._structures[0].DefaultMaterials[0].SetColors(Top,Middle,Bottom,Spec);
        public static void ChangeSlimeColors(this SlimeDefinition slimedef, Color32 Top, Color32 Middle, Color32 Bottom)
            => slimedef.AppearancesDefault[0]._structures[0].DefaultMaterials[0].SetColors(Top,Middle,Bottom);

        public static void SetSlimeColors(this SlimeDefinition slimedef, Color32 Top, Color32 Middle, Color32 Bottom, Color32 Spec, int index, int index2, bool isSS, int structure)
        {
            Material mat = null;
            if (isSS)
                mat = slimedef.AppearancesDynamic.ToArray()[index]._structures[structure].DefaultMaterials[index2];
            else
                mat = slimedef.AppearancesDefault[index]._structures[structure].DefaultMaterials[index2];
            
            mat.SetColor("_TopColor", Top);
            mat.SetColor("_MiddleColor", Middle);
            mat.SetColor("_BottomColor", Bottom);
            mat.SetColor("_SpecColor", Spec);
        } 

        public static void MakeSellable(this IdentifiableType ident, float marketValue, float marketSaturation, bool hideInMarket=false)
        {
            if (marketData.ContainsKey(ident))
            { MelonLogger.Error("Failed to make object sellable: The object is already sellable"); return; }
            
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
        
        public static Il2CppArrayBase<IdentifiableType> GetAllMembersArray(this IdentifiableTypeGroup group) =>
            group.GetAllMembers().ToArray();
        public static SlimeDefinition GetSlime(string name)
        {
            foreach (IdentifiableType type in slimes.GetAllMembersArray())
                if (type.name.ToUpper() == name.ToUpper())
                    return type.Cast<SlimeDefinition>();
            return null;
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
        public static IdentifiableType GetPlort(string name)
        {
            foreach (IdentifiableType type in plorts.GetAllMembersArray())
                if (type.name.ToUpper() == name.ToUpper())
                    return type;
            return null;
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
        internal static void INTERNAL_SetupSaveForIdent(string RefID, IdentifiableType ident)
        {
            ident.referenceId = RefID;
            GameContext.Instance.AutoSaveDirector.SavedGame.identifiableTypeLookup.Add(RefID, ident);
            GameContext.Instance.AutoSaveDirector.identifiableTypes.memberTypes.Add(ident);
            GameContext.Instance.AutoSaveDirector.SavedGame.identifiableTypeToPersistenceId._primaryIndex.AddItem(RefID);
            GameContext.Instance.AutoSaveDirector.SavedGame.identifiableTypeToPersistenceId._reverseIndex.Add(RefID, GameContext.Instance.AutoSaveDirector.SavedGame.identifiableTypeToPersistenceId._reverseIndex.Count);
        }

        
        
        public static Sprite ConvertToSprite(this Texture2D texture) => Sprite.Create(texture, new Rect(0f, 0f, (float)texture.width, (float)texture.height), new Vector2(0.5f, 0.5f), 1f);
        
        public static GameObject CopyObject(this GameObject obj) => Object.Instantiate(obj, rootOBJ.transform);

            
        /*
        public static void AddRange<T>(this Il2CppReferenceArray<T> array, Il2CppReferenceArray<T> obj) where T : Il2CppObjectBase
        {
            var s = new T[0];
            array = HarmonyLib.CollectionExtensions.AddRangeToArray(s, obj);
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
                if (!list.Contains(iobj))
                    list.Add(iobj);
        }
        */
        

        public static void MakePrefab(this GameObject obj)
        {
            UnityEngine.Object.DontDestroyOnLoad(obj);
            obj.transform.parent = rootOBJ.transform;
        }
        public static GameObject? Get(string name) => Get<GameObject>(name);
        public static GameV04? Save 
        {
            get { return gameContext.AutoSaveDirector.SavedGame.gameState; }
            set { gameContext.AutoSaveDirector.SavedGame.gameState = value; }
        }
    }
}
