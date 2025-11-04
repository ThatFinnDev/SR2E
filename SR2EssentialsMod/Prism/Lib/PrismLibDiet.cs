using Cotton;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppMonomiPark.SlimeRancher.Slime;
using SR2E.Cotton;
using SR2E.Prism.Data;

namespace SR2E.Prism.Lib;

public static class PrismLibDiet
{
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


    
    public static void AddEatmapToSlime(this PrismSlime prismSlime, SlimeDiet.EatMapEntry eatmap)
    {
        if (!CottonSlimes.customEatmaps.TryGetValue(prismSlime, out var eatmaps))
        {
            eatmaps = new List<SlimeDiet.EatMapEntry>();
            CottonSlimes.customEatmaps.Add(prismSlime, eatmaps);
        }
        eatmaps.Add(eatmap);
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
        BecomesOnTarrifyIdentifiableType = Get<IdentifiableType>("Tarr"),
        EdiblePlortIdentifiableTypeGroup = Get<IdentifiableTypeGroup>("EdiblePlortFoodGroup"),
        StableResourceIdentifiableTypeGroup = Get<IdentifiableTypeGroup>("StableResourcesGroup"),
        UnstableResourceIdentifiableTypeGroup = Get<IdentifiableTypeGroup>("UnstableResourcesGroup"),
        UnstablePlort = CottonLibrary.Actors.GetPlort("UnstablePlort")
    };
}