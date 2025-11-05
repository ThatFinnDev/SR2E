using System;
using SR2E.Prism.Lib;
using UnityEngine.Localization; 
namespace SR2E.Cotton;

public static class CottonSlimes
{







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


    public static IdentifiableType CreateGordoType(string name, Sprite icon, LocalizedString localizedName,
        string refID)
    {
        var type = Object.Instantiate(Get<IdentifiableType>("PinkGordo"));
        type.name = name + "Gordo";
        type.icon = icon;
        type.localizedName = localizedName;
        type.referenceId = refID;
        
        type.Prism_AddToGroup("GordoGroup");
        
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

}