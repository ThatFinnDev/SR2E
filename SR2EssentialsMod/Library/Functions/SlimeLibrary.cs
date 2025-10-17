using System;
using System.Linq;
using HarmonyLib;
using Il2Cpp;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppMonomiPark.SlimeRancher.Damage;
using Il2CppMonomiPark.SlimeRancher.Slime;
using Il2CppSystem.Linq;
using MelonLoader;
using SR2E.Managers;
using UnityEngine;
using UnityEngine.Localization;

namespace CottonLibrary;

public static partial class Library
{
    internal static bool DoesLargoComboExist(SlimeDefinition slime1, SlimeDefinition slime2)
    {
        var ret = false;
        foreach (var largoCombo in largoCombos)
        {
            if ((largoCombo.Contains(slime1.name) && largoCombo.Contains(slime2.name)))
            {
                ret = true;
                break;
            }
        }

        return ret;
    }

    /// <summary>
    /// Create a SlimeDefinition.
    /// </summary>
    /// <param name="name">The name of the SlimeDefinition object.</param>
    /// <param name="vacColor">The vac color for the slime.</param>
    /// <param name="icon">The icon for the slime.</param>
    /// <param name="baseAppearance">The appearance to copy.</param>
    /// <param name="appearanceName">The name of the new appearance.</param>
    /// <param name="refID">The reference ID, used for saving. Recommended to do <c>SlimeDefinition.{name</c>,</param>
    /// <returns>The new SlimeDefinition that was created.</returns>
    public static SlimeDefinition CreateSlimeDef(
        string name,
        Color32 vacColor,
        Sprite icon,
        SlimeAppearance baseAppearance,
        string appearanceName,
        string refID)
        => CreateSlimeDef(
            name,
            vacColor,
            icon,
            baseAppearance,
            appearanceName,
            refID,
            false,
            0 // Empty LargoSettings flags
        );

    /// <summary>
    /// Create a SlimeDefinition.
    /// </summary>
    /// <param name="name">The name of the SlimeDefinition object.</param>
    /// <param name="vacColor">The vac color for the slime.</param>
    /// <param name="icon">The icon for the slime.</param>
    /// <param name="baseAppearance">The appearance to copy.</param>
    /// <param name="appearanceName">The name of the new appearance.</param>
    /// <param name="refID">The reference ID, used for saving. Recommended to do <c>SlimeDefinition.{name</c>,</param>
    /// <param name="largoable">If the slime can made into a largo.</param>
    /// <param name="largoSettings">The settings used for the largo's appearance.</param>
    /// <returns>The new SlimeDefinition that was created.</returns>
    public static SlimeDefinition CreateSlimeDef(
        string name,
        Color32 vacColor,
        Sprite icon,
        SlimeAppearance baseAppearance,
        string appearanceName,
        string refID,
        bool largoable,
        LargoSettings largoSettings)
    {
        SlimeDefinition slimeDef = Object.Instantiate(GetSlime("Pink"));
        Object.DontDestroyOnLoad(slimeDef);
        slimeDef.hideFlags = HideFlags.HideAndDontSave;
        slimeDef.name = name;
        slimeDef.AppearancesDefault = new Il2CppReferenceArray<SlimeAppearance>(1);

        SlimeAppearance appearance = Object.Instantiate(baseAppearance);
        Object.DontDestroyOnLoad(appearance);
        appearance.name = appearanceName;
        appearance._icon = icon;
        slimeDef.AppearancesDefault = slimeDef.AppearancesDefault.Add(appearance);
        if (slimeDef.AppearancesDefault[0] == null)
        {
            slimeDef.AppearancesDefault[0] = appearance;
        }

        for (int i = 0; i < slimeDef.AppearancesDefault[0].Structures.Count - 1; i++)
        {
            SlimeAppearanceStructure a = slimeDef.AppearancesDefault[0].Structures[i];
            var a2 = new SlimeAppearanceStructure(a);
            slimeDef.AppearancesDefault[0].Structures[i] = a2;
            if (a.DefaultMaterials.Count != 0)
            {
                a2.DefaultMaterials[0] = Object.Instantiate(a.DefaultMaterials[0]);
            }
        }

        SlimeDiet slimeDiet = CreateNewDiet();
        slimeDef.Diet = slimeDiet;
        slimeDef.color = vacColor;
        slimeDef.icon = icon;

        if (!slimeDef.IsLargo)
        {
            gameContext.SlimeDefinitions.Slimes = gameContext.SlimeDefinitions.Slimes.AddItem(slimeDef).ToCppArray();
            gameContext.SlimeDefinitions._slimeDefinitionsByIdentifiable.TryAdd(slimeDef, slimeDef);
        }

        INTERNAL_SetupLoadForIdent(refID, slimeDef);

        if (largoable)
        {
            createLargoActions.Add(() =>
            {
                foreach (var slime in baseSlimes.GetAllMembersArray())
                    if (slime.Cast<SlimeDefinition>().CanLargofy)
                        CreateCompleteLargo(slimeDef, slime.Cast<SlimeDefinition>(), largoSettings);
            });
        }

        baseSlimes._memberTypes.Add(slimeDef);

        baseSlimes.GetRuntimeObject()._memberTypes.Add(slimeDef);
        
        slimeDef.AppearancesDefault[0]._colorPalette = new SlimeAppearance.Palette
            { Ammo = vacColor, Bottom = vacColor, Middle = vacColor, Top = vacColor };
        
        slimeDef.CanLargofy = largoable;

        return slimeDef;
    }


    /// <summary>
    /// Sets the palette
    /// </summary>
    /// <param name="app">The slime appearance to set the palette for.</param>
    /// <param name="slimeMaterial">The material to get colors from.</param>
    /// <param name="definition">The slime definition for the ammo color.</param>
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

    internal static SlimeAppearance.Palette INTERNAL_GetTwinPalette(this SlimeAppearance app)
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

    internal static SlimeAppearance.Palette INTERNAL_GetSloomberPalette(this SlimeAppearance app)
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

        return new SlimeAppearance.Palette()
        {
            Ammo = new Color32(255, 255, 255, 255),
            Top = mat.GetColor("_SloomberTopColor"),
            Middle = mat.GetColor("_SloomberMiddleColor"),
            Bottom = mat.GetColor("_SloomberBottomColor"),
        };
    }

    internal static bool INTERNAL_GetLargoHasTwinEffect(SlimeAppearance slime1, SlimeAppearance slime2)
    {
        bool result = false;

        foreach (var structure in slime1._structures)
        {
            if (structure.DefaultMaterials.Count != 0 &&
                structure.DefaultMaterials[0].IsKeywordEnabled("_ENABLETWINEFFECT_ON"))
            {
                result = true;
                break;
            }
        }

        foreach (var structure in slime2._structures)
        {
            if (result) break;

            if (structure.DefaultMaterials.Count != 0 &&
                structure.DefaultMaterials[0].IsKeywordEnabled("_ENABLETWINEFFECT_ON"))
            {
                result = true;
                break;
            }
        }

        return result;
    }

    internal static bool INTERNAL_GetLargoHasSloomberEffect(SlimeAppearance slime1, SlimeAppearance slime2)
    {
        bool result = false;

        foreach (var structure in slime1._structures)
        {
            if (structure.DefaultMaterials.Count != 0 &&
                structure.DefaultMaterials[0].IsKeywordEnabled("_BODYCOLORING_SLOOMBER"))
            {
                result = true;
                break;
            }
        }

        foreach (var structure in slime2._structures)
        {
            if (result) break;

            if (structure.DefaultMaterials.Count != 0 &&
                structure.DefaultMaterials[0].IsKeywordEnabled("_BODYCOLORING_SLOOMBER"))
            {
                result = true;
                break;
            }
        }

        return result;
    }

    /// <summary>
    /// Merges two appearance structures together for a largo.
    /// </summary>
    /// <param name="slime1">Base slime #1</param>
    /// <param name="slime2">Base slime #2</param>
    /// <param name="settings">Settings that determine what structures this function merges, and what colors.</param>
    /// <returns>A new array of structures.</returns>
    public static Il2CppReferenceArray<SlimeAppearanceStructure> MergeStructures(SlimeAppearance slime1,
        SlimeAppearance slime2, LargoSettings settings)
    {
        var newStructures = new List<SlimeAppearanceStructure>(0);
        SlimeAppearance.Palette firstColor = slime1._colorPalette;
        SlimeAppearance.Palette firstColorTwin = slime1.INTERNAL_GetTwinPalette();
        SlimeAppearance.Palette firstColorSloomber = slime1.INTERNAL_GetSloomberPalette();

        SlimeAppearance.Palette secondColor = slime2._colorPalette;
        SlimeAppearance.Palette secondColorTwin = slime2.INTERNAL_GetTwinPalette();
        SlimeAppearance.Palette secondColorSloomber = slime2.INTERNAL_GetSloomberPalette();


        bool useTwinShader = INTERNAL_GetLargoHasTwinEffect(slime1, slime2);

        bool useSloomberShader = INTERNAL_GetLargoHasSloomberEffect(slime1, slime2);
        Material sloomberMat = GetSlime("Sloomber").AppearancesDefault[0]._structures[0].DefaultMaterials[0];

        foreach (var structure in slime1.Structures)
        {
            if (structure.Element.Type == SlimeAppearanceElement.ElementType.FACE ||
                structure.Element.Type == SlimeAppearanceElement.ElementType.FACE_ATTACH)
            {
                if (settings.HasFlag(LargoSettings.KeepFirstFace))
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
                            if (settings.HasFlag(LargoSettings.KeepFirstColor))
                            {
                                mat.SetColor("_TopColor", firstColor.Top);
                                mat.SetColor("_MiddleColor", firstColor.Middle);
                                mat.SetColor("_BottomColor", firstColor.Bottom);
                                mat.SetColor("_SpecColor", firstColor.Middle);
                            }
                            else if (settings.HasFlag(LargoSettings.KeepSecondColor))
                            {
                                mat.SetColor("_TopColor", secondColor.Top);
                                mat.SetColor("_MiddleColor", secondColor.Middle);
                                mat.SetColor("_BottomColor", secondColor.Bottom);
                                mat.SetColor("_SpecColor", secondColor.Middle);
                            }
                            else if (settings.HasFlag(LargoSettings.MergeColors))
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
                if (settings.HasFlag(LargoSettings.KeepFirstBody))
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
                            if (settings.HasFlag(LargoSettings.KeepFirstColor))
                            {
                                mat.SetColor("_TopColor", firstColor.Top);
                                mat.SetColor("_MiddleColor", firstColor.Middle);
                                mat.SetColor("_BottomColor", firstColor.Bottom);
                                mat.SetColor("_SpecColor", firstColor.Middle);
                            }
                            else if (settings.HasFlag(LargoSettings.KeepSecondColor))
                            {
                                mat.SetColor("_TopColor", secondColor.Top);
                                mat.SetColor("_MiddleColor", secondColor.Middle);
                                mat.SetColor("_BottomColor", secondColor.Bottom);
                                mat.SetColor("_SpecColor", secondColor.Middle);
                            }
                            else if (settings.HasFlag(LargoSettings.MergeColors))
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

                                if (settings.HasFlag(LargoSettings.KeepFirstTwinColor))
                                {
                                    mat.SetColor("_TwinTopColor", firstColorTwin.Top);
                                    mat.SetColor("_TwinMiddleColor", firstColorTwin.Middle);
                                    mat.SetColor("_TwinBottomColor", firstColor.Bottom);
                                }
                                else if (settings.HasFlag(LargoSettings.KeepSecondTwinColor))
                                {
                                    mat.SetColor("_TwinTopColor", secondColorTwin.Top);
                                    mat.SetColor("_TwinMiddleColor", secondColorTwin.Middle);
                                    mat.SetColor("_TwinBottomColor", secondColorTwin.Bottom);
                                }
                                else if (settings.HasFlag(LargoSettings.MergeTwinColors))
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
                                    mat.SetTexture("_SloomberColorOverlay",
                                        sloomberMat.GetTexture("_SloomberColorOverlay"));
                                    mat.SetTexture("_SloomberStarMask",
                                        sloomberMat.GetTexture("_SloomberStarMask"));

                                    mat.EnableKeyword("_BODYCOLORING_SLOOMBER");
                                    mat.DisableKeyword("_BODYCOLORING_DEFAULT");

                                    if (settings.HasFlag(LargoSettings.KeepFirstColor))
                                    {
                                        mat.SetColor("_SloomberTopColor", firstColorSloomber.Top);
                                        mat.SetColor("_SloomberMiddleColor", firstColorSloomber.Middle);
                                        mat.SetColor("_SloomberBottomColor", firstColorSloomber.Bottom);
                                    }
                                    else if (settings.HasFlag(LargoSettings.KeepSecondColor))
                                    {
                                        mat.SetColor("_SloomberTopColor", secondColorSloomber.Top);
                                        mat.SetColor("_SloomberMiddleColor", secondColorSloomber.Middle);
                                        mat.SetColor("_SloomberBottomColor", secondColorSloomber.Bottom);
                                    }
                                    else if (settings.HasFlag(LargoSettings.MergeColors))
                                    {
                                        var top = Color.Lerp(firstColorSloomber.Top, secondColorSloomber.Top, 0.5f);
                                        var middle = Color.Lerp(firstColorSloomber.Middle,
                                            secondColorSloomber.Middle,
                                            0.5f);
                                        var bottom = Color.Lerp(firstColorSloomber.Bottom,
                                            secondColorSloomber.Bottom,
                                            0.5f);
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
            else if (structure != null && !newStructures.Contains(structure) &&
                     structure.DefaultMaterials.Length != 0)
            {
                var newStructure = new SlimeAppearanceStructure(structure);
                newStructures.Add(newStructure);
                var mat = Object.Instantiate(structure.DefaultMaterials[0]);
                structure.DefaultMaterials[0] = mat;

                try
                {
                    if (settings.HasFlag(LargoSettings.KeepFirstColor))
                    {
                        mat.SetColor("_TopColor", firstColor.Top);
                        mat.SetColor("_MiddleColor", firstColor.Middle);
                        mat.SetColor("_BottomColor", firstColor.Bottom);
                        mat.SetColor("_SpecColor", firstColor.Middle);
                    }
                    else if (settings.HasFlag(LargoSettings.KeepSecondColor))
                    {
                        mat.SetColor("_TopColor", secondColor.Top);
                        mat.SetColor("_MiddleColor", secondColor.Middle);
                        mat.SetColor("_BottomColor", secondColor.Bottom);
                        mat.SetColor("_SpecColor", secondColor.Middle);
                    }
                    else if (settings.HasFlag(LargoSettings.MergeColors))
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

                        if (settings.HasFlag(LargoSettings.KeepFirstColor))
                        {
                            mat.SetColor("_SloomberTopColor", firstColorSloomber.Top);
                            mat.SetColor("_SloomberMiddleColor", firstColorSloomber.Middle);
                            mat.SetColor("_SloomberBottomColor", firstColorSloomber.Bottom);
                        }
                        else if (settings.HasFlag(LargoSettings.KeepSecondColor))
                        {
                            mat.SetColor("_SloomberTopColor", secondColorSloomber.Top);
                            mat.SetColor("_SloomberMiddleColor", secondColorSloomber.Middle);
                            mat.SetColor("_SloomberBottomColor", secondColorSloomber.Bottom);
                        }
                        else if (settings.HasFlag(LargoSettings.MergeColors))
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

                        if (settings.HasFlag(LargoSettings.KeepFirstTwinColor))
                        {
                            mat.SetColor("_TwinTopColor", firstColorTwin.Top);
                            mat.SetColor("_TwinMiddleColor", firstColorTwin.Middle);
                            mat.SetColor("_TwinBottomColor", firstColorTwin.Bottom);
                        }
                        else if (settings.HasFlag(LargoSettings.KeepSecondTwinColor))
                        {
                            mat.SetColor("_TwinTopColor", secondColorTwin.Top);
                            mat.SetColor("_TwinMiddleColor", secondColorTwin.Middle);
                            mat.SetColor("_TwinBottomColor", secondColorTwin.Bottom);
                        }
                        else if (settings.HasFlag(LargoSettings.MergeTwinColors))
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
                if (settings.HasFlag(LargoSettings.KeepSecondFace))
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
                            if (settings.HasFlag(LargoSettings.KeepFirstColor))
                            {
                                mat.SetColor("_TopColor", firstColor.Top);
                                mat.SetColor("_MiddleColor", firstColor.Middle);
                                mat.SetColor("_BottomColor", firstColor.Bottom);
                                mat.SetColor("_SpecColor", firstColor.Middle);
                            }
                            else if (settings.HasFlag(LargoSettings.KeepSecondColor))
                            {
                                mat.SetColor("_TopColor", secondColor.Top);
                                mat.SetColor("_MiddleColor", secondColor.Middle);
                                mat.SetColor("_BottomColor", secondColor.Bottom);
                                mat.SetColor("_SpecColor", secondColor.Middle);
                            }
                            else if (settings.HasFlag(LargoSettings.MergeColors))
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

                                if (settings.HasFlag(LargoSettings.KeepFirstColor))
                                {
                                    mat.SetColor("_SloomberTopColor", firstColorSloomber.Top);
                                    mat.SetColor("_SloomberMiddleColor", firstColorSloomber.Middle);
                                    mat.SetColor("_SloomberBottomColor", firstColorSloomber.Bottom);
                                }
                                else if (settings.HasFlag(LargoSettings.KeepSecondColor))
                                {
                                    mat.SetColor("_SloomberTopColor", secondColorSloomber.Top);
                                    mat.SetColor("_SloomberMiddleColor", secondColorSloomber.Middle);
                                    mat.SetColor("_SloomberBottomColor", secondColorSloomber.Bottom);
                                }
                                else if (settings.HasFlag(LargoSettings.MergeColors))
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

                                if (settings.HasFlag(LargoSettings.KeepFirstTwinColor))
                                {
                                    mat.SetColor("_TwinTopColor", firstColorTwin.Top);
                                    mat.SetColor("_TwinMiddleColor", firstColorTwin.Middle);
                                    mat.SetColor("_TwinBottomColor", firstColorTwin.Bottom);
                                }
                                else if (settings.HasFlag(LargoSettings.KeepSecondTwinColor))
                                {
                                    mat.SetColor("_TwinTopColor", secondColorTwin.Top);
                                    mat.SetColor("_TwinMiddleColor", secondColorTwin.Middle);
                                    mat.SetColor("_TwinBottomColor", secondColorTwin.Bottom);
                                }
                                else if (settings.HasFlag(LargoSettings.MergeTwinColors))
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
                if (settings.HasFlag(LargoSettings.KeepSecondBody))
                {
                    if (!newStructures.Contains(structure))
                    {
                        var newStructure = new SlimeAppearanceStructure(structure);
                        newStructures.Add(newStructure);
                        var mat = Object.Instantiate(structure.DefaultMaterials[0]);
                        newStructure.DefaultMaterials[0] = mat;


                        try
                        {
                            if (settings.HasFlag(LargoSettings.KeepFirstColor))
                            {
                                mat.SetColor("_TopColor", firstColor.Top);
                                mat.SetColor("_MiddleColor", firstColor.Middle);
                                mat.SetColor("_BottomColor", firstColor.Bottom);
                                mat.SetColor("_SpecColor", firstColor.Middle);
                            }
                            else if (settings.HasFlag(LargoSettings.KeepSecondColor))
                            {
                                mat.SetColor("_TopColor", secondColor.Top);
                                mat.SetColor("_MiddleColor", secondColor.Middle);
                                mat.SetColor("_BottomColor", secondColor.Bottom);
                                mat.SetColor("_SpecColor", secondColor.Middle);
                            }
                            else if (settings.HasFlag(LargoSettings.MergeColors))
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

                                if (settings.HasFlag(LargoSettings.KeepFirstColor))
                                {
                                    mat.SetColor("_SloomberTopColor", firstColorSloomber.Top);
                                    mat.SetColor("_SloomberMiddleColor", firstColorSloomber.Middle);
                                    mat.SetColor("_SloomberBottomColor", firstColorSloomber.Bottom);
                                }
                                else if (settings.HasFlag(LargoSettings.KeepSecondColor))
                                {
                                    mat.SetColor("_SloomberTopColor", secondColorSloomber.Top);
                                    mat.SetColor("_SloomberMiddleColor", secondColorSloomber.Middle);
                                    mat.SetColor("_SloomberBottomColor", secondColorSloomber.Bottom);
                                }
                                else if (settings.HasFlag(LargoSettings.MergeColors))
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

                                if (settings.HasFlag(LargoSettings.KeepFirstTwinColor))
                                {
                                    mat.SetColor("_TwinTopColor", firstColorTwin.Top);
                                    mat.SetColor("_TwinMiddleColor", firstColorTwin.Middle);
                                    mat.SetColor("_TwinBottomColor", firstColorTwin.Bottom);
                                }
                                else if (settings.HasFlag(LargoSettings.KeepSecondTwinColor))
                                {
                                    mat.SetColor("_TwinTopColor", secondColorTwin.Top);
                                    mat.SetColor("_TwinMiddleColor", secondColorTwin.Middle);
                                    mat.SetColor("_TwinBottomColor", secondColorTwin.Bottom);
                                }
                                else if (settings.HasFlag(LargoSettings.MergeTwinColors))
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
                    if (settings.HasFlag(LargoSettings.KeepFirstColor))
                    {
                        mat.SetColor("_TopColor", firstColor.Top);
                        mat.SetColor("_MiddleColor", firstColor.Middle);
                        mat.SetColor("_BottomColor", firstColor.Bottom);
                        mat.SetColor("_SpecColor", firstColor.Middle);
                    }
                    else if (settings.HasFlag(LargoSettings.KeepSecondColor))
                    {
                        mat.SetColor("_TopColor", secondColor.Top);
                        mat.SetColor("_MiddleColor", secondColor.Middle);
                        mat.SetColor("_BottomColor", secondColor.Bottom);
                        mat.SetColor("_SpecColor", secondColor.Middle);
                    }
                    else if (settings.HasFlag(LargoSettings.MergeColors))
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

                        if (settings.HasFlag(LargoSettings.KeepFirstColor))
                        {
                            mat.SetColor("_SloomberTopColor", firstColorSloomber.Top);
                            mat.SetColor("_SloomberMiddleColor", firstColorSloomber.Middle);
                            mat.SetColor("_SloomberBottomColor", firstColorSloomber.Bottom);
                        }
                        else if (settings.HasFlag(LargoSettings.KeepSecondColor))
                        {
                            mat.SetColor("_SloomberTopColor", secondColorSloomber.Top);
                            mat.SetColor("_SloomberMiddleColor", secondColorSloomber.Middle);
                            mat.SetColor("_SloomberBottomColor", secondColorSloomber.Bottom);
                        }
                        else if (settings.HasFlag(LargoSettings.MergeColors))
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

                        if (settings.HasFlag(LargoSettings.KeepFirstTwinColor))
                        {
                            mat.SetColor("_TwinTopColor", firstColorTwin.Top);
                            mat.SetColor("_TwinMiddleColor", firstColorTwin.Middle);
                            mat.SetColor("_TwinBottomColor", firstColorTwin.Bottom);
                        }
                        else if (settings.HasFlag(LargoSettings.KeepSecondTwinColor))
                        {
                            mat.SetColor("_TwinTopColor", secondColorTwin.Top);
                            mat.SetColor("_TwinMiddleColor", secondColorTwin.Middle);
                            mat.SetColor("_TwinBottomColor", secondColorTwin.Bottom);
                        }
                        else if (settings.HasFlag(LargoSettings.MergeTwinColors))
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

    /// <summary>
    /// Merges two slime's favorite toys list. Use for largos
    /// </summary>
    /// <param name="slimeOne">The first slime</param>
    /// <param name="slimeTwo">The other slime</param>
    /// <returns>An IL2Cpp array of ToyDefinition</returns>
    public static Il2CppReferenceArray<ToyDefinition> MergeFavoriteToys(SlimeDefinition slimeOne, SlimeDefinition slimeTwo)
    {
        List<ToyDefinition> toys = new List<ToyDefinition>();

        foreach (var toy in slimeOne.FavoriteToyIdents)
            toys.Add(toy);
        foreach (var toy in slimeTwo.FavoriteToyIdents)
            toys.Add(toy);

        return toys.ToArray();
    }

    public class LargoOverrides
    {

        public string overrideTranslation = "{0} {1} Largo";

        public string overridePediaSuffix = "{0}_{1}_largo";
    }

    /// <summary>
    /// Merges the components on two GameObjects.
    /// </summary>
    /// <param name="obj">The target GameObject.</param>
    /// <param name="oldOne">The first GameObject to get components from.</param>
    /// <param name="oldTwo">The other GameObject to get components from, optional.</param>
    public static void MergeComponents(GameObject obj, GameObject oldOne, GameObject? oldTwo)
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
                if (obj.TryAddComponentTypeIfNull(component.Value.Item1.GetIl2CppType(), out var newComp))
                    newComp.CopyFields(component.Value.Item1);
                MelonLogger.Msg(component.Key);
            }
        
        MelonLogger.Msg("Merged components!");
    }

    /// <summary>
    /// Creates a fully completed largo.
    /// </summary>
    /// <param name="slimeOne">Base slime #1</param>
    /// <param name="slimeTwo">Base slime #2</param>
    /// <param name="settings">The appearance settings.</param>
    /// <param name="overrides">The overrides for the translations.</param>
    /// <returns>The SlimeDefinition created for the largo.</returns>
    public static SlimeDefinition CreateCompleteLargo(SlimeDefinition slimeOne, SlimeDefinition slimeTwo,
        LargoSettings settings, LargoOverrides overrides = null)
    {
        if (DoesLargoComboExist(slimeOne, slimeTwo)) return null;

        SlimeDefinition baseLargo = GetLargo("PinkRock");

        if (slimeOne.IsLargo || slimeTwo.IsLargo)
            return null;

        SlimeDefinition largoDef = Object.Instantiate(baseLargo);
        largoDef.BaseSlimes = new[]
        {
            slimeOne, slimeTwo
        };
        largoDef.SlimeModules = new[]
        {
            Get<GameObject>("moduleSlime" + slimeOne.name), Get<GameObject>("moduleSlime" + slimeTwo.name)
        };


        if (overrides != null)
            largoDef._pediaPersistenceSuffix = string.Format(overrides.overridePediaSuffix, slimeOne.name.ToLower(),
                slimeTwo.name.ToLower());
        else
            largoDef._pediaPersistenceSuffix = slimeOne.name.ToLower() + "_" + slimeTwo.name.ToLower() + "_largo";

        largoDef.referenceId = "SlimeDefinition." + slimeOne.name + slimeTwo.name;

        MelonLogger.Error(largoDef.referenceId);
        
        if (overrides != null)
            largoDef.localizedName =
                AddTranslation(string.Format(overrides.overrideTranslation, slimeOne.name, slimeTwo.name),
                    "l." + largoDef._pediaPersistenceSuffix);
        else
            largoDef.localizedName = AddTranslation(slimeOne.name + " " + slimeTwo.name + " Largo",
                "l." + largoDef._pediaPersistenceSuffix);


        largoDef.FavoriteToyIdents = new Il2CppReferenceArray<ToyDefinition>(MergeFavoriteToys(slimeOne, slimeTwo));

        Object.DontDestroyOnLoad(largoDef);
        largoDef.hideFlags = HideFlags.HideAndDontSave;
        largoDef.name = slimeOne.name + slimeTwo.name;

        largoDef.prefab = Object.Instantiate(baseLargo.prefab, rootOBJ.transform);
        largoDef.prefab.name = $"slime{slimeOne.name + slimeTwo.name}";
        largoDef.prefab.GetComponent<Identifiable>().identType = largoDef;
        largoDef.prefab.GetComponent<SlimeEat>().SlimeDefinition = largoDef;
        largoDef.prefab.GetComponent<SlimeAppearanceApplicator>().SlimeDefinition = largoDef;
        largoDef.prefab.GetComponent<PlayWithToys>().SlimeDefinition = largoDef;
        largoDef.prefab.GetComponent<ReactToToyNearby>().SlimeDefinition = largoDef;
        largoDef.prefab.RemoveComponent<RockSlimeRoll>();
        largoDef.prefab.RemoveComponent<DamagePlayerOnTouch>();

        SlimeAppearance appearance = Object.Instantiate(baseLargo.AppearancesDefault[0]);
        largoDef.AppearancesDefault[0] = appearance;
        Object.DontDestroyOnLoad(appearance);
        appearance.name = slimeOne.AppearancesDefault[0].name + slimeTwo.AppearancesDefault[0].name;

        appearance._dependentAppearances = new[]
        {
            slimeOne.AppearancesDefault[0], slimeTwo.AppearancesDefault[0]
        };

        appearance._structures = MergeStructures(appearance._dependentAppearances[0],
            appearance._dependentAppearances[1], settings);

        try
        {
            largoDef.Diet = MergeDiet(slimeOne.Diet, slimeTwo.Diet);
        }
        catch
        {
            if (settings.HasFlag(LargoSettings.KeepFirstBody)) largoDef.Diet = slimeOne.Diet;
            else if (settings.HasFlag(LargoSettings.KeepSecondBody)) largoDef.Diet = slimeTwo.Diet;
            else
            {
                largoDef.Diet = slimeOne.Diet;
                MelonLogger.BigError("Largo Error",
                    "Failed to merge diet, and largo settings are incorrectly set! Defaulting to slime 1's diet.");
                MelonLogger.BigError("Largo Error",
                    "Failed to merge diet, and largo settings are incorrectly set! Defaulting to slime 1's diet.");
                MelonLogger.BigError("Largo Error",
                    "Failed to merge diet, and largo settings are incorrectly set! Defaulting to slime 1's diet.");
            }
        }

        largoDef.RefreshEatmap();

        slimeDefinitions.Slimes = slimeDefinitions.Slimes.Add(largoDef);
        slimeDefinitions._slimeDefinitionsByIdentifiable.TryAdd(largoDef, largoDef);
        mainAppearanceDirector.RegisterDependentAppearances(largoDef, largoDef.AppearancesDefault[0]);
        mainAppearanceDirector.UpdateChosenSlimeAppearance(largoDef, largoDef.AppearancesDefault[0]);

        largoDef.AddToGroup("LargoGroup");
        largoDef.AddToGroup("SlimesGroup");
        INTERNAL_SetupLoadForIdent(largoDef.referenceId, largoDef);

        slimeOne.RefreshEatmap();
        slimeTwo.RefreshEatmap();

        largoCombos.Add($"{slimeOne.name} {slimeTwo.name}");

        slimeDefinitions.RefreshDefinitions();
        slimeDefinitions.RefreshIndexes();

        MergeComponents(largoDef.prefab, slimeOne.prefab, slimeTwo.prefab);

        return largoDef;
    }

    public static void AddStructure(this SlimeAppearance appearance, SlimeAppearanceStructure structure)
    {
        appearance.Structures.Add(structure);
    }

    public static SlimeDiet MergeDiet(this SlimeDiet firstDiet, SlimeDiet secondDiet)
    {

        var mergedDiet = CreateNewDiet();

        mergedDiet.EatMap.AddListRangeNoMultiple(firstDiet.EatMap);
        mergedDiet.EatMap.AddListRangeNoMultiple(secondDiet.EatMap);

        mergedDiet.AdditionalFoodIdents =
            mergedDiet.AdditionalFoodIdents.AddRangeNoMultiple(firstDiet.AdditionalFoodIdents);
        mergedDiet.AdditionalFoodIdents =
            mergedDiet.AdditionalFoodIdents.AddRangeNoMultiple(secondDiet.AdditionalFoodIdents);

        mergedDiet.FavoriteIdents = mergedDiet.FavoriteIdents.AddRangeNoMultiple(firstDiet.FavoriteIdents);
        mergedDiet.FavoriteIdents = mergedDiet.FavoriteIdents.AddRangeNoMultiple(secondDiet.FavoriteIdents);

        mergedDiet.MajorFoodIdentifiableTypeGroups =
            mergedDiet.MajorFoodIdentifiableTypeGroups.AddRangeNoMultiple(firstDiet.MajorFoodIdentifiableTypeGroups);
        mergedDiet.MajorFoodIdentifiableTypeGroups =
            mergedDiet.MajorFoodIdentifiableTypeGroups.AddRangeNoMultiple(secondDiet.MajorFoodIdentifiableTypeGroups);

        mergedDiet.ProduceIdents = mergedDiet.ProduceIdents.AddRangeNoMultiple(firstDiet.ProduceIdents);
        mergedDiet.ProduceIdents = mergedDiet.ProduceIdents.AddRangeNoMultiple(secondDiet.ProduceIdents);

        return mergedDiet;
    }

    public static void SwitchSlimeAppearances(this SlimeDefinition slimeOneDef, SlimeDefinition slimeTwoDef)
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

    }

    public static SlimeDiet.EatMapEntry CreateEatmap(SlimeEmotions.Emotion driver, float mindrive,
        IdentifiableType produce, IdentifiableType eat, IdentifiableType becomes)
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

    public static SlimeDiet.EatMapEntry CreateEatmap(SlimeEmotions.Emotion driver, float mindrive,
        IdentifiableType produce, IdentifiableType eat)
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

    public static void ModifyEatmap(this SlimeDiet.EatMapEntry eatmap, SlimeEmotions.Emotion driver, float mindrive,
        IdentifiableType produce, IdentifiableType eat, IdentifiableType becomes)
    {
        eatmap.EatsIdent = eat;
        eatmap.BecomesIdent = becomes;
        eatmap.ProducesIdent = produce;
        eatmap.Driver = driver;
        eatmap.MinDrive = mindrive;
    }

    public static void ModifyEatmap(this SlimeDiet.EatMapEntry eatmap, SlimeEmotions.Emotion driver, float mindrive,
        IdentifiableType produce, IdentifiableType eat)
    {
        eatmap.EatsIdent = eat;
        eatmap.ProducesIdent = produce;
        eatmap.Driver = driver;
        eatmap.MinDrive = mindrive;
    }

    public static void AddProduceIdent(this SlimeDefinition slimeDef, IdentifiableType ident)
    {
        slimeDef.Diet.ProduceIdents = slimeDef.Diet.ProduceIdents.Add(ident);
    }

    public static void SetProduceIdent(this SlimeDefinition slimeDef, IdentifiableType ident, int index)
    {
        slimeDef.Diet.ProduceIdents[index] = ident;
    }

    public static void AddExtraEatIdent(this SlimeDefinition slimeDef, IdentifiableType ident)
    {
        slimeDef.Diet.AdditionalFoodIdents = slimeDef.Diet.AdditionalFoodIdents.Add(ident);

    }

    public static void SetFavoriteProduceCount(this SlimeDefinition slimeDef, int count)
    {
        slimeDef.Diet.FavoriteProductionCount = count;
    }

    public static void AddFavorite(this SlimeDefinition slimeDef, IdentifiableType id)
    {
        slimeDef.Diet.FavoriteIdents = slimeDef.Diet.FavoriteIdents.Add(id);
    }

    public static void AddEatmapToSlime(this SlimeDefinition slimeDef, SlimeDiet.EatMapEntry eatmap)
    {
        if (!customEatmaps.TryGetValue(slimeDef, out var eatmaps))
        {
            eatmaps = new List<SlimeDiet.EatMapEntry>();
            customEatmaps.Add(slimeDef, eatmaps);
        }
        eatmaps.Add(eatmap);
        slimeDef.Diet.EatMap.Add(eatmap);
    }

    public static void SetStructColor(this SlimeAppearanceStructure structure, int id, Color color)
    {
        structure.DefaultMaterials[0].SetColor(id, color);
    }

    public static void RefreshEatmap(this SlimeDefinition def)
    {
        def.Diet.RefreshEatMap(slimeDefinitions, def);
    }

    public static void ChangeSlimeFoodGroup(this SlimeDefinition def, IdentifiableTypeGroup FG, int index)
    {
        def.Diet.MajorFoodIdentifiableTypeGroups[index] = FG;
    }

    public static void AddSlimeFoodGroup(this SlimeDefinition def, IdentifiableTypeGroup FG)
    {
        def.Diet.MajorFoodIdentifiableTypeGroups = def.Diet.MajorFoodIdentifiableTypeGroups.Add(FG);
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
        // 0.6 - Unstable identifiables
        StableResourceIdentifiableTypeGroup = Get<IdentifiableTypeGroup>("StableResourcesGroup"),
        UnstableResourceIdentifiableTypeGroup = Get<IdentifiableTypeGroup>("UnstableResourcesGroup"),
        UnstablePlort = GetPlort("UnstablePlort")
    };

    public static SlimeDefinition GetSlime(string name)
    {
        foreach (IdentifiableType type in slimes.GetAllMembersArray())
            if (type.name.ToUpper() == name.ToUpper())
                return type.Cast<SlimeDefinition>();

        return null;
    }

    public static void SetSlimeColorSpecific(this SlimeDefinition slimeDef, Color32 top, Color32 middle, Color32 bottom,
        Color32 special, int index, int index2, bool isSS, int structure)
    {
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

    public static void SetSlimeColor(this SlimeDefinition slimeDef, Color32 top, Color32 middle, Color32 bottom)
    {
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

    public static void SetTwinColor(this SlimeDefinition slimeDef, Color32 top, Color32 middle, Color32 bottom)
    {
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
    public static void SetSloomberColor(this SlimeDefinition slimeDef, Color32 top, Color32 middle, Color32 bottom)
    {
        for (int i = 0; i < slimeDef.AppearancesDefault[0].Structures.Count - 1; i++)
        {
            SlimeAppearanceStructure a = slimeDef.AppearancesDefault[0].Structures[i];
            var mat = a.DefaultMaterials[0];

            mat.SetColor("_SloomberTopColor", top);
            mat.SetColor("_SloomberMiddleColor", middle);
            mat.SetColor("_SloomberBottomColor", bottom);
        }
    }
    
    public static void SetTwinColorSpecific(this SlimeDefinition slimeDef, Color32 top, Color32 middle, Color32 bottom,
        int index, int index2, bool isSS, int structure)
    {
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

    public static void SetSloomberColorSpecific(this SlimeDefinition slimeDef, Color32 top, Color32 middle, Color32 bottom,
        int index, int index2, bool isSS, int structure)
    {
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
    public static void EnableTwinEffectSpecific(this SlimeDefinition slimeDef, int index, int index2, bool isSS, int structure)
    {
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
    public static void EnableTwinEffect(this SlimeDefinition slimeDef)
    {
        for (int i = 0; i < slimeDef.AppearancesDefault[0].Structures.Count - 1; i++)
        {
            SlimeAppearanceStructure a = slimeDef.AppearancesDefault[0].Structures[i];
            var mat = a.DefaultMaterials[0];
            
            mat.EnableKeyword("_ENABLETWINEFFECT_ON");
        }
    }

    // Sloomber effect uses the shader keyword "_BODYCOLORING_SLOOMBER"
    public static void EnableSloomberEffect(this SlimeDefinition slimeDef)
    {
        for (int i = 0; i < slimeDef.AppearancesDefault[0].Structures.Count - 1; i++)
        {
            SlimeAppearanceStructure a = slimeDef.AppearancesDefault[0].Structures[i];
            var mat = a.DefaultMaterials[0];
            
            mat.EnableKeyword("_BODYCOLORING_SLOOMBER");
        }
    }

    public static void DisableTwinEffect(this SlimeDefinition slimeDef, int index, int index2, bool isSS, int structure)
    {

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
    }

    public static SlimeDefinition GetBaseSlime(string name)
    {
        foreach (IdentifiableType type in baseSlimes.GetAllMembersArray())
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

    /// <summary>
    /// Automated largos from the <see cref="Library.CreateSlimeDef"/> function will use <c>KeepFirst</c> values for your slime.
    /// </summary>
    [Flags]
    public enum LargoSettings : ulong
    {
        KeepFirstBody = 1 << 0,
        KeepSecondBody = 1 << 1,
        KeepFirstFace = 1 << 2,
        KeepSecondFace = 1 << 3,
        KeepFirstColor = 1 << 4,
        KeepSecondColor = 1 << 5,
        KeepFirstTwinColor = 1 << 6,
        KeepSecondTwinColor = 1 << 7,
        MergeColors = 1 << 8,
        MergeTwinColors = 1 << 9,
    }

    public static SlimeDiet.EatMapEntry? GetEatMap(SlimeDiet diet, IdentifiableType type) =>
        diet.EatMap._items.FirstOrDefault(eatmap => eatmap.EatsIdent == type);

    public static bool TryGetEatMap(SlimeDiet diet, IdentifiableType type, out SlimeDiet.EatMapEntry eatMap)
    {
        SlimeDiet.EatMapEntry eatMapEntry = null;

        try
        {
            eatMapEntry = GetEatMap(diet, type);
            if (eatMapEntry != null)
            {
                eatMap = eatMapEntry;
                return true;
            }

            eatMap = null;
            return false;
        }
        catch
        {
            eatMap = null;
            return false;
        }
    }


    public static SlimeDiet.EatMapEntry[] GetEatMapsByName(SlimeDiet diet, string group) =>
        diet?.EatMap?._items.Where(map => { 
            if (map == null) return false;
            if (map.EatsIdent == null) return false;
            return map.EatsIdent.name.Contains(group);
        }).ToArray();

    public static SlimeDiet.EatMapEntry[] GetEatMapsByIdentifiableGroup(SlimeDiet diet, IdentifiableTypeGroup group) =>
        diet?.EatMap?._items.Where(map => { 
            if (map == null) return false;
            if (map.EatsIdent == null) return false;
            if (group == null) return false;
            var members = group.GetAllMembers();
            if (members == null) return false;

            return members.Contains(map.EatsIdent);
        }).ToArray();

    public static IdentifiableType CreateGordoType(string name, Sprite icon, LocalizedString localizedName,
        string refID)
    {
        var type = Object.Instantiate(Get<IdentifiableType>("PinkGordo"));
        type.name = name + "Gordo";
        type.icon = icon;
        type.localizedName = localizedName;
        type.referenceId = refID;
        
        type.AddToGroup("GordoGroup");
        
        INTERNAL_SetupLoadForIdent(refID, type);
        return type;
    }

    public static GameObject CreateGordoObject(GameObject baseObj, IdentifiableType gordoType,
        SlimeDefinition baseSlime, GordoFaceData face, params Material[] materials)
    {
        var gordo = baseObj.CopyObject();
        gordo.GetComponent<GordoIdentifiable>().identType = gordoType;
        gordo.GetComponent<GordoEat>().SlimeDefinition = baseSlime;
        
        var faceComp = gordo.GetComponent<GordoFaceComponents>();
        if (face.eyesBlink != null)
            faceComp.BlinkEyes = face.eyesBlink;
        if (face.eyesNormal != null)
            faceComp.StrainEyes = face.eyesNormal;
        if (face.mouthHappy != null)
            faceComp.HappyMouth = face.mouthHappy;
        if (face.mouthChompOpen != null)
            faceComp.ChompOpenMouth = face.mouthChompOpen;
        if (face.mouthEating != null)
            faceComp.StrainMouth = face.mouthEating;
        
        var attachments = gordo.transform.GetComponentsInChildren<SkinnedMeshRenderer>();
        var i = 0;
        foreach (var att in attachments)
        {
            if (materials.Length > i)
                if (materials[i] != null)
                    att.material = materials[i];

            i++;
        }

        GameContext.Instance.LookupDirector._gordoDict.Add(gordoType, gordo);
        GameContext.Instance.LookupDirector._gordoEntries.items.Add(gordo);
        
        gordoType.prefab = gordo;
        
        return gordo;
    }

    public static void DebuggingPrintGordoAttachments(GameObject baseObj)
    {
        string print = "[DEBUG] Printing gordo attachments for " + baseObj.name + "\n";
        int i = 0;
        foreach (var obj in baseObj.GetComponentsInChildren<SkinnedMeshRenderer>())
        {
            print += $"[index: {i}] {obj.gameObject.name} -- {obj.material.name}\n";
        }

        MelonLogger.Msg(print);
    }

    public class GordoFaceData
    {
        public Material eyesBlink;
        public Material eyesNormal;
        public Material mouthHappy;
        public Material mouthChompOpen;
        public Material mouthEating;
    }

    public static void SetRequiredBait(this GameObject gordo, IdentifiableType baitType)
    {
        var idComp = gordo.GetComponent<GordoIdentifiable>();
        if (!idComp)
            throw new InvalidCastException("You cannot set the bait for this object as it is not a gordo!");
        gordoBaitDict.Add(baitType.name, idComp.identType);
    }
    internal static Dictionary<string, IdentifiableType> gordoBaitDict = new Dictionary<string, IdentifiableType>();

    public static Dictionary<SlimeDefinition, List<SlimeDiet.EatMapEntry>> customEatmaps = new Dictionary<SlimeDefinition, List<SlimeDiet.EatMapEntry>>();
}