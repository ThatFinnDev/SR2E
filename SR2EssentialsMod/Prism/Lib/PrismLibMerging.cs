using Cotton;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using SR2E.Prism.Data;
using SR2E.Prism.Enums;

namespace SR2E.Prism.Lib;

public static class PrismLibMerging
{
    
    public static SlimeDiet MergeDiet(SlimeDiet firstDiet, SlimeDiet secondDiet)
    {
        var mergedDiet = PrismLibDiet.CreateNewDiet();

        mergedDiet.EatMap=mergedDiet.EatMap.AddRangeNoMultipleToNew(firstDiet.EatMap);
        mergedDiet.EatMap=mergedDiet.EatMap.AddRangeNoMultipleToNew(secondDiet.EatMap);

        mergedDiet.AdditionalFoodIdents =
            mergedDiet.AdditionalFoodIdents.AddRangeNoMultipleToNew(firstDiet.AdditionalFoodIdents);
        mergedDiet.AdditionalFoodIdents =
            mergedDiet.AdditionalFoodIdents.AddRangeNoMultipleToNew(secondDiet.AdditionalFoodIdents);

        mergedDiet.FavoriteIdents = mergedDiet.FavoriteIdents.AddRangeNoMultipleToNew(firstDiet.FavoriteIdents);
        mergedDiet.FavoriteIdents = mergedDiet.FavoriteIdents.AddRangeNoMultipleToNew(secondDiet.FavoriteIdents);

        mergedDiet.MajorFoodIdentifiableTypeGroups =
            mergedDiet.MajorFoodIdentifiableTypeGroups.AddRangeNoMultipleToNew(firstDiet.MajorFoodIdentifiableTypeGroups);
        mergedDiet.MajorFoodIdentifiableTypeGroups =
            mergedDiet.MajorFoodIdentifiableTypeGroups.AddRangeNoMultipleToNew(secondDiet.MajorFoodIdentifiableTypeGroups);

        mergedDiet.ProduceIdents = mergedDiet.ProduceIdents.AddRangeNoMultipleToNew(firstDiet.ProduceIdents);
        mergedDiet.ProduceIdents = mergedDiet.ProduceIdents.AddRangeNoMultipleToNew(secondDiet.ProduceIdents);

        return mergedDiet;
    }
    /// <summary>
    /// Merges the components on two GameObjects.
    /// </summary>
    /// <param name="obj">The target GameObject.</param>
    /// <param name="oldOne">The first GameObject to get components from.</param>
    /// <param name="oldTwo">The other GameObject to get components from, optional.</param>
    public static void MergeComponents(GameObject obj, GameObject oldOne, GameObject? oldTwo = null)
    {
        Dictionary<string, (MonoBehaviour, bool)> components = new Dictionary<string, (MonoBehaviour, bool)>();
        
        foreach (var comp in obj.GetComponents<MonoBehaviour>())
            components.TryAdd(comp.GetIl2CppType().Name,(comp,false));
        
        foreach (var comp in oldOne.GetComponents<MonoBehaviour>())
            components.TryAdd(comp.GetIl2CppType().Name,(comp,true));
        
        if (oldTwo)
            foreach (var comp in oldTwo.GetComponents<MonoBehaviour>())
                components.TryAdd(comp.GetIl2CppType().Name,(comp,true));

        foreach (var component in components)
            if (component.Value.Item2)
                if (!component.Key.ContainsAny("AweTowardsLargos", "SlimeEyeComponents", "SlimeMouthComponents", "ColliderTotemLinkerHelper"))
                {
                    if (!obj.GetComponent(component.Value.Item1.GetIl2CppType()))
                    {
                        var newComp = obj.AddComponent(component.Value.Item1.GetIl2CppType());
                        newComp.CopyFields(component.Value.Item1);
                    }
                    //MelonLogger.Msg(component.Key);
                }
        
        //MelonLogger.Msg("Merged components!");
    }
    /// <summary>
    /// Merges two slime's favorite toys list. Use for largos
    /// </summary>
    /// <param name="slimeOne">The first slime</param>
    /// <param name="slimeTwo">The other slime</param>
    /// <returns>An IL2Cpp array of ToyDefinition</returns>
    public static Il2CppReferenceArray<ToyDefinition> MergeFavoriteToys(PrismSlime slimeOne, PrismSlime slimeTwo)
    {
        List<ToyDefinition> toys = new List<ToyDefinition>();

        foreach (var toy in slimeOne.GetSlimeDefinition().FavoriteToyIdents)
            toys.Add(toy);
        foreach (var toy in slimeTwo.GetSlimeDefinition().FavoriteToyIdents)
            toys.Add(toy);

        return toys.ToArray();
    }
    
    /// <summary>
    /// Merges two appearance structures together for a largo.
    /// </summary>
    /// <param name="slime1">Base slime #1</param>
    /// <param name="slime2">Base slime #2</param>
    /// <param name="settings">Settings that determine what structures this function merges, and what colors.</param>
    /// <returns>A new array of structures.</returns>
    public static Il2CppReferenceArray<SlimeAppearanceStructure> MergeStructures(SlimeAppearance slime1, SlimeAppearance slime2, PrismLargoMergeSettings settings)
    {
        var newStructures = new List<SlimeAppearanceStructure>(0);
        SlimeAppearance.Palette firstColor = slime1._colorPalette;
        SlimeAppearance.Palette firstColorTwin = slime1.INTERNAL_GetTwinPalette();
        SlimeAppearance.Palette firstColorSloomber = slime1.INTERNAL_GetSloomberPalette();

        SlimeAppearance.Palette secondColor = slime2._colorPalette;
        SlimeAppearance.Palette secondColorTwin = slime2.INTERNAL_GetTwinPalette();
        SlimeAppearance.Palette secondColorSloomber = slime2.INTERNAL_GetSloomberPalette();


        bool useTwinShader = CottonSlimes.INTERNAL_GetLargoHasTwinEffect(slime1, slime2);

        bool useSloomberShader = CottonSlimes.INTERNAL_GetLargoHasSloomberEffect(slime1, slime2);
        Material sloomberMat = CottonSlimes.GetSlime("Sloomber").AppearancesDefault[0]._structures[0].DefaultMaterials[0];

        foreach (var structure in slime1.Structures)
        {
            if (structure.Element.Type == SlimeAppearanceElement.ElementType.FACE ||
                structure.Element.Type == SlimeAppearanceElement.ElementType.FACE_ATTACH)
            {
                if (settings.face==PrismTwoMergeStrategy.KeepFirst)
                {
                    if (structure != null && !newStructures.Contains(structure) &&
                        structure.DefaultMaterials.Length != 0)
                    {
                        var newStructure = new SlimeAppearanceStructure(structure);
                        newStructures.Add(newStructure);
                        var mat = Object.Instantiate(structure.DefaultMaterials[0]);
                        newStructure.DefaultMaterials[0] = mat;


                        try
                        {
                            if (settings.baseColors==PrismThreeMergeStrategy.PrioritizeFirst)
                            {
                                mat.SetColor("_TopColor", firstColor.Top);
                                mat.SetColor("_MiddleColor", firstColor.Middle);
                                mat.SetColor("_BottomColor", firstColor.Bottom);
                                mat.SetColor("_SpecColor", firstColor.Middle);
                            }
                            else if (settings.baseColors==PrismThreeMergeStrategy.PrioritizeSecond)
                            {
                                mat.SetColor("_TopColor", secondColor.Top);
                                mat.SetColor("_MiddleColor", secondColor.Middle);
                                mat.SetColor("_BottomColor", secondColor.Bottom);
                                mat.SetColor("_SpecColor", secondColor.Middle);
                            }
                            else if (settings.baseColors==PrismThreeMergeStrategy.Merge)
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
                        catch
                        {
                        }
                    }
                }
            }
            else if (structure.Element.Type == SlimeAppearanceElement.ElementType.BODY)
            {
                if (settings.body==PrismTwoMergeStrategy.KeepFirst)
                {
                    if (structure != null && !newStructures.Contains(structure) &&
                        structure.DefaultMaterials.Length != 0)
                    {
                        var newStructure = new SlimeAppearanceStructure(structure);
                        newStructures.Add(newStructure);
                        var mat = Object.Instantiate(structure.DefaultMaterials[0]);
                        newStructure.DefaultMaterials[0] = mat;

                        try
                        {
                            if (settings.baseColors==PrismThreeMergeStrategy.PrioritizeFirst)
                            {
                                mat.SetColor("_TopColor", firstColor.Top);
                                mat.SetColor("_MiddleColor", firstColor.Middle);
                                mat.SetColor("_BottomColor", firstColor.Bottom);
                                mat.SetColor("_SpecColor", firstColor.Middle);
                            }
                            else if (settings.baseColors==PrismThreeMergeStrategy.PrioritizeSecond)
                            {
                                mat.SetColor("_TopColor", secondColor.Top);
                                mat.SetColor("_MiddleColor", secondColor.Middle);
                                mat.SetColor("_BottomColor", secondColor.Bottom);
                                mat.SetColor("_SpecColor", secondColor.Middle);
                            }
                            else if (settings.baseColors==PrismThreeMergeStrategy.Merge)
                            {
                                var top = Color.Lerp(firstColor.Top, secondColor.Top, 0.5f);
                                var middle = Color.Lerp(firstColor.Middle, secondColor.Middle, 0.5f);
                                var bottom = Color.Lerp(firstColor.Bottom, secondColor.Bottom, 0.5f);
                                mat.SetColor("_TopColor", top);
                                mat.SetColor("_MiddleColor", middle);
                                mat.SetColor("_BottomColor", bottom);
                                mat.SetColor("_SpecColor", middle);
                            }

                            
                            // 0.6 - Twin material
                            if (useTwinShader)
                            {
                                mat.EnableKeyword("_ENABLETWINEFFECT_ON");

                                if (settings.twinColors==PrismThreeMergeStrategy.PrioritizeFirst)
                                {
                                    mat.SetColor("_TwinTopColor", firstColorTwin.Top);
                                    mat.SetColor("_TwinMiddleColor", firstColorTwin.Middle);
                                    mat.SetColor("_TwinBottomColor", firstColor.Bottom);
                                }
                                else if (settings.twinColors==PrismThreeMergeStrategy.PrioritizeSecond)
                                {
                                    mat.SetColor("_TwinTopColor", secondColorTwin.Top);
                                    mat.SetColor("_TwinMiddleColor", secondColorTwin.Middle);
                                    mat.SetColor("_TwinBottomColor", secondColorTwin.Bottom);
                                }
                                else if (settings.twinColors==PrismThreeMergeStrategy.Merge)
                                {
                                    var top = Color.Lerp(firstColorTwin.Top, secondColorTwin.Top, 0.5f);
                                    var middle = Color.Lerp(firstColorTwin.Middle, secondColorTwin.Middle, 0.5f);
                                    var bottom = Color.Lerp(firstColorTwin.Bottom, secondColorTwin.Bottom, 0.5f);
                                    mat.SetColor("_TwinTopColor", top);
                                    mat.SetColor("_TwinMiddleColor", middle);
                                    mat.SetColor("_TwinBottomColor", bottom);
                                }

                                if (useSloomberShader)
                                {
                                    mat.SetTexture("_SloomberColorOverlay", sloomberMat.GetTexture("_SloomberColorOverlay"));
                                    mat.SetTexture("_SloomberStarMask", sloomberMat.GetTexture("_SloomberStarMask"));

                                    mat.EnableKeyword("_BODYCOLORING_SLOOMBER");
                                    mat.DisableKeyword("_BODYCOLORING_DEFAULT");

                                    if (settings.sloomberColors==PrismThreeMergeStrategy.PrioritizeFirst)
                                    {
                                        mat.SetColor("_SloomberTopColor", firstColorSloomber.Top);
                                        mat.SetColor("_SloomberMiddleColor", firstColorSloomber.Middle);
                                        mat.SetColor("_SloomberBottomColor", firstColorSloomber.Bottom);
                                    }
                                    else if (settings.sloomberColors==PrismThreeMergeStrategy.PrioritizeSecond)
                                    {
                                        mat.SetColor("_SloomberTopColor", secondColorSloomber.Top);
                                        mat.SetColor("_SloomberMiddleColor", secondColorSloomber.Middle);
                                        mat.SetColor("_SloomberBottomColor", secondColorSloomber.Bottom);
                                    }
                                    else if (settings.sloomberColors==PrismThreeMergeStrategy.Merge)
                                    {
                                        var top = Color.Lerp(firstColorSloomber.Top, secondColorSloomber.Top, 0.5f);
                                        var middle = Color.Lerp(firstColorSloomber.Middle, secondColorSloomber.Middle, 0.5f);
                                        var bottom = Color.Lerp(firstColorSloomber.Bottom, secondColorSloomber.Bottom, 0.5f);
                                        mat.SetColor("_SloomberTopColor", top);
                                        mat.SetColor("_SloomberMiddleColor", middle);
                                        mat.SetColor("_SloomberBottomColor", bottom);
                                    }
                                }
                            }
                        }
                        catch
                        {
                        }
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
                    if (settings.baseColors==PrismThreeMergeStrategy.PrioritizeFirst)
                    {
                        mat.SetColor("_TopColor", firstColor.Top);
                        mat.SetColor("_MiddleColor", firstColor.Middle);
                        mat.SetColor("_BottomColor", firstColor.Bottom);
                        mat.SetColor("_SpecColor", firstColor.Middle);
                    }
                    else if (settings.baseColors==PrismThreeMergeStrategy.PrioritizeSecond)
                    {
                        mat.SetColor("_TopColor", secondColor.Top);
                        mat.SetColor("_MiddleColor", secondColor.Middle);
                        mat.SetColor("_BottomColor", secondColor.Bottom);
                        mat.SetColor("_SpecColor", secondColor.Middle);
                    }
                    else if (settings.baseColors==PrismThreeMergeStrategy.Merge)
                    {
                        var top = Color.Lerp(firstColor.Top, secondColor.Top, 0.5f);
                        var middle = Color.Lerp(firstColor.Middle, secondColor.Middle, 0.5f);
                        var bottom = Color.Lerp(firstColor.Bottom, secondColor.Bottom, 0.5f);
                        mat.SetColor("_TopColor", top);
                        mat.SetColor("_MiddleColor", middle);
                        mat.SetColor("_BottomColor", bottom);
                        mat.SetColor("_SpecColor", middle);
                    }

                    if (useSloomberShader)
                    {
                        mat.SetTexture("_SloomberColorOverlay", sloomberMat.GetTexture("_SloomberColorOverlay"));
                        mat.SetTexture("_SloomberStarMask", sloomberMat.GetTexture("_SloomberStarMask"));

                        mat.EnableKeyword("_BODYCOLORING_SLOOMBER");
                        mat.DisableKeyword("_BODYCOLORING_DEFAULT");

                        if (settings.sloomberColors==PrismThreeMergeStrategy.PrioritizeFirst)
                        {
                            mat.SetColor("_SloomberTopColor", firstColorSloomber.Top);
                            mat.SetColor("_SloomberMiddleColor", firstColorSloomber.Middle);
                            mat.SetColor("_SloomberBottomColor", firstColorSloomber.Bottom);
                        }
                        else if (settings.sloomberColors==PrismThreeMergeStrategy.PrioritizeSecond)
                        {
                            mat.SetColor("_SloomberTopColor", secondColorSloomber.Top);
                            mat.SetColor("_SloomberMiddleColor", secondColorSloomber.Middle);
                            mat.SetColor("_SloomberBottomColor", secondColorSloomber.Bottom);
                        }
                        else if (settings.sloomberColors==PrismThreeMergeStrategy.Merge)
                        {
                            var top = Color.Lerp(firstColorSloomber.Top, secondColorSloomber.Top, 0.5f);
                            var middle = Color.Lerp(firstColorSloomber.Middle, secondColorSloomber.Middle, 0.5f);
                            var bottom = Color.Lerp(firstColorSloomber.Bottom, secondColorSloomber.Bottom, 0.5f);
                            mat.SetColor("_SloomberTopColor", top);
                            mat.SetColor("_SloomberMiddleColor", middle);
                            mat.SetColor("_SloomberBottomColor", bottom);
                        }
                    }

                    if (useTwinShader)
                    {
                        mat.EnableKeyword("_ENABLETWINEFFECT_ON");

                        if (settings.twinColors==PrismThreeMergeStrategy.PrioritizeFirst)
                        {
                            mat.SetColor("_TwinTopColor", firstColorTwin.Top);
                            mat.SetColor("_TwinMiddleColor", firstColorTwin.Middle);
                            mat.SetColor("_TwinBottomColor", firstColorTwin.Bottom);
                        }
                        else if (settings.twinColors==PrismThreeMergeStrategy.PrioritizeSecond)
                        {
                            mat.SetColor("_TwinTopColor", secondColorTwin.Top);
                            mat.SetColor("_TwinMiddleColor", secondColorTwin.Middle);
                            mat.SetColor("_TwinBottomColor", secondColorTwin.Bottom);
                        }
                        else if (settings.twinColors==PrismThreeMergeStrategy.Merge)
                        {
                            var top = Color.Lerp(firstColorTwin.Top, secondColorTwin.Top, 0.5f);
                            var middle = Color.Lerp(firstColorTwin.Middle, secondColorTwin.Middle, 0.5f);
                            var bottom = Color.Lerp(firstColorTwin.Bottom, secondColorTwin.Bottom, 0.5f);
                            mat.SetColor("_TwinTopColor", top);
                            mat.SetColor("_TwinMiddleColor", middle);
                            mat.SetColor("_TwinBottomColor", bottom);
                        }
                    }
                }
                catch
                {
                }
            }
        }

        foreach (var structure in slime2.Structures)
        {
            if (structure.Element.Type == SlimeAppearanceElement.ElementType.FACE ||
                structure.Element.Type == SlimeAppearanceElement.ElementType.FACE_ATTACH)
            {
                if (settings.face==PrismTwoMergeStrategy.KeepSecond)
                {
                    if (structure != null && !newStructures.Contains(structure) &&
                        structure.DefaultMaterials.Length != 0)
                    {
                        var newStructure = new SlimeAppearanceStructure(structure);
                        newStructures.Add(newStructure);
                        var mat = Object.Instantiate(structure.DefaultMaterials[0]);
                        newStructure.DefaultMaterials[0] = mat;


                        try
                        {
                            if (settings.baseColors==PrismThreeMergeStrategy.PrioritizeFirst)
                            {
                                mat.SetColor("_TopColor", firstColor.Top);
                                mat.SetColor("_MiddleColor", firstColor.Middle);
                                mat.SetColor("_BottomColor", firstColor.Bottom);
                                mat.SetColor("_SpecColor", firstColor.Middle);
                            }
                            else if (settings.baseColors==PrismThreeMergeStrategy.PrioritizeSecond)
                            {
                                mat.SetColor("_TopColor", secondColor.Top);
                                mat.SetColor("_MiddleColor", secondColor.Middle);
                                mat.SetColor("_BottomColor", secondColor.Bottom);
                                mat.SetColor("_SpecColor", secondColor.Middle);
                            }
                            else if (settings.baseColors==PrismThreeMergeStrategy.Merge)
                            {
                                var top = Color.Lerp(firstColor.Top, secondColor.Top, 0.5f);
                                var middle = Color.Lerp(firstColor.Middle, secondColor.Middle, 0.5f);
                                var bottom = Color.Lerp(firstColor.Bottom, secondColor.Bottom, 0.5f);
                                mat.SetColor("_TopColor", top);
                                mat.SetColor("_MiddleColor", middle);
                                mat.SetColor("_BottomColor", bottom);
                                mat.SetColor("_SpecColor", middle);
                            }

                            if (useSloomberShader)
                            {
                                mat.SetTexture("_SloomberColorOverlay",
                                    sloomberMat.GetTexture("_SloomberColorOverlay"));
                                mat.SetTexture("_SloomberStarMask", sloomberMat.GetTexture("_SloomberStarMask"));

                                mat.EnableKeyword("_BODYCOLORING_SLOOMBER");
                                mat.DisableKeyword("_BODYCOLORING_DEFAULT");

                                if (settings.sloomberColors==PrismThreeMergeStrategy.PrioritizeFirst)
                                {
                                    mat.SetColor("_SloomberTopColor", firstColorSloomber.Top);
                                    mat.SetColor("_SloomberMiddleColor", firstColorSloomber.Middle);
                                    mat.SetColor("_SloomberBottomColor", firstColorSloomber.Bottom);
                                }
                                else if (settings.sloomberColors==PrismThreeMergeStrategy.PrioritizeSecond)
                                {
                                    mat.SetColor("_SloomberTopColor", secondColorSloomber.Top);
                                    mat.SetColor("_SloomberMiddleColor", secondColorSloomber.Middle);
                                    mat.SetColor("_SloomberBottomColor", secondColorSloomber.Bottom);
                                }
                                else if (settings.sloomberColors==PrismThreeMergeStrategy.Merge)
                                {
                                    var top = Color.Lerp(firstColorSloomber.Top, secondColorSloomber.Top, 0.5f);
                                    var middle = Color.Lerp(firstColorSloomber.Middle, secondColorSloomber.Middle,
                                        0.5f);
                                    var bottom = Color.Lerp(firstColorSloomber.Bottom, secondColorSloomber.Bottom,
                                        0.5f);
                                    mat.SetColor("_SloomberTopColor", top);
                                    mat.SetColor("_SloomberMiddleColor", middle);
                                    mat.SetColor("_SloomberBottomColor", bottom);
                                }
                            }

                            if (useTwinShader)
                            {
                                mat.EnableKeyword("_ENABLETWINEFFECT_ON");

                                if (settings.twinColors==PrismThreeMergeStrategy.PrioritizeFirst)
                                {
                                    mat.SetColor("_TwinTopColor", firstColorTwin.Top);
                                    mat.SetColor("_TwinMiddleColor", firstColorTwin.Middle);
                                    mat.SetColor("_TwinBottomColor", firstColorTwin.Bottom);
                                }
                                else if (settings.twinColors==PrismThreeMergeStrategy.PrioritizeSecond)
                                {
                                    mat.SetColor("_TwinTopColor", secondColorTwin.Top);
                                    mat.SetColor("_TwinMiddleColor", secondColorTwin.Middle);
                                    mat.SetColor("_TwinBottomColor", secondColorTwin.Bottom);
                                }
                                else if (settings.twinColors==PrismThreeMergeStrategy.Merge)
                                {
                                    var top = Color.Lerp(firstColorTwin.Top, secondColorTwin.Top, 0.5f);
                                    var middle = Color.Lerp(firstColorTwin.Middle, secondColorTwin.Middle, 0.5f);
                                    var bottom = Color.Lerp(firstColorTwin.Bottom, secondColorTwin.Bottom, 0.5f);
                                    mat.SetColor("_TwinTopColor", top);
                                    mat.SetColor("_TwinMiddleColor", middle);
                                    mat.SetColor("_TwinBottomColor", bottom);
                                }
                            }
                        }
                        catch
                        {
                        }
                    }
                }
            }
            else if (structure.Element.Type == SlimeAppearanceElement.ElementType.BODY)
            {
                if (settings.body==PrismTwoMergeStrategy.KeepSecond)
                {
                    if (!newStructures.Contains(structure))
                    {
                        var newStructure = new SlimeAppearanceStructure(structure);
                        newStructures.Add(newStructure);
                        var mat = Object.Instantiate(structure.DefaultMaterials[0]);
                        newStructure.DefaultMaterials[0] = mat;


                        try
                        {
                            if (settings.baseColors==PrismThreeMergeStrategy.PrioritizeFirst)
                            {
                                mat.SetColor("_TopColor", firstColor.Top);
                                mat.SetColor("_MiddleColor", firstColor.Middle);
                                mat.SetColor("_BottomColor", firstColor.Bottom);
                                mat.SetColor("_SpecColor", firstColor.Middle);
                            }
                            else if (settings.baseColors==PrismThreeMergeStrategy.PrioritizeSecond)
                            {
                                mat.SetColor("_TopColor", secondColor.Top);
                                mat.SetColor("_MiddleColor", secondColor.Middle);
                                mat.SetColor("_BottomColor", secondColor.Bottom);
                                mat.SetColor("_SpecColor", secondColor.Middle);
                            }
                            else if (settings.baseColors==PrismThreeMergeStrategy.Merge)
                            {
                                var top = Color.Lerp(firstColor.Top, secondColor.Top, 0.5f);
                                var middle = Color.Lerp(firstColor.Middle, secondColor.Middle, 0.5f);
                                var bottom = Color.Lerp(firstColor.Bottom, secondColor.Bottom, 0.5f);
                                mat.SetColor("_TopColor", top);
                                mat.SetColor("_MiddleColor", middle);
                                mat.SetColor("_BottomColor", bottom);
                                mat.SetColor("_SpecColor", middle);
                            }

                            if (useSloomberShader)
                            {
                                mat.SetTexture("_SloomberColorOverlay",
                                    sloomberMat.GetTexture("_SloomberColorOverlay"));
                                mat.SetTexture("_SloomberStarMask", sloomberMat.GetTexture("_SloomberStarMask"));

                                mat.EnableKeyword("_BODYCOLORING_SLOOMBER");
                                mat.DisableKeyword("_BODYCOLORING_DEFAULT");

                                if (settings.sloomberColors==PrismThreeMergeStrategy.PrioritizeFirst)
                                {
                                    mat.SetColor("_SloomberTopColor", firstColorSloomber.Top);
                                    mat.SetColor("_SloomberMiddleColor", firstColorSloomber.Middle);
                                    mat.SetColor("_SloomberBottomColor", firstColorSloomber.Bottom);
                                }
                                else if (settings.sloomberColors==PrismThreeMergeStrategy.PrioritizeSecond)
                                {
                                    mat.SetColor("_SloomberTopColor", secondColorSloomber.Top);
                                    mat.SetColor("_SloomberMiddleColor", secondColorSloomber.Middle);
                                    mat.SetColor("_SloomberBottomColor", secondColorSloomber.Bottom);
                                }
                                else if (settings.sloomberColors==PrismThreeMergeStrategy.Merge)
                                {
                                    var top = Color.Lerp(firstColorSloomber.Top, secondColorSloomber.Top, 0.5f);
                                    var middle = Color.Lerp(firstColorSloomber.Middle, secondColorSloomber.Middle,
                                        0.5f);
                                    var bottom = Color.Lerp(firstColorSloomber.Bottom, secondColorSloomber.Bottom,
                                        0.5f);
                                    mat.SetColor("_SloomberTopColor", top);
                                    mat.SetColor("_SloomberMiddleColor", middle);
                                    mat.SetColor("_SloomberBottomColor", bottom);
                                }
                            }

                            if (useTwinShader)
                            {
                                mat.EnableKeyword("_ENABLETWINEFFECT_ON");

                                if (settings.twinColors==PrismThreeMergeStrategy.PrioritizeFirst)
                                {
                                    mat.SetColor("_TwinTopColor", firstColorTwin.Top);
                                    mat.SetColor("_TwinMiddleColor", firstColorTwin.Middle);
                                    mat.SetColor("_TwinBottomColor", firstColorTwin.Bottom);
                                }
                                else if (settings.twinColors==PrismThreeMergeStrategy.PrioritizeSecond)
                                {
                                    mat.SetColor("_TwinTopColor", secondColorTwin.Top);
                                    mat.SetColor("_TwinMiddleColor", secondColorTwin.Middle);
                                    mat.SetColor("_TwinBottomColor", secondColorTwin.Bottom);
                                }
                                else if (settings.twinColors==PrismThreeMergeStrategy.Merge)
                                {
                                    var top = Color.Lerp(firstColorTwin.Top, secondColorTwin.Top, 0.5f);
                                    var middle = Color.Lerp(firstColorTwin.Middle, secondColorTwin.Middle, 0.5f);
                                    var bottom = Color.Lerp(firstColorTwin.Bottom, secondColorTwin.Bottom, 0.5f);
                                    mat.SetColor("_TwinTopColor", top);
                                    mat.SetColor("_TwinMiddleColor", middle);
                                    mat.SetColor("_TwinBottomColor", bottom);
                                }
                            }
                        }
                        catch
                        {
                        }
                    }

                }
            }
            else if (structure != null && !newStructures.Contains(structure) &&
                     structure.DefaultMaterials.Length != 0)
            {

                var newStructure = new SlimeAppearanceStructure(structure);
                newStructures.Add(newStructure);
                var mat = Object.Instantiate(structure.DefaultMaterials[0]);
                newStructure.DefaultMaterials[0] = mat;

                try
                {
                    if (settings.baseColors==PrismThreeMergeStrategy.PrioritizeFirst)
                    {
                        mat.SetColor("_TopColor", firstColor.Top);
                        mat.SetColor("_MiddleColor", firstColor.Middle);
                        mat.SetColor("_BottomColor", firstColor.Bottom);
                        mat.SetColor("_SpecColor", firstColor.Middle);
                    }
                    else if (settings.baseColors==PrismThreeMergeStrategy.PrioritizeSecond)
                    {
                        mat.SetColor("_TopColor", secondColor.Top);
                        mat.SetColor("_MiddleColor", secondColor.Middle);
                        mat.SetColor("_BottomColor", secondColor.Bottom);
                        mat.SetColor("_SpecColor", secondColor.Middle);
                    }
                    else if (settings.baseColors==PrismThreeMergeStrategy.Merge)
                    {
                        var top = Color.Lerp(firstColor.Top, secondColor.Top, 0.5f);
                        var middle = Color.Lerp(firstColor.Middle, secondColor.Middle, 0.5f);
                        var bottom = Color.Lerp(firstColor.Bottom, secondColor.Bottom, 0.5f);
                        mat.SetColor("_TopColor", top);
                        mat.SetColor("_MiddleColor", middle);
                        mat.SetColor("_BottomColor", bottom);
                        mat.SetColor("_SpecColor", middle);
                    }

                    if (useSloomberShader)
                    {
                        mat.SetTexture("_SloomberColorOverlay", sloomberMat.GetTexture("_SloomberColorOverlay"));
                        mat.SetTexture("_SloomberStarMask", sloomberMat.GetTexture("_SloomberStarMask"));

                        mat.EnableKeyword("_BODYCOLORING_SLOOMBER");
                        mat.DisableKeyword("_BODYCOLORING_DEFAULT");

                        if (settings.sloomberColors==PrismThreeMergeStrategy.PrioritizeFirst)
                        {
                            mat.SetColor("_SloomberTopColor", firstColorSloomber.Top);
                            mat.SetColor("_SloomberMiddleColor", firstColorSloomber.Middle);
                            mat.SetColor("_SloomberBottomColor", firstColorSloomber.Bottom);
                        }
                        else if (settings.sloomberColors==PrismThreeMergeStrategy.PrioritizeSecond)
                        {
                            mat.SetColor("_SloomberTopColor", secondColorSloomber.Top);
                            mat.SetColor("_SloomberMiddleColor", secondColorSloomber.Middle);
                            mat.SetColor("_SloomberBottomColor", secondColorSloomber.Bottom);
                        }
                        else if (settings.sloomberColors==PrismThreeMergeStrategy.Merge)
                        {
                            var top = Color.Lerp(firstColorSloomber.Top, secondColorSloomber.Top, 0.5f);
                            var middle = Color.Lerp(firstColorSloomber.Middle, secondColorSloomber.Middle, 0.5f);
                            var bottom = Color.Lerp(firstColorSloomber.Bottom, secondColorSloomber.Bottom, 0.5f);
                            mat.SetColor("_SloomberTopColor", top);
                            mat.SetColor("_SloomberMiddleColor", middle);
                            mat.SetColor("_SloomberBottomColor", bottom);
                        }
                    }

                    if (useTwinShader)
                    {
                        mat.EnableKeyword("_ENABLETWINEFFECT_ON");

                        if (settings.twinColors==PrismThreeMergeStrategy.PrioritizeFirst)
                        {
                            mat.SetColor("_TwinTopColor", firstColorTwin.Top);
                            mat.SetColor("_TwinMiddleColor", firstColorTwin.Middle);
                            mat.SetColor("_TwinBottomColor", firstColorTwin.Bottom);
                        }
                        else if (settings.twinColors==PrismThreeMergeStrategy.PrioritizeSecond)
                        {
                            mat.SetColor("_TwinTopColor", secondColorTwin.Top);
                            mat.SetColor("_TwinMiddleColor", secondColorTwin.Middle);
                            mat.SetColor("_TwinBottomColor", secondColorTwin.Bottom);
                        }
                        else if (settings.twinColors==PrismThreeMergeStrategy.Merge)
                        {
                            var top = Color.Lerp(firstColorTwin.Top, secondColorTwin.Top, 0.5f);
                            var middle = Color.Lerp(firstColorTwin.Middle, secondColorTwin.Middle, 0.5f);
                            var bottom = Color.Lerp(firstColorTwin.Bottom, secondColorTwin.Bottom, 0.5f);
                            mat.SetColor("_TwinTopColor", top);
                            mat.SetColor("_TwinMiddleColor", middle);
                            mat.SetColor("_TwinBottomColor", bottom);
                        }
                    }
                }
                catch
                {
                }
            }
        }

        return new Il2CppReferenceArray<SlimeAppearanceStructure>(newStructures.ToArray());
    }


}