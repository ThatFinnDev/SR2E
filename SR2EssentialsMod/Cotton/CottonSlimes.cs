using System;
using System.Linq;
using HarmonyLib;
using Il2Cpp;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppMonomiPark.SlimeRancher.Damage;
using Il2CppMonomiPark.SlimeRancher.Slime;
using Il2CppSystem.Linq;
using MelonLoader;
using SR2E.Cotton;
using SR2E.Managers;
using SR2E.Prism;
using SR2E.Prism.Lib;
using UnityEngine;
using UnityEngine.Localization; 
using static SR2E.Cotton.CottonLibrary;
namespace Cotton;

public static class CottonSlimes
{
    internal static bool DoesLargoComboExist(SlimeDefinition slime1, SlimeDefinition slime2)
    {
        try
        {
            if (gameContext.SlimeDefinitions.GetLargoByBaseSlimes(slime1, slime2) != null)
                return true;
        } catch { }
        try
        {
            if (gameContext.SlimeDefinitions.GetLargoByBaseSlimes(slime2, slime1) != null)
                return true;
        } catch { }
        return false;
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




    public static void AddToGroup(this IdentifiableType type, string groupName)
    {
        var group = Get<IdentifiableTypeGroup>(groupName);
        if (group.GetAllMembers().ToArray().Contains(type)) return;
        group._memberTypes.Add(type);
        group.GetRuntimeObject()._memberTypes.Add(type);
    }
    public static void AddToGroup(this IdentifiableTypeGroup group, string groupName)
    {
        var group2 = Get<IdentifiableTypeGroup>(groupName);
        if (group2._memberGroups.Contains(group)) return;
        group2._memberGroups.Add(group);
        group2.GetRuntimeObject()._memberGroups.Add(group);
    }
    public static bool IsInImmediateGroup(this IdentifiableType type, string groupName)
    {
        var group = Get<IdentifiableTypeGroup>(groupName);
        if (group == null) return false;
        return group._memberTypes.Contains(type);
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



    public static void SetStructColor(this SlimeAppearanceStructure structure, int id, Color color)
    {
        structure.DefaultMaterials[0].SetColor(id, color);
    }
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
    public static SlimeDefinition GetLargo(string name)
    {
        foreach (IdentifiableType type in largos.GetAllMembersArray())
            if (type.name.ToUpper() == name.ToUpper())
                return type.Cast<SlimeDefinition>();
        return null;
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
        
        PrismLibSaving.SetupForSaving(type,refID);
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

        gameContext.LookupDirector._gordoDict.Add(gordoType, gordo);
        gameContext.LookupDirector._gordoEntries.items.Add(gordo);
        
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