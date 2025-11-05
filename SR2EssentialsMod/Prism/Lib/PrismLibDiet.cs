using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppMonomiPark.SlimeRancher.Slime;
using SR2E.Prism.Data;

namespace SR2E.Prism.Lib;

public static class PrismLibDiet
{    
    internal static Dictionary<SlimeDefinition, Dictionary<SlimeDiet.EatMapEntry, bool>> customEatmaps = new ();

    public static SlimeDiet.EatMapEntry CreateEatmapEntry(SlimeEmotions.Emotion driver, float minDrive, IdentifiableType produce, IdentifiableType eat, IdentifiableType becomes)
        => new SlimeDiet.EatMapEntry
        {
            EatsIdent = eat,
            ProducesIdent = produce,
            BecomesIdent = becomes,
            Driver = driver,
            MinDrive = minDrive
        };

    public static SlimeDiet.EatMapEntry CreateEatmapEntry(SlimeEmotions.Emotion driver, float mindrive, IdentifiableType produce, IdentifiableType eat) 
        => new SlimeDiet.EatMapEntry
        {
            EatsIdent = eat,
            ProducesIdent = produce,
            Driver = driver,
            MinDrive = mindrive
        };


   internal static bool _CarefulCheck(Il2CppSystem.Collections.Generic.List<SlimeDiet.EatMapEntry> list,
        SlimeDiet.EatMapEntry eatmap)
   {
       if (list == null) return false;
       if (eatmap == null) return false;
       foreach (var entry in list)
       {
           if (entry.BecomesIdent != entry.BecomesIdent) continue;
           if (entry.EatsIdent != entry.EatsIdent) continue;
           if (entry.Driver != entry.Driver) continue;
           if (entry.MinDrive != entry.MinDrive) continue;
           if (entry.ProducesIdent != entry.ProducesIdent) continue;
           return false;
       } 
       return true;
    }
    public static void AddEatmapToSlime(this PrismSlime prismSlime, SlimeDiet.EatMapEntry eatmap, bool beCareful = false)
    {
        var eatmaps = new Dictionary<SlimeDiet.EatMapEntry,bool>();
        if (customEatmaps.TryGetValue(prismSlime, out var dict))
            eatmaps = dict;
        else customEatmaps.Add(prismSlime, eatmaps);
        eatmaps.Add(eatmap,beCareful);
        if(beCareful)
            if(!_CarefulCheck(prismSlime.GetSlimeDiet().EatMap,eatmap)) 
                return;
        prismSlime.GetSlimeDiet().EatMap.Add(eatmap);
    }


    public static void RefreshEatMap(this PrismSlime prismSlime)
    {
        prismSlime.GetSlimeDiet().RefreshEatMap(gameContext.SlimeDefinitions, prismSlime);
    }

    
    
    
    //Produce
    public static void AddEatProduction(this PrismSlime prismSlime, IdentifiableType identifiableType)
    {
        prismSlime.GetSlimeDiet().ProduceIdents = prismSlime.GetSlimeDiet().ProduceIdents.AddToNew(identifiableType);
    }
    public static void SetEatProduction(this PrismSlime prismSlime, IdentifiableType identifiableType, int index)
    {
        prismSlime.GetSlimeDiet().ProduceIdents[index] = identifiableType;
    }
    public static void RemoveEatProduction(this PrismSlime prismSlime, IdentifiableType identifiableType, int index)
    {
        prismSlime.GetSlimeDiet().ProduceIdents = prismSlime.GetSlimeDiet().ProduceIdents.RemoveToNew(identifiableType);
    }

    
    //Favourite Food
    public static void AddFavoriteFood(this PrismSlime prismSlime, IdentifiableType identifiableType)
    {
        prismSlime.GetSlimeDiet().FavoriteIdents = prismSlime.GetSlimeDiet().FavoriteIdents.AddToNew(identifiableType);
    }
    public static void SetFavoriteFood(this PrismSlime prismSlime, IdentifiableType identifiableType, int index)
    {
        prismSlime.GetSlimeDiet().FavoriteIdents[index] = identifiableType;
    }
    public static void RemoveFavoriteFood(this PrismSlime prismSlime, IdentifiableType identifiableType)
    {
        prismSlime.GetSlimeDiet().FavoriteIdents = prismSlime.GetSlimeDiet().FavoriteIdents.RemoveToNew(identifiableType);
    }
    public static void SetFavoriteFoodProductionCount(this PrismSlime prismSlime, int count)
    {
        prismSlime.GetSlimeDiet().FavoriteProductionCount = count;
    }
    
    
    //Additional Food
    public static void AddAdditionalFood(this PrismSlime prismSlime, IdentifiableType identifiableType)
    {
        prismSlime.GetSlimeDiet().AdditionalFoodIdents = prismSlime.GetSlimeDiet().AdditionalFoodIdents.AddToNew(identifiableType);
    }
    public static void SetAdditionalFood(this PrismSlime prismSlime, IdentifiableType identifiableType, int index)
    {
        prismSlime.GetSlimeDiet().AdditionalFoodIdents[index] = identifiableType;
    }
    public static void RemoveAdditionalFood(this PrismSlime prismSlime, IdentifiableType identifiableType)
    {
        prismSlime.GetSlimeDiet().AdditionalFoodIdents = prismSlime.GetSlimeDiet().AdditionalFoodIdents.RemoveToNew(identifiableType);
    }


    //Major Food Groups
    public static void AddFoodGroup(this PrismSlime prismSlime, IdentifiableTypeGroup identifiableTypeGroup)
    {
        prismSlime.GetSlimeDiet().MajorFoodIdentifiableTypeGroups = prismSlime.GetSlimeDiet().MajorFoodIdentifiableTypeGroups.AddToNew(identifiableTypeGroup);
    }
    public static void SetFoodGroup(this PrismSlime prismSlime, IdentifiableTypeGroup identifiableTypeGroup, int index)
    {
        prismSlime.GetSlimeDiet().MajorFoodIdentifiableTypeGroups[index] = identifiableTypeGroup;
    }
    public static void RemoveFoodGroup(this PrismSlime prismSlime, IdentifiableTypeGroup identifiableTypeGroup)
    {
        prismSlime.GetSlimeDiet().MajorFoodIdentifiableTypeGroups = prismSlime.GetSlimeDiet().MajorFoodIdentifiableTypeGroups.RemoveToNew(identifiableTypeGroup);
    }

    
    

    internal static SlimeDiet CreateNewDiet() => new SlimeDiet
    {
        ProduceIdents = new Il2CppReferenceArray<IdentifiableType>(0),
        FavoriteProductionCount = 2,
        EatMap = new Il2CppSystem.Collections.Generic.List<SlimeDiet.EatMapEntry>(0),
        FavoriteIdents = new Il2CppReferenceArray<IdentifiableType>(0),
        AdditionalFoodIdents = new Il2CppReferenceArray<IdentifiableType>(0),
        MajorFoodIdentifiableTypeGroups = new Il2CppReferenceArray<IdentifiableTypeGroup>(0),
        BecomesOnTarrifyIdentifiableType = PrismNativeBaseSlime.Tarr.GetPrismBaseSlime(),
        EdiblePlortIdentifiableTypeGroup = LookupEUtil.allIdentifiableTypeGroups["EdiblePlortFoodGroup"],
        StableResourceIdentifiableTypeGroup = LookupEUtil.allIdentifiableTypeGroups["StableResourcesGroup"],
        UnstableResourceIdentifiableTypeGroup = LookupEUtil.allIdentifiableTypeGroups["UnstableResourcesGroup"],
        UnstablePlort = PrismNativePlort.Unstable.GetPrismPlort()
    };
}