using Il2CppInterop.Runtime.InteropTypes.Arrays;
using SR2E.Prism.Data;

namespace SR2E.Prism.Lib;

public static class PrismLibMerging
{

    public static SlimeDiet MergeDiet(SlimeDiet firstDiet, SlimeDiet secondDiet)
    {
        var mergedDiet = PrismLibDiet.CreateNewDiet();

        mergedDiet.EatMap=mergedDiet.EatMap.AddRangeNoMultipleToNew(firstDiet.EatMap);
        mergedDiet.EatMap=mergedDiet.EatMap.AddRangeNoMultipleToNew(secondDiet.EatMap);

        mergedDiet.AdditionalFoodIdents = mergedDiet.AdditionalFoodIdents.AddRangeNoMultipleToNew(firstDiet.AdditionalFoodIdents);
        mergedDiet.AdditionalFoodIdents = mergedDiet.AdditionalFoodIdents.AddRangeNoMultipleToNew(secondDiet.AdditionalFoodIdents);

        mergedDiet.FavoriteIdents = mergedDiet.FavoriteIdents.AddRangeNoMultipleToNew(firstDiet.FavoriteIdents);
        mergedDiet.FavoriteIdents = mergedDiet.FavoriteIdents.AddRangeNoMultipleToNew(secondDiet.FavoriteIdents);

        mergedDiet.MajorFoodIdentifiableTypeGroups = mergedDiet.MajorFoodIdentifiableTypeGroups.AddRangeNoMultipleToNew(firstDiet.MajorFoodIdentifiableTypeGroups);
        mergedDiet.MajorFoodIdentifiableTypeGroups = mergedDiet.MajorFoodIdentifiableTypeGroups.AddRangeNoMultipleToNew(secondDiet.MajorFoodIdentifiableTypeGroups);

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
    public static void MergeComponentsV01(GameObject obj, GameObject oldOne, GameObject? oldTwo = null)
    {
        Dictionary<string, (MonoBehaviour, bool)> components = new Dictionary<string, (MonoBehaviour, bool)>();
        
        foreach (var comp in obj.GetComponents<MonoBehaviour>())
            components.TryAdd(comp.GetIl2CppType().Name,(comp,false));
        
        foreach (var comp in oldOne.GetComponents<MonoBehaviour>())
            components.TryAdd(comp.GetIl2CppType().Name,(comp,true));
        
        if (oldTwo!=null)
            foreach (var comp in oldTwo.GetComponents<MonoBehaviour>())
                components.TryAdd(comp.GetIl2CppType().Name,(comp,true));

        foreach (var component in components)
            if (component.Value.Item2)
                if (!component.Key.ContainsAny("CrystalSlimeLaunch",
                        "AweTowardsLargos", "SlimeEyeComponents", "SlimeMouthComponents", "ColliderTotemLinkerHelper"))
                {
                    if (!obj.GetComponent(component.Value.Item1.GetIl2CppType()))
                    {
                        var newComp = obj.AddComponent(component.Value.Item1.GetIl2CppType());
                        newComp.CopyFields(component.Value.Item1);
                        if (newComp.TryCast<Behaviour>() != null)
                            newComp.Cast<Behaviour>().enabled = true;
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

    public static PrismThreeMergeStrategy GetOptimalRandomizationV01(SlimeDefinition one, SlimeDefinition two)
    {
        //if (one.ReferenceId == "SlimeDefinition.Flutter") return PrismThreeMergeStrategy.PrioritizeFirst;
        //if (two.ReferenceId == "SlimeDefinition.Flutter") return PrismThreeMergeStrategy.PrioritizeSecond;
        if (one.ReferenceId == "SlimeDefinition.Saber") return PrismThreeMergeStrategy.PrioritizeFirst;
        if (two.ReferenceId == "SlimeDefinition.Saber") return PrismThreeMergeStrategy.PrioritizeSecond;
        if (one.ReferenceId == "SlimeDefinition.Hyper") return PrismThreeMergeStrategy.PrioritizeFirst;
        if (two.ReferenceId == "SlimeDefinition.Hyper") return PrismThreeMergeStrategy.PrioritizeSecond;
        if (one.ReferenceId == "SlimeDefinition.Phosphor") return PrismThreeMergeStrategy.PrioritizeFirst;
        if (two.ReferenceId == "SlimeDefinition.Phosphor") return PrismThreeMergeStrategy.PrioritizeSecond;
        if (one.ReferenceId == "SlimeDefinition.Gold") return PrismThreeMergeStrategy.PrioritizeFirst;
        if (two.ReferenceId == "SlimeDefinition.Gold") return PrismThreeMergeStrategy.PrioritizeSecond;
        if (one.ReferenceId == "SlimeDefinition.Shadow") return PrismThreeMergeStrategy.PrioritizeFirst;
        if (two.ReferenceId == "SlimeDefinition.Shadow") return PrismThreeMergeStrategy.PrioritizeSecond;
        
        if (one.ReferenceId == "SlimeDefinition.Flutter") return PrismThreeMergeStrategy.PrioritizeSecond;
        if (two.ReferenceId == "SlimeDefinition.Flutter") return PrismThreeMergeStrategy.PrioritizeFirst;
        /*if (one.ReferenceId == "SlimeDefinition.Pink") return PrismThreeMergeStrategy.PrioritizeSecond;
        if (two.ReferenceId == "SlimeDefinition.Pink") return PrismThreeMergeStrategy.PrioritizeFirst;
        if (one.ReferenceId == "SlimeDefinition.Hyper") return PrismThreeMergeStrategy.PrioritizeSecond;
        if (two.ReferenceId == "SlimeDefinition.Hyper") return PrismThreeMergeStrategy.PrioritizeFirst;*/
        
        return PrismThreeMergeStrategy.Merge; //(string.Compare(one.Name, two.Name) < 0)?PrismThreeMergeStrategy.PrioritizeFirst:PrismThreeMergeStrategy.PrioritizeSecond;
    }
    private static bool logCombineErrors = false;
    /// <summary>
    /// Merges two appearance structures together for a largo.
    /// </summary>
    /// <param name="slime1">Base slime #1</param>
    /// <param name="slime2">Base slime #2</param>
    /// <param name="settings">Settings that determine what structures this function merges, and what colors.</param>
    /// <returns>A new array of structures.</returns>
    public static Il2CppReferenceArray<SlimeAppearanceStructure> MergeStructuresV01(SlimeAppearance slime1, SlimeAppearance slime2, 
        PrismLargoMergeSettings settings, PrismThreeMergeStrategy optimalPrioritization)
    {
        var newStructures = new List<SlimeAppearanceStructure>(0);
        SlimeAppearance.Palette firstColor = slime1.INTERNAL_GetMainPalette();
        SlimeAppearance.Palette firstColorTwin = slime1.INTERNAL_GetTwinPalette();
        SlimeAppearance.Palette firstColorSloomber = slime1.INTERNAL_GetSloomberPalette();

        SlimeAppearance.Palette secondColor = slime2.INTERNAL_GetMainPalette();
        SlimeAppearance.Palette secondColorTwin = slime2.INTERNAL_GetTwinPalette();
        SlimeAppearance.Palette secondColorSloomber = slime2.INTERNAL_GetSloomberPalette();


        var oneHasTwin = INTERNAL_GetLargoHasTwinEffect(slime1);
        var twoHasTwin = INTERNAL_GetLargoHasTwinEffect(slime2);
        bool useTwinShader = oneHasTwin||twoHasTwin;
        bool useBoomShader = INTERNAL_GetLargoHasBoomEffect(slime1, slime2);
        Material boomMat = PrismNativeBaseSlime.Boom.GetPrismBaseSlime().GetSlimeDefinition().AppearancesDefault[0]._structures[0].DefaultMaterials[0];
        bool useHyperShader = INTERNAL_GetLargoHasHyperEffect(slime1, slime2);

        var oneHasSloomber = INTERNAL_GetLargoHasSloomberEffect(slime1);
        var twoHasSloomber = INTERNAL_GetLargoHasSloomberEffect(slime2);
        bool useSloomberShader = oneHasSloomber||twoHasSloomber;
        Material sloomberMat = PrismNativeBaseSlime.Sloomber.GetPrismBaseSlime().GetSlimeDefinition().AppearancesDefault[0]._structures[0].DefaultMaterials[0];

        bool firstBody = false;
        bool firstFace = false; 
        //name=DefaultBody_v4
        //name=SlimeFace
        switch (settings.body)
        {
            case PrismBFMergeStrategy.KeepFirst:
                firstBody = true; break;
            case PrismBFMergeStrategy.KeepSecond:
                firstBody = false; break;
            case PrismBFMergeStrategy.Optimal:
                if (!INTERNAL_GetLargoHasDefaultBody(slime1))
                    firstBody = true;
                else firstBody = false;
                break;
        }
        switch (settings.face)
        {
            case PrismBFMergeStrategy.KeepFirst:
                firstFace = true; break;
            case PrismBFMergeStrategy.KeepSecond:
                firstFace = false; break;
            case PrismBFMergeStrategy.Optimal:
                if (!INTERNAL_GetLargoHasDefaultFace(slime1))
                    firstFace = true;
                else firstFace = false;
                break;
        }
        
        bool pFirst = false; bool pSecond = false; bool pMerge = false;
        bool pTwinFirst = false; bool pTwinSecond = false; bool pTwinMerge = false;
        bool pSloomberFirst = false; bool pSloomberSecond = false; bool pSloomberMerge = false;
        if (settings.baseColors == PrismColorMergeStrategy.Optimal)
        {
            if (optimalPrioritization == PrismThreeMergeStrategy.PrioritizeFirst) pFirst = true;
            else if (optimalPrioritization == PrismThreeMergeStrategy.PrioritizeSecond) pSecond = true;
            else if (optimalPrioritization == PrismThreeMergeStrategy.Merge) pMerge = true;
        }
        if (settings.twinColors == PrismColorMergeStrategy.Optimal)
        {
            if (oneHasTwin&&!twoHasTwin) pTwinFirst = true;
            else if (!oneHasTwin&&twoHasTwin) pTwinSecond = true;
            else if (optimalPrioritization == PrismThreeMergeStrategy.PrioritizeFirst) pTwinFirst = true;
            else if (optimalPrioritization == PrismThreeMergeStrategy.PrioritizeSecond) pTwinSecond = true;
            else if (optimalPrioritization == PrismThreeMergeStrategy.Merge) pTwinMerge = true;
        }
        if (settings.sloomberColors == PrismColorMergeStrategy.Optimal)
        {
            if (oneHasSloomber&&!twoHasSloomber) pTwinFirst = true;
            else if (!oneHasSloomber&&twoHasSloomber) pSloomberSecond = true;
            else if (optimalPrioritization == PrismThreeMergeStrategy.PrioritizeFirst) pSloomberFirst = true;
            else if (optimalPrioritization == PrismThreeMergeStrategy.PrioritizeSecond) pSloomberSecond = true;
            else if (optimalPrioritization == PrismThreeMergeStrategy.Merge) pSloomberMerge = true;
        }
        foreach (var structure in slime1.Structures)
        {
            if (structure.Element.Type == SlimeAppearanceElement.ElementType.FACE ||
                structure.Element.Type == SlimeAppearanceElement.ElementType.FACE_ATTACH)
            {
                if (firstFace)
                {
                    if (structure != null && !newStructures.Contains(structure) &&
                        structure.DefaultMaterials.Length != 0)
                    {
                        var newStructure = new SlimeAppearanceStructure(structure);
                        newStructures.Add(newStructure);
                        var mat = Object.Instantiate(structure.DefaultMaterials[0]);
                        newStructure.DefaultMaterials = new List<Material>() { mat }.ToArray();


                        try
                        {
                            if (settings.baseColors==PrismColorMergeStrategy.PrioritizeFirst||pFirst)
                            {
                                mat.SetColor("_TopColor", firstColor.Top);
                                mat.SetColor("_MiddleColor", firstColor.Middle);
                                mat.SetColor("_BottomColor", firstColor.Bottom);
                                mat.SetColor("_SpecColor", firstColor.Middle);
                            }
                            else if (settings.baseColors==PrismColorMergeStrategy.PrioritizeSecond||pSecond)
                            {
                                mat.SetColor("_TopColor", secondColor.Top);
                                mat.SetColor("_MiddleColor", secondColor.Middle);
                                mat.SetColor("_BottomColor", secondColor.Bottom);
                                mat.SetColor("_SpecColor", secondColor.Middle);
                            }
                            else if (settings.baseColors==PrismColorMergeStrategy.Merge||pMerge)
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
                        catch (Exception e) { if(logCombineErrors) MelonLogger.Error(e); }
                    }
                }
            }
            else if (structure.Element.Type == SlimeAppearanceElement.ElementType.BODY)
            {
                if (firstBody)
                {
                    if (structure != null && !newStructures.Contains(structure) &&
                        structure.DefaultMaterials.Length != 0)
                    {
                        var newStructure = new SlimeAppearanceStructure(structure);
                        newStructures.Add(newStructure);
                        var mat = Object.Instantiate(structure.DefaultMaterials[0]);
                        newStructure.DefaultMaterials = new List<Material>() { mat }.ToArray();

                        try
                        {
                            if (settings.baseColors==PrismColorMergeStrategy.PrioritizeFirst||pFirst)
                            {
                                mat.SetColor("_TopColor", firstColor.Top);
                                mat.SetColor("_MiddleColor", firstColor.Middle);
                                mat.SetColor("_BottomColor", firstColor.Bottom);
                                mat.SetColor("_SpecColor", firstColor.Middle);
                            }
                            else if (settings.baseColors==PrismColorMergeStrategy.PrioritizeSecond||pSecond)
                            {
                                mat.SetColor("_TopColor", secondColor.Top);
                                mat.SetColor("_MiddleColor", secondColor.Middle);
                                mat.SetColor("_BottomColor", secondColor.Bottom);
                                mat.SetColor("_SpecColor", secondColor.Middle);
                            }
                            else if (settings.baseColors==PrismColorMergeStrategy.Merge||pMerge)
                            {
                                var top = Color.Lerp(firstColor.Top, secondColor.Top, 0.5f);
                                var middle = Color.Lerp(firstColor.Middle, secondColor.Middle, 0.5f);
                                var bottom = Color.Lerp(firstColor.Bottom, secondColor.Bottom, 0.5f);
                                mat.SetColor("_TopColor", top);
                                mat.SetColor("_MiddleColor", middle);
                                mat.SetColor("_BottomColor", bottom);
                                mat.SetColor("_SpecColor", middle);
                            }

                            if(useBoomShader)
                            {
                                mat.SetTexture("_Cracks", boomMat.GetTexture("_Cracks"));
                                mat.SetTexture("_CrackNoise", boomMat.GetTexture("_CrackNoise"));
                                mat.EnableKeyword("_ENABLEBOOMCRACKS_ON");
                            }
                            if(useHyperShader) mat.EnableKeyword("_ENABLEHYPEREFFECT_ON");
                            
                            // 0.6 - Twin material
                            if (useTwinShader)
                            {
                                mat.EnableKeyword("_ENABLETWINEFFECT_ON");

                                if (settings.twinColors==PrismColorMergeStrategy.PrioritizeFirst||pTwinFirst)
                                {
                                    mat.SetColor("_TwinTopColor", firstColorTwin.Top);
                                    mat.SetColor("_TwinMiddleColor", firstColorTwin.Middle);
                                    mat.SetColor("_TwinBottomColor", firstColor.Bottom);
                                }
                                else if (settings.twinColors==PrismColorMergeStrategy.PrioritizeSecond||pTwinSecond)
                                {
                                    mat.SetColor("_TwinTopColor", secondColorTwin.Top);
                                    mat.SetColor("_TwinMiddleColor", secondColorTwin.Middle);
                                    mat.SetColor("_TwinBottomColor", secondColorTwin.Bottom);
                                }
                                else if (settings.twinColors==PrismColorMergeStrategy.Merge||pTwinMerge)
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

                                    if (settings.sloomberColors==PrismColorMergeStrategy.PrioritizeFirst||pSloomberFirst)
                                    {
                                        mat.SetColor("_SloomberTopColor", firstColorSloomber.Top);
                                        mat.SetColor("_SloomberBottomColor", firstColorSloomber.Bottom);
                                    }
                                    else if (settings.sloomberColors==PrismColorMergeStrategy.PrioritizeSecond||pSloomberSecond)
                                    {
                                        mat.SetColor("_SloomberTopColor", secondColorSloomber.Top);
                                        mat.SetColor("_SloomberBottomColor", secondColorSloomber.Bottom);
                                    }
                                    else if (settings.sloomberColors==PrismColorMergeStrategy.Merge||pSloomberMerge)
                                    {
                                        var top = Color.Lerp(firstColorSloomber.Top, secondColorSloomber.Top, 0.5f);
                                        var middle = Color.Lerp(firstColorSloomber.Middle, secondColorSloomber.Middle, 0.5f);
                                        var bottom = Color.Lerp(firstColorSloomber.Bottom, secondColorSloomber.Bottom, 0.5f);
                                        mat.SetColor("_SloomberTopColor", top);
                                        mat.SetColor("_SloomberBottomColor", bottom);
                                    }
                                }
                            }
                        }
                        catch (Exception e) { if(logCombineErrors) MelonLogger.Error(e); }
                    }
                }
            }
            else if (structure != null && !newStructures.Contains(structure) && structure.DefaultMaterials.Length != 0)
            {
                var newStructure = new SlimeAppearanceStructure(structure);
                newStructures.Add(newStructure);
                var mat = Object.Instantiate(structure.DefaultMaterials[0]);
                newStructure.DefaultMaterials = new List<Material>() { mat }.ToArray();
                if (structure.Element.Type == SlimeAppearanceElement.ElementType.FOREHEAD) continue;

                try
                {
                    if (settings.baseColors==PrismColorMergeStrategy.PrioritizeFirst||pFirst)
                    {
                        mat.SetColor("_TopColor", firstColor.Top);
                        mat.SetColor("_MiddleColor", firstColor.Middle);
                        mat.SetColor("_BottomColor", firstColor.Bottom);
                        mat.SetColor("_SpecColor", firstColor.Middle);
                    }
                    else if (settings.baseColors==PrismColorMergeStrategy.PrioritizeSecond||pSecond)
                    {
                        mat.SetColor("_TopColor", secondColor.Top);
                        mat.SetColor("_MiddleColor", secondColor.Middle);
                        mat.SetColor("_BottomColor", secondColor.Bottom);
                        mat.SetColor("_SpecColor", secondColor.Middle);
                    }
                    else if (settings.baseColors==PrismColorMergeStrategy.Merge||pMerge)
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

                        if (settings.sloomberColors==PrismColorMergeStrategy.PrioritizeFirst||pSloomberFirst)
                        {
                            mat.SetColor("_SloomberTopColor", firstColorSloomber.Top);
                            mat.SetColor("_SloomberBottomColor", firstColorSloomber.Bottom);
                        }
                        else if (settings.sloomberColors==PrismColorMergeStrategy.PrioritizeSecond||pSloomberSecond)
                        {
                            mat.SetColor("_SloomberTopColor", secondColorSloomber.Top);
                            mat.SetColor("_SloomberBottomColor", secondColorSloomber.Bottom);
                        }
                        else if (settings.sloomberColors==PrismColorMergeStrategy.Merge||pSloomberMerge)
                        {
                            var top = Color.Lerp(firstColorSloomber.Top, secondColorSloomber.Top, 0.5f);
                            var bottom = Color.Lerp(firstColorSloomber.Bottom, secondColorSloomber.Bottom, 0.5f);
                            mat.SetColor("_SloomberTopColor", top);
                            mat.SetColor("_SloomberBottomColor", bottom);
                        }
                    }
                    if(useBoomShader)
                    {
                        mat.SetTexture("_Cracks", boomMat.GetTexture("_Cracks"));
                        mat.SetTexture("_CrackNoise", boomMat.GetTexture("_CrackNoise"));
                        mat.EnableKeyword("_ENABLEBOOMCRACKS_ON");
                    }
                    if(useHyperShader) mat.EnableKeyword("_ENABLEHYPEREFFECT_ON");
                    
                    if (useTwinShader)
                    {
                        mat.EnableKeyword("_ENABLETWINEFFECT_ON");

                        if (settings.twinColors==PrismColorMergeStrategy.PrioritizeFirst||pTwinFirst)
                        {
                            mat.SetColor("_TwinTopColor", firstColorTwin.Top);
                            mat.SetColor("_TwinMiddleColor", firstColorTwin.Middle);
                            mat.SetColor("_TwinBottomColor", firstColorTwin.Bottom);
                        }
                        else if (settings.twinColors==PrismColorMergeStrategy.PrioritizeSecond||pTwinSecond)
                        {
                            mat.SetColor("_TwinTopColor", secondColorTwin.Top);
                            mat.SetColor("_TwinMiddleColor", secondColorTwin.Middle);
                            mat.SetColor("_TwinBottomColor", secondColorTwin.Bottom);
                        }
                        else if (settings.twinColors==PrismColorMergeStrategy.Merge||pTwinMerge)
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
                catch (Exception e) { if(logCombineErrors) MelonLogger.Error(e); }
            }
        }

        foreach (var structure in slime2.Structures)
        {
            if (structure.Element.Type == SlimeAppearanceElement.ElementType.FACE ||
                structure.Element.Type == SlimeAppearanceElement.ElementType.FACE_ATTACH)
            {
                if (!firstFace)
                {
                    if (structure != null && !newStructures.Contains(structure) && structure.DefaultMaterials.Length != 0)
                    {
                        var newStructure = new SlimeAppearanceStructure(structure);
                        newStructures.Add(newStructure);
                        var mat = Object.Instantiate(structure.DefaultMaterials[0]);
                        newStructure.DefaultMaterials = new List<Material>() { mat }.ToArray();


                        try
                        {
                            if (settings.baseColors==PrismColorMergeStrategy.PrioritizeFirst||pFirst)
                            {
                                mat.SetColor("_TopColor", firstColor.Top);
                                mat.SetColor("_MiddleColor", firstColor.Middle);
                                mat.SetColor("_BottomColor", firstColor.Bottom);
                                mat.SetColor("_SpecColor", firstColor.Middle);
                            }
                            else if (settings.baseColors==PrismColorMergeStrategy.PrioritizeSecond||pSecond)
                            {
                                mat.SetColor("_TopColor", secondColor.Top);
                                mat.SetColor("_MiddleColor", secondColor.Middle);
                                mat.SetColor("_BottomColor", secondColor.Bottom);
                                mat.SetColor("_SpecColor", secondColor.Middle);
                            }
                            else if (settings.baseColors==PrismColorMergeStrategy.Merge||pMerge)
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

                                if (settings.sloomberColors==PrismColorMergeStrategy.PrioritizeFirst||pSloomberFirst)
                                {
                                    mat.SetColor("_SloomberTopColor", firstColorSloomber.Top);
                                    mat.SetColor("_SloomberBottomColor", firstColorSloomber.Bottom);
                                }
                                else if (settings.sloomberColors==PrismColorMergeStrategy.PrioritizeSecond||pSloomberSecond)
                                {
                                    mat.SetColor("_SloomberTopColor", secondColorSloomber.Top);
                                    mat.SetColor("_SloomberBottomColor", secondColorSloomber.Bottom);
                                }
                                else if (settings.sloomberColors==PrismColorMergeStrategy.Merge||pSloomberMerge)
                                {
                                    var top = Color.Lerp(firstColorSloomber.Top, secondColorSloomber.Top, 0.5f);
                                    var bottom = Color.Lerp(firstColorSloomber.Bottom, secondColorSloomber.Bottom, 0.5f);
                                    mat.SetColor("_SloomberTopColor", top);
                                    mat.SetColor("_SloomberBottomColor", bottom);
                                }
                            }
                            if(useBoomShader)
                            {
                                mat.SetTexture("_Cracks", boomMat.GetTexture("_Cracks"));
                                mat.SetTexture("_CrackNoise", boomMat.GetTexture("_CrackNoise"));
                                mat.EnableKeyword("_ENABLEBOOMCRACKS_ON");
                            }
                            if(useHyperShader) mat.EnableKeyword("_ENABLEHYPEREFFECT_ON");
                            
                            if (useTwinShader)
                            {
                                mat.EnableKeyword("_ENABLETWINEFFECT_ON");

                                if (settings.twinColors==PrismColorMergeStrategy.PrioritizeFirst||pTwinFirst)
                                {
                                    mat.SetColor("_TwinTopColor", firstColorTwin.Top);
                                    mat.SetColor("_TwinMiddleColor", firstColorTwin.Middle);
                                    mat.SetColor("_TwinBottomColor", firstColorTwin.Bottom);
                                }
                                else if (settings.twinColors==PrismColorMergeStrategy.PrioritizeSecond||pTwinSecond)
                                {
                                    mat.SetColor("_TwinTopColor", secondColorTwin.Top);
                                    mat.SetColor("_TwinMiddleColor", secondColorTwin.Middle);
                                    mat.SetColor("_TwinBottomColor", secondColorTwin.Bottom);
                                }
                                else if (settings.twinColors==PrismColorMergeStrategy.Merge||pTwinMerge)
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
                        catch (Exception e) { if(logCombineErrors) MelonLogger.Error(e); }
                    }
                }
            }
            else if (structure.Element.Type == SlimeAppearanceElement.ElementType.BODY)
            {
                if (!firstBody)
                {
                    if (!newStructures.Contains(structure))
                    {
                        var newStructure = new SlimeAppearanceStructure(structure);
                        newStructures.Add(newStructure);
                        var mat = Object.Instantiate(structure.DefaultMaterials[0]);
                        newStructure.DefaultMaterials = new List<Material>() { mat }.ToArray();


                        try
                        {
                            if (settings.baseColors==PrismColorMergeStrategy.PrioritizeFirst||pFirst)
                            {
                                mat.SetColor("_TopColor", firstColor.Top);
                                mat.SetColor("_MiddleColor", firstColor.Middle);
                                mat.SetColor("_BottomColor", firstColor.Bottom);
                                mat.SetColor("_SpecColor", firstColor.Middle);
                            }
                            else if (settings.baseColors==PrismColorMergeStrategy.PrioritizeSecond||pSecond)
                            {
                                mat.SetColor("_TopColor", secondColor.Top);
                                mat.SetColor("_MiddleColor", secondColor.Middle);
                                mat.SetColor("_BottomColor", secondColor.Bottom);
                                mat.SetColor("_SpecColor", secondColor.Middle);
                            }
                            else if (settings.baseColors==PrismColorMergeStrategy.Merge||pMerge)
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

                                if (settings.sloomberColors==PrismColorMergeStrategy.PrioritizeFirst||pSloomberFirst)
                                {
                                    mat.SetColor("_SloomberTopColor", firstColorSloomber.Top);
                                    mat.SetColor("_SloomberBottomColor", firstColorSloomber.Bottom);
                                }
                                else if (settings.sloomberColors==PrismColorMergeStrategy.PrioritizeSecond||pSloomberSecond)
                                {
                                    mat.SetColor("_SloomberTopColor", secondColorSloomber.Top);
                                    mat.SetColor("_SloomberBottomColor", secondColorSloomber.Bottom);
                                }
                                else if (settings.sloomberColors==PrismColorMergeStrategy.Merge||pSloomberMerge)
                                {
                                    var top = Color.Lerp(firstColorSloomber.Top, secondColorSloomber.Top, 0.5f);
                                    var bottom = Color.Lerp(firstColorSloomber.Bottom, secondColorSloomber.Bottom, 0.5f);
                                    mat.SetColor("_SloomberTopColor", top);
                                    mat.SetColor("_SloomberBottomColor", bottom);
                                }
                            }
                            if(useBoomShader)
                            {
                                mat.SetTexture("_Cracks", boomMat.GetTexture("_Cracks"));
                                mat.SetTexture("_CrackNoise", boomMat.GetTexture("_CrackNoise"));
                                mat.EnableKeyword("_ENABLEBOOMCRACKS_ON");
                            }
                            if(useHyperShader) mat.EnableKeyword("_ENABLEHYPEREFFECT_ON");
                            
                            if (useTwinShader)
                            {
                                mat.EnableKeyword("_ENABLETWINEFFECT_ON");

                                if (settings.twinColors==PrismColorMergeStrategy.PrioritizeFirst||pTwinFirst)
                                {
                                    mat.SetColor("_TwinTopColor", firstColorTwin.Top);
                                    mat.SetColor("_TwinMiddleColor", firstColorTwin.Middle);
                                    mat.SetColor("_TwinBottomColor", firstColorTwin.Bottom);
                                }
                                else if (settings.twinColors==PrismColorMergeStrategy.PrioritizeSecond||pTwinSecond)
                                {
                                    mat.SetColor("_TwinTopColor", secondColorTwin.Top);
                                    mat.SetColor("_TwinMiddleColor", secondColorTwin.Middle);
                                    mat.SetColor("_TwinBottomColor", secondColorTwin.Bottom);
                                }
                                else if (settings.twinColors==PrismColorMergeStrategy.Merge||pTwinMerge)
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
                        catch (Exception e) { if(logCombineErrors) MelonLogger.Error(e); }
                    }

                }
            }
            else if (structure != null && !newStructures.Contains(structure) && structure.DefaultMaterials.Length != 0)
            {
                var newStructure = new SlimeAppearanceStructure(structure);
                newStructures.Add(newStructure);
                var mat = Object.Instantiate(structure.DefaultMaterials[0]);
                newStructure.DefaultMaterials = new List<Material>() { mat }.ToArray();

                if (structure.Element.Type == SlimeAppearanceElement.ElementType.FOREHEAD) continue;
                try
                {
                    if (settings.baseColors==PrismColorMergeStrategy.PrioritizeFirst||pFirst)
                    {
                        mat.SetColor("_TopColor", firstColor.Top);
                        mat.SetColor("_MiddleColor", firstColor.Middle);
                        mat.SetColor("_BottomColor", firstColor.Bottom);
                        mat.SetColor("_SpecColor", firstColor.Middle);
                    }
                    else if (settings.baseColors==PrismColorMergeStrategy.PrioritizeSecond||pSecond)
                    {
                        mat.SetColor("_TopColor", secondColor.Top);
                        mat.SetColor("_MiddleColor", secondColor.Middle);
                        mat.SetColor("_BottomColor", secondColor.Bottom);
                        mat.SetColor("_SpecColor", secondColor.Middle);
                    }
                    else if (settings.baseColors==PrismColorMergeStrategy.Merge||pMerge)
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

                        if (settings.sloomberColors==PrismColorMergeStrategy.PrioritizeFirst||pSloomberFirst)
                        {
                            mat.SetColor("_SloomberTopColor", firstColorSloomber.Top);
                            mat.SetColor("_SloomberBottomColor", firstColorSloomber.Bottom);
                        }
                        else if (settings.sloomberColors==PrismColorMergeStrategy.PrioritizeSecond||pSloomberSecond)
                        {
                            mat.SetColor("_SloomberTopColor", secondColorSloomber.Top);
                            mat.SetColor("_SloomberBottomColor", secondColorSloomber.Bottom);
                        }
                        else if (settings.sloomberColors==PrismColorMergeStrategy.Merge||pSloomberMerge)
                        {
                            var top = Color.Lerp(firstColorSloomber.Top, secondColorSloomber.Top, 0.5f);
                            var bottom = Color.Lerp(firstColorSloomber.Bottom, secondColorSloomber.Bottom, 0.5f);
                            mat.SetColor("_SloomberTopColor", top);
                            mat.SetColor("_SloomberBottomColor", bottom);
                        }
                    }
                    if(useBoomShader)
                    {
                        mat.SetTexture("_Cracks", boomMat.GetTexture("_Cracks"));
                        mat.SetTexture("_CrackNoise", boomMat.GetTexture("_CrackNoise"));
                        mat.EnableKeyword("_ENABLEBOOMCRACKS_ON");
                    }
                    if(useHyperShader) mat.EnableKeyword("_ENABLEHYPEREFFECT_ON");
                    
                    if (useTwinShader)
                    {
                        mat.EnableKeyword("_ENABLETWINEFFECT_ON");

                        if (settings.twinColors==PrismColorMergeStrategy.PrioritizeFirst||pTwinFirst)
                        {
                            mat.SetColor("_TwinTopColor", firstColorTwin.Top);
                            mat.SetColor("_TwinMiddleColor", firstColorTwin.Middle);
                            mat.SetColor("_TwinBottomColor", firstColorTwin.Bottom);
                        }
                        else if (settings.twinColors==PrismColorMergeStrategy.PrioritizeSecond||pTwinSecond)
                        {
                            mat.SetColor("_TwinTopColor", secondColorTwin.Top);
                            mat.SetColor("_TwinMiddleColor", secondColorTwin.Middle);
                            mat.SetColor("_TwinBottomColor", secondColorTwin.Bottom);
                        }
                        else if (settings.twinColors==PrismColorMergeStrategy.Merge||pTwinMerge)
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
                catch (Exception e) { if(logCombineErrors) MelonLogger.Error(e); }
            }
        }

        return new Il2CppReferenceArray<SlimeAppearanceStructure>(newStructures.ToArray());
    }

    static SlimeAppearance.Palette INTERNAL_GetTwinPalette(this SlimeAppearance app)
    {
        Material mat = null;
        foreach (var structure in app._structures)
        {
            if (structure.Element.Type == SlimeAppearanceElement.ElementType.BODY)
            {
                mat = structure.DefaultMaterials[0];
                break;
            }
        }

        if (mat == null) return new SlimeAppearance.Palette();

        return new SlimeAppearance.Palette()
        {
            Ammo = new Color32(255, 255, 255, 255),
            Top = mat.GetColor("_TwinTopColor"),
            Middle = mat.GetColor("_TwinMiddleColor"),
            Bottom = mat.GetColor("_TwinBottomColor"),
        };
    }

    static SlimeAppearance.Palette INTERNAL_GetSloomberPalette(this SlimeAppearance app)
    {
        Material mat = null;
        foreach (var structure in app._structures)
        {
            if (structure.Element.Type == SlimeAppearanceElement.ElementType.BODY)
            {
                mat = structure.DefaultMaterials[0];
                break;
            }
        }

        if (mat == null) return new SlimeAppearance.Palette();
        return new SlimeAppearance.Palette()
        {
            Ammo = new Color32(255, 255, 255, 255),
            Top = mat.GetColor("_SloomberTopColor"),
            Bottom = mat.GetColor("_SloomberBottomColor"),
        };
    }
    static SlimeAppearance.Palette INTERNAL_GetMainPalette(this SlimeAppearance app)
    {
        Material mat = null;
        foreach (var structure in app._structures)
        {
            if (structure.Element.Type == SlimeAppearanceElement.ElementType.BODY)
            {
                mat = structure.DefaultMaterials[0];
                break;
            }
        }

        if (mat == null) return new SlimeAppearance.Palette();
        return new SlimeAppearance.Palette()
        {
            Ammo = new Color32(255, 255, 255, 255),
            Top = mat.GetColor("_TopColor"),
            Middle = mat.GetColor("_MiddleColor"),
            Bottom = mat.GetColor("_BottomColor"),
        };
    }
    internal static bool INTERNAL_GetLargoHasHyperEffect(SlimeAppearance slime1, SlimeAppearance slime2)
    {
        foreach (var structure in slime1._structures)
            if (structure.Element.Type == SlimeAppearanceElement.ElementType.BODY)
                try { if (structure.DefaultMaterials[0].IsKeywordEnabled("_ENABLEHYPEREFFECT_ON")) return true; } catch { }
        foreach (var structure in slime2._structures)
            if (structure.Element.Type == SlimeAppearanceElement.ElementType.BODY)
                try { if (structure.DefaultMaterials[0].IsKeywordEnabled("_ENABLEHYPEREFFECT_ON")) return true; } catch { }
        return false;
    }
    internal static bool INTERNAL_GetLargoHasDefaultBody(SlimeAppearance slime1)
    {
        foreach (var structure in slime1._structures)
            if (structure.Element.Type == SlimeAppearanceElement.ElementType.BODY)
                try { if (structure.Element.name.Contains("DefaultBody")) return true; } catch { }
        return false;
    }
    internal static bool INTERNAL_GetLargoHasDefaultFace(SlimeAppearance slime1)
    {
        foreach (var structure in slime1._structures)
            if (structure.Element.Type == SlimeAppearanceElement.ElementType.FACE)
                try { if (structure.Element.name=="SlimeFace".Replace("(Clone)","")) return true; } catch { }
        return false;
    }
    internal static bool INTERNAL_GetLargoHasBoomEffect(SlimeAppearance slime1, SlimeAppearance slime2)
    {
        foreach (var structure in slime1._structures)
            if (structure.Element.Type == SlimeAppearanceElement.ElementType.BODY)
                try { if (structure.DefaultMaterials[0].IsKeywordEnabled("_ENABLEBOOMCRACKS_ON")) return true; } catch { }
        foreach (var structure in slime2._structures)
            if (structure.Element.Type == SlimeAppearanceElement.ElementType.BODY)
                try { if (structure.DefaultMaterials[0].IsKeywordEnabled("_ENABLEBOOMCRACKS_ON")) return true; } catch { }
        return false;
    }
    internal static bool INTERNAL_GetLargoHasTwinEffect(SlimeAppearance slime)
    {
        foreach (var structure in slime._structures)
            if (structure.Element.Type == SlimeAppearanceElement.ElementType.BODY)
                try { if (structure.DefaultMaterials[0].IsKeywordEnabled("_ENABLETWINEFFECT_ON")) return true; } catch { }
        return false;
    }
    internal static bool INTERNAL_GetLargoHasTwinEffect(SlimeAppearance slime1, SlimeAppearance slime2)
    {
        foreach (var structure in slime1._structures)
            if (structure.Element.Type == SlimeAppearanceElement.ElementType.BODY)
                try { if (structure.DefaultMaterials[0].IsKeywordEnabled("_ENABLETWINEFFECT_ON")) return true; } catch { }
        foreach (var structure in slime2._structures)
            if (structure.Element.Type == SlimeAppearanceElement.ElementType.BODY)
                try { if (structure.DefaultMaterials[0].IsKeywordEnabled("_ENABLETWINEFFECT_ON")) return true; } catch { }
        return false;
    }
 
    internal static bool INTERNAL_GetLargoHasSloomberEffect(SlimeAppearance slime1)
    {
        foreach (var structure in slime1._structures)
            if (structure.Element.Type == SlimeAppearanceElement.ElementType.BODY)
                try { if (structure.DefaultMaterials[0].IsKeywordEnabled("_BODYCOLORING_SLOOMBER")) return true; } catch { }
        return false;
    }
    internal static bool INTERNAL_GetLargoHasSloomberEffect(SlimeAppearance slime1, SlimeAppearance slime2)
    {
        foreach (var structure in slime1._structures)
            if (structure.Element.Type == SlimeAppearanceElement.ElementType.BODY)
                try { if (structure.DefaultMaterials[0].IsKeywordEnabled("_BODYCOLORING_SLOOMBER")) return true; } catch { }
        foreach (var structure in slime2._structures)
            if (structure.Element.Type == SlimeAppearanceElement.ElementType.BODY)
                try { if (structure.DefaultMaterials[0].IsKeywordEnabled("_BODYCOLORING_SLOOMBER")) return true; } catch { }
        return false;
    }

}