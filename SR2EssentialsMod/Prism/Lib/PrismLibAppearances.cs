using Il2CppInterop.Runtime.InteropTypes.Arrays;
using SR2E.Prism.Data;

namespace SR2E.Prism.Lib;
/// <summary>
/// A library of helper functions for dealing with slime appearances
/// </summary>
public static class PrismLibAppearances
{
    /*
    /// <summary>
    /// Switches the appearances of two slimes
    /// </summary>
    /// <param name="slimeOneDef">The first slime to switch</param>
    /// <param name="slimeTwoDef">The second slime to switch</param>
    public static void SwitchSlimeAppearances(SlimeDefinition slimeOneDef, SlimeDefinition slimeTwoDef)
    {
        var appearanceOne = slimeOneDef.AppearancesDefault[0]._structures;
        slimeOneDef.AppearancesDefault[0]._structures = slimeTwoDef.AppearancesDefault[0]._structures;
        slimeTwoDef.AppearancesDefault[0]._structures = appearanceOne;
        var appearanceSplatOne = slimeOneDef.AppearancesDefault[0]._splatColor;
        slimeOneDef.AppearancesDefault[0]._splatColor = slimeTwoDef.AppearancesDefault[0]._splatColor;
        slimeTwoDef.AppearancesDefault[0]._splatColor = appearanceSplatOne;

        var colorPalate = slimeOneDef.AppearancesDefault[0]._colorPalette;
        slimeOneDef.AppearancesDefault[0]._colorPalette = slimeTwoDef.AppearancesDefault[0]._colorPalette;
        slimeTwoDef.AppearancesDefault[0]._colorPalette = colorPalate;

        var structureIcon = slimeOneDef.AppearancesDefault[0]._icon;
        slimeOneDef.AppearancesDefault[0]._icon = slimeTwoDef.AppearancesDefault[0]._icon;
        slimeTwoDef.AppearancesDefault[0]._icon = structureIcon;
        var icon = slimeOneDef.icon;
        slimeOneDef.icon = slimeTwoDef.icon;
        slimeTwoDef.icon = icon;

        var debugIcon = slimeOneDef.debugIcon;
        slimeOneDef.debugIcon = slimeTwoDef.debugIcon;
        slimeTwoDef.debugIcon = debugIcon;

    }*/
    /// <summary>
    /// Sets the color of a slime appearance structure
    /// </summary>
    /// <param name="structure">The structure to set the color of</param>
    /// <param name="id">The id of the color to set</param>
    /// <param name="color">The color to set</param>
    public static void SetStructColor(SlimeAppearanceStructure structure, int id, Color color)
    {
        structure.DefaultMaterials[0].SetColor(id, color);
    }

    /// <summary>
    /// Adds a structure to a slime appearance
    /// </summary>
    /// <param name="appearance">The appearance to add the structure to</param>
    /// <param name="structure">The structure to add</param>
    public static void AddStructure(SlimeAppearance appearance, SlimeAppearanceStructure structure)
    {
        appearance.Structures=appearance.Structures.AddToNew(structure);
    }
    /// <summary>
    /// Adds a structure to a slime appearance
    /// </summary>
    /// <param name="app">The appearance to add the structure to</param>
    /// <param name="mesh">The mesh of the structure</param>
    /// <param name="rootBone">The root bone of the structure</param>
    /// <param name="parentBone">The parent bone of the structure</param>
    /// <param name="elementName">The name of the element</param>
    /// <returns>The new structure</returns>
    public static SlimeAppearanceStructure AddStructure(SlimeAppearance app, Mesh mesh, SlimeAppearance.SlimeBone rootBone, SlimeAppearance.SlimeBone parentBone, string elementName)
    {
        var structPrefab = app._structures[0].Element.Prefabs[0].gameObject.CopyObject();
        structPrefab.GetComponent<SkinnedMeshRenderer>().sharedMesh = mesh;
        
        var structObj = structPrefab.GetComponent<SlimeAppearanceObject>();
        structObj.IgnoreLODIndex = true;
        structObj.RootBone = rootBone;
        structObj.ParentBone = parentBone;
        structObj.AttachedBones = new Il2CppStructArray<SlimeAppearance.SlimeBone>(0);
        
        var structure = new SlimeAppearanceStructure(app._structures[0]);
        structure.Element = ScriptableObject.CreateInstance<SlimeAppearanceElement>();
        structure.Element.CastsShadows = true;
        structure.Element.Name = elementName;
        structure.Element.Prefabs = new Il2CppReferenceArray<SlimeAppearanceObject>(new[]
        {
            structObj
        });
        
        app._structures = app._structures.AddToNew(structure);
        return structure;
    }
    /// <summary>
    /// Sets the base colors of a plort
    /// </summary>
    /// <param name="prismPlort">The plort to set the colors of</param>
    /// <param name="Top">The top color</param>
    /// <param name="Middle">The middle color</param>
    /// <param name="Bottom">The bottom color</param>
    public static void SetPlortBaseColors(this PrismPlort prismPlort, Color32 Top, Color32 Middle, Color32 Bottom)
    {
        var material = prismPlort.GetPrefab().GetComponent<MeshRenderer>().material;
        material.SetColor("_TopColor", Top);
        material.SetColor("_MiddleColor", Middle);
        material.SetColor("_BottomColor", Bottom);
        
    }

    /// <summary>
    /// Sets the twin colors of a plort
    /// </summary>
    /// <param name="prismPlort">The plort to set the colors of</param>
    /// <param name="Top">The top color</param>
    /// <param name="Middle">The middle color</param>
    /// <param name="Bottom">The bottom color</param>
    public static void SetPlortTwinColors(this PrismPlort prismPlort, Color32 Top, Color32 Middle, Color32 Bottom)
    {
        var material = prismPlort.GetPrefab().GetComponent<MeshRenderer>().material;
        material.SetColor("_TwinTopColor", Top);
        material.SetColor("_TwinMiddleColor", Middle);
        material.SetColor("_TwinBottomColor", Bottom);
    }
    /// <summary>
    /// Sets the base colors of a slime
    /// </summary>
    /// <param name="prismSlime">The slime to set the colors of</param>
    /// <param name="top">The top color</param>
    /// <param name="middle">The middle color</param>
    /// <param name="bottom">The bottom color</param>
    /// <param name="special">The special color</param>
    /// <param name="index">The index of the appearance</param>
    /// <param name="index2">The index of the material</param>
    /// <param name="isSS">Whether the appearance is a secret style</param>
    /// <param name="structure">The index of the structure</param>
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

    /// <summary>
    /// Sets the base colors of a slime
    /// </summary>
    /// <param name="prismSlime">The slime to set the colors of</param>
    /// <param name="top">The top color</param>
    /// <param name="middle">The middle color</param>
    /// <param name="bottom">The bottom color</param>
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

    /// <summary>
    /// Sets the twin colors of a slime
    /// </summary>
    /// <param name="prismSlime">The slime to set the colors of</param>
    /// <param name="top">The top color</param>
    /// <param name="middle">The middle color</param>
    /// <param name="bottom">The bottom color</param>
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
    /// <summary>
    /// Sets the sloomber colors of a slime
    /// </summary>
    /// <param name="prismSlime">The slime to set the colors of</param>
    /// <param name="top">The top color</param>
    /// <param name="middle">The middle color</param>
    /// <param name="bottom">The bottom color</param>
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
    
    /// <summary>
    /// Sets the twin colors of a slime
    /// </summary>
    /// <param name="prismSlime">The slime to set the colors of</param>
    /// <param name="top">The top color</param>
    /// <param name="middle">The middle color</param>
    /// <param name="bottom">The bottom color</param>
    /// <param name="index">The index of the appearance</param>
    /// <param name="index2">The index of the material</param>
    /// <param name="isSS">Whether the appearance is a secret style</param>
    /// <param name="structure">The index of the structure</param>
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

    /// <summary>
    /// Sets the sloomber colors of a slime
    /// </summary>
    /// <param name="prismSlime">The slime to set the colors of</param>
    /// <param name="top">The top color</param>
    /// <param name="middle">The middle color</param>
    /// <param name="bottom">The bottom color</param>
    /// <param name="index">The index of the appearance</param>
    /// <param name="index2">The index of the material</param>
    /// <param name="isSS">Whether the appearance is a secret style</param>
    /// <param name="structure">The index of the structure</param>
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

    /// <summary>
    /// Enables the twin effect on a slime
    /// </summary>
    /// <param name="prismSlime">The slime to enable the effect on</param>
    /// <param name="index">The index of the appearance</param>
    /// <param name="index2">The index of the material</param>
    /// <param name="isSS">Whether the appearance is a secret style</param>
    /// <param name="structure">The index of the structure</param>
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

    /// <summary>
    /// Enables the twin effect on a slime
    /// </summary>
    /// <param name="prismSlime">The slime to enable the effect on</param>
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

    /// <summary>
    /// Enables the sloomber effect on a slime
    /// </summary>
    /// <param name="prismSlime">The slime to enable the effect on</param>
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

    /// <summary>
    /// Disables the twin effect on a slime
    /// </summary>
    /// <param name="prismSlime">The slime to disable the effect on</param>
    /// <param name="index">The index of the appearance</param>
    /// <param name="index2">The index of the material</param>
    /// <param name="isSS">Whether the appearance is a secret style</param>
    /// <param name="structure">The index of the structure</param>
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

    /*
    
       public static void SetPalette(this SlimeAppearance app, Material slimeMaterial, SlimeDefinition definition)
       {
           app._colorPalette = new SlimeAppearance.Palette
           {
               Ammo = definition.color,
               Bottom = slimeMaterial.GetColor("_BottomColor"),
               Middle = slimeMaterial.GetColor("_MiddleColor"),
               Top = slimeMaterial.GetColor("_TopColor"),
           };
       }
     public static void SetSlimeMatTopColor(this Material mat, Color color) => mat.SetColor("_TopColor", color);
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