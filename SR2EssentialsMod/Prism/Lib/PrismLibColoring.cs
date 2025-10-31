using SR2E.Prism.Data;

namespace SR2E.Prism.Lib;

public static class PrismLibColoring
{
    public static void SetPlortBaseColors(this PrismPlort prismPlort, Color32 Top, Color32 Middle, Color32 Bottom)
    {
        var material = prismPlort.GetPrefab().GetComponent<MeshRenderer>().material;
        material.SetColor("_TopColor", Top);
        material.SetColor("_MiddleColor", Middle);
        material.SetColor("_BottomColor", Bottom);
    }

    public static void SetPlortTwinColors(this PrismPlort prismPlort, Color32 Top, Color32 Middle, Color32 Bottom)
    {
        var material = prismPlort.GetPrefab().GetComponent<MeshRenderer>().material;
        material.SetColor("_TwinTopColor", Top);
        material.SetColor("_TwinMiddleColor", Middle);
        material.SetColor("_TwinBottomColor", Bottom);
    }
    public static void SetSlimeBaseColorsSpecific(this PrismSlime prismSlime, Color32 top, Color32 middle, Color32 bottom, Color32 special, int index, int index2, bool isSS, int structure)
    {
        var slimeDef = prismSlime.GetSlimeDefinition();
        Material mat = null;
        if (isSS)
        {
            mat = slimeDef.AppearancesDynamic.ToArray()[index].Structures[structure].DefaultMaterials[index2];
        }
        else
        {
            mat = slimeDef.AppearancesDefault[index].Structures[structure].DefaultMaterials[index2];
        }

        mat.SetColor("_TopColor", top);
        mat.SetColor("_MiddleColor", middle);
        mat.SetColor("_BottomColor", bottom);
        mat.SetColor("_SpecColor", special);
    }

    public static void SetSlimeBaseColors(this PrismSlime prismSlime, Color32 top, Color32 middle, Color32 bottom)
    {
        var slimeDef = prismSlime.GetSlimeDefinition();
        for (int i = 0; i < slimeDef.AppearancesDefault[0].Structures.Count - 1; i++)
        {
            SlimeAppearanceStructure a = slimeDef.AppearancesDefault[0].Structures[i];
            var mat = a.DefaultMaterials[0];
            mat.SetColor("_TopColor", top);
            mat.SetColor("_MiddleColor", middle);
            mat.SetColor("_BottomColor", bottom);
            mat.SetColor("_SpecColor", middle);
        }
    }

    public static void SetSlimeTwinColors(this PrismSlime prismSlime, Color32 top, Color32 middle, Color32 bottom)
    {
        var slimeDef = prismSlime.GetSlimeDefinition();
        for (int i = 0; i < slimeDef.AppearancesDefault[0].Structures.Count - 1; i++)
        {
            SlimeAppearanceStructure a = slimeDef.AppearancesDefault[0].Structures[i];
            var mat = a.DefaultMaterials[0];
            mat.SetColor("_TwinTopColor", top);
            mat.SetColor("_TwinMiddleColor", middle);
            mat.SetColor("_TwinBottomColor", bottom);
            mat.SetColor("_TwinSpecColor", middle);
        }
    }
    public static void SetSlimeSloomberColors(this PrismSlime prismSlime, Color32 top, Color32 middle, Color32 bottom)
    {
        var slimeDef = prismSlime.GetSlimeDefinition();
        for (int i = 0; i < slimeDef.AppearancesDefault[0].Structures.Count - 1; i++)
        {
            SlimeAppearanceStructure a = slimeDef.AppearancesDefault[0].Structures[i];
            var mat = a.DefaultMaterials[0];

            mat.SetColor("_SloomberTopColor", top);
            mat.SetColor("_SloomberMiddleColor", middle);
            mat.SetColor("_SloomberBottomColor", bottom);
        }
    }
    
    public static void SetSlimeTwinColorsSpecific(this PrismSlime prismSlime, Color32 top, Color32 middle, Color32 bottom, int index, int index2, bool isSS, int structure)
    {
        var slimeDef = prismSlime.GetSlimeDefinition();
        Material mat = null;
        if (isSS == true)
        {
            mat = slimeDef.AppearancesDynamic.ToArray()[index].Structures[structure].DefaultMaterials[index2];
        }
        else
        {
            mat = slimeDef.AppearancesDefault[index].Structures[structure].DefaultMaterials[index2];
        }

        mat.SetColor("_TwinTopColor", top);
        mat.SetColor("_TwinMiddleColor", middle);
        mat.SetColor("_TwinBottomColor", bottom);
    }

    public static void SetSlimeSloomberColorsSpecific(this PrismSlime prismSlime, Color32 top, Color32 middle, Color32 bottom, int index, int index2, bool isSS, int structure)
    {
        var slimeDef = prismSlime.GetSlimeDefinition();
        Material mat = null;
        if (isSS)
        {
            mat = slimeDef.AppearancesDynamic.ToArray()[index].Structures[structure].DefaultMaterials[index2];
        }
        else
        {
            mat = slimeDef.AppearancesDefault[index].Structures[structure].DefaultMaterials[index2];
        }

        mat.SetColor("_SloomberTopColor", top);
        mat.SetColor("_SloomberMiddleColor", middle);
        mat.SetColor("_SloomberBottomColor", bottom);
    }

    // Twin effect uses the shader keyword "_ENABLETWINEFFECT_ON"
    public static void EnableTwinEffectSpecific(this PrismSlime prismSlime, int index, int index2, bool isSS, int structure)
    {
        var slimeDef = prismSlime.GetSlimeDefinition();
        Material mat;
        if (isSS == true)
        {
            mat = slimeDef.AppearancesDynamic.ToArray()[index].Structures[structure].DefaultMaterials[index2];
        }
        else
        {
            mat = slimeDef.AppearancesDefault[index].Structures[structure].DefaultMaterials[index2];
        }

        mat.EnableKeyword("_ENABLETWINEFFECT_ON");
    }

    // Twin effect uses the shader keyword "_ENABLETWINEFFECT_ON"
    public static void EnableTwinEffect(this PrismSlime prismSlime)
    {
        var slimeDef = prismSlime.GetSlimeDefinition();
        for (int i = 0; i < slimeDef.AppearancesDefault[0].Structures.Count - 1; i++)
        {
            SlimeAppearanceStructure a = slimeDef.AppearancesDefault[0].Structures[i];
            var mat = a.DefaultMaterials[0];
            
            mat.EnableKeyword("_ENABLETWINEFFECT_ON");
        }
    }

    // Sloomber effect uses the shader keyword "_BODYCOLORING_SLOOMBER"
    public static void EnableSloomberEffect(this PrismSlime prismSlime)
    {
        var slimeDef = prismSlime.GetSlimeDefinition();
        for (int i = 0; i < slimeDef.AppearancesDefault[0].Structures.Count - 1; i++)
        {
            SlimeAppearanceStructure a = slimeDef.AppearancesDefault[0].Structures[i];
            var mat = a.DefaultMaterials[0];
            
            mat.EnableKeyword("_BODYCOLORING_SLOOMBER");
        }
    }

    public static void DisableTwinEffect(this PrismSlime prismSlime, int index, int index2, bool isSS, int structure)
    {
        var slimeDef = prismSlime.GetSlimeDefinition();
        Material mat;
        if (isSS == true)
        {
            mat = slimeDef.AppearancesDynamic.ToArray()[index].Structures[structure].DefaultMaterials[index2];
        }
        else
        {
            mat = slimeDef.AppearancesDefault[index].Structures[structure].DefaultMaterials[index2];
        }

        mat.DisableKeyword("_ENABLETWINEFFECT_ON");
    }

    /*public static void SetSlimeMatTopColor(this Material mat, Color color) => mat.SetColor("_TopColor", color);
    public static void SetSlimeMatMiddleColor(this Material mat, Color color) => mat.SetColor("_MiddleColor", color);

    public static void SetSlimeMatBottomColor(this Material mat, Color color) => mat.SetColor("_BottomColor", color);

    public static void SetSlimeMatColors(this Material material, Color32 Top, Color32 Middle, Color32 Bottom,
        Color32 Specular)
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
    }*/

}