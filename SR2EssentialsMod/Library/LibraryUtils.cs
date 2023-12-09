using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppInterop.Runtime.InteropTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Il2CppMonomiPark.SlimeRancher.UI;

namespace SR2E.Library
{
    public static class LibraryUtils
    {
        public static IdentifiableTypeGroup? slimes;
        public static IdentifiableTypeGroup? largos;
        public static IdentifiableTypeGroup? baseSlimes;
        public static IdentifiableTypeGroup? food;
        public static IdentifiableTypeGroup? meat;
        public static IdentifiableTypeGroup? veggies;
        public static IdentifiableTypeGroup? fruit;
        public static GameObject? player;
        public static SystemContext systemContext { get { return SystemContext.Instance; } }
        public static GameContext gameContext { get { return GameContext.Instance; } }
        public static SceneContext sceneContext { get { return SceneContext.Instance; } }
        public enum VanillaPediaEntryCategories { TUTORIAL, SLIMES, RESOURCES, WORLD, RANCH, SCIENCE }
        internal static Dictionary<IdentifiableType, ModdedMarketData> marketData = new Dictionary<IdentifiableType, ModdedMarketData>(0);
        internal static List<MarketUI.PlortEntry> marketPlortEntries = new List<MarketUI.PlortEntry>(0);
        internal static GameObject rootOBJ;
        internal static Dictionary<SlimeDefinition, SR2EMod.LargoSettings>? LargoData = new Dictionary<SlimeDefinition, SR2EMod.LargoSettings>(0);
        public static IdentifiableType GetIdent(this GameObject obj)
        {
            var comp = obj.GetComponent<IdentifiableActor>();

            if (comp != null)
            {
                return comp.identType;
            }
            else { return null; }
        }
        public static SlimeDiet.EatMapEntry CreateEatmap(this SlimeEmotions.Emotion driver, float mindrive, IdentifiableType produce, IdentifiableType eat, IdentifiableType becomes)
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
        public static SlimeDiet.EatMapEntry CreateEatmap(this SlimeEmotions.Emotion driver, float mindrive, IdentifiableType produce, IdentifiableType eat)
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
                slimedef.Diet.AdditionalFoodIdents.Add(ident);
            }
        public static void SetFavoriteProduceCount(this SlimeDefinition slimedef, int count)
            {
                slimedef.Diet.FavoriteProductionCount = count;
            }
        public static void AddEatmapToSlime(this SlimeDefinition slimedef, SlimeDiet.EatMapEntry eatmap)
            {
                slimedef.Diet.EatMap.Add(eatmap);
            }
        public static void SetStructColor(this SlimeAppearanceStructure structure, int id, Color color)
            {
                structure.DefaultMaterials[0].SetColor(id, color);
            }
        public static void RefreshEatmaps(this SlimeDefinition def)
            {
                def.Diet.RefreshEatMap(SR2EMod.slimeDefinitions, def);
            }
        public static void SetObjectIdent(GameObject obj, IdentifiableType ident)
            {
                obj.GetComponent<IdentifiableActor>().identType = ident;
            }
        public static void ChangeSlimeFoodGroup(this SlimeDefinition def, SlimeEat.FoodGroup FG, int index)
            {
                def.Diet.MajorFoodGroups[index] = FG;
            }
        public static void AddSlimeFoodGroup(this SlimeDefinition def, SlimeEat.FoodGroup FG)
            {
                def.Diet.MajorFoodGroups.AddItem<SlimeEat.FoodGroup>(FG);
            }
        public static GameObject SpawnActor(GameObject obj, Vector3 pos)
            {
                return SRBehaviour.InstantiateActor(obj,
                    SRSingleton<SceneContext>.Instance.RegionRegistry.CurrentSceneGroup, pos, Quaternion.identity,
                    false, SlimeAppearance.AppearanceSaveSet.NONE, SlimeAppearance.AppearanceSaveSet.NONE);
            }
        public static GameObject SpawnDynamic(GameObject obj, Vector3 pos)
            {
                return SRBehaviour.InstantiateDynamic(obj, pos, Quaternion.identity, false);
            }
        public static T? Get<T>(string name) where T : Object { return Resources.FindObjectsOfTypeAll<T>().FirstOrDefault((T x) => x.name == name); }
        public static void AddToGroup(IdentifiableType type, string groupName)
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

        public static void SetObjectPrefab(IdentifiableType Object, GameObject prefab)
        {
            Object.prefab = prefab;
        }
        public static void SetObjectIdent(IdentifiableType Object, GameObject prefab)
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
            GameContext.Instance.AutoSaveDirector.SavedGame.identifiableTypeLookup.Add(RefID, ident);
            GameContext.Instance.AutoSaveDirector.identifiableTypes.memberTypes.Add(ident);
            GameContext.Instance.AutoSaveDirector.SavedGame.identifiableTypeToPersistenceId._primaryIndex.AddString(RefID);
            GameContext.Instance.AutoSaveDirector.SavedGame.identifiableTypeToPersistenceId._reverseIndex.Add(RefID, GameContext.Instance.AutoSaveDirector.SavedGame.identifiableTypeToPersistenceId._reverseIndex.Count);
        }
        public static void SetPlortColor(Color32 Top, Color32 Middle, Color32 Bottom, GameObject Prefab)
        {
            var material = Prefab.GetComponent<MeshRenderer>().material;
            material.SetColor("_TopColor", Top);
            material.SetColor("_MiddleColor", Middle);
            material.SetColor("_BottomColor", Bottom);
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
        public static void SetSlimeColor(Color32 Top, Color32 Middle, Color32 Bottom, Color32 Spec, SlimeDefinition slimedef, int index, int index2, bool isSS, int structure)
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
        
        public static Sprite ConvertToSprite(Texture2D texture)
        {
            return Sprite.Create(texture, new Rect(0f, 0f, (float)texture.width, (float)texture.height), new Vector2(0.5f, 0.5f), 1f);
        }
        public static GameObject CopyObject(GameObject obj) => Object.Instantiate(obj, rootOBJ.transform);

            
        
        public static void Add<T>(this Il2CppReferenceArray<T> array, T obj) where T : Il2CppObjectBase
        {
            var s = new T[0];
            array = HarmonyLib.CollectionExtensions.AddToArray(s, obj);
        }
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
            {
                if (!list.Contains(iobj))
                {
                    list.Add(iobj);
                }
            }
        }
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
    }
}
