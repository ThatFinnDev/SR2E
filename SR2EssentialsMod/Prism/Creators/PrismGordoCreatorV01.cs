using Il2CppInterop.Runtime.InteropTypes.Arrays;
using SR2E.Prism.Data;
using SR2E.Prism.Lib;
using UnityEngine.Localization;

namespace SR2E.Prism.Creators;

public class PrismGordoCreatorV01
{
    private PrismGordo _createdGordo;
    
    public Sprite icon;
    public LocalizedString localized;
    public PrismBaseSlime baseSlime = null;
    public string referenceID => "IdentifiableType.Modded" + baseSlime._slimeDefinition.name +"Gordo";

    public int customMaxEatCount = 0;
    public Material customEyesBlink;
    public Material customEyesNormal;
    public Material customMouthHappy;
    public Material customMouthChompOpen;
    public Material customMouthEating;
    
    public PrismGordoCreatorV01(PrismBaseSlime baseSlime, Sprite icon, LocalizedString localized)
    {
        this.baseSlime = baseSlime;
        this.icon = icon;
        this.localized = localized;
    }
    
    public bool IsValid()
    {
        if (baseSlime==null) return false;
        if (icon==null) return false;
        if (localized==null) return false;
        return true;
    }


    private static IdentifiableType baseType = null;
    
    public PrismGordo CreateGordo()
    {
        if (!IsValid()) return null;
        if (_createdGordo != null) return _createdGordo;

        var baseMaterial = baseSlime.GetBaseMaterial();
        if (baseMaterial == null) return null;
        if (baseType == null) baseType = Get<IdentifiableType>("PinkGordo");
        if (baseType == null) return null;
        var gordoType = Object.Instantiate(baseType);
        gordoType.name = baseSlime._slimeDefinition.name.ToLower() + "ModdedGordo";
        gordoType.icon = icon;
        gordoType.localizedName = localized;
        gordoType.referenceId = referenceID;
        gordoType._pediaPersistenceSuffix=baseSlime._slimeDefinition.name.ToLower()+"_gordo";
        
        gordoType.Prism_AddToGroup("GordoGroup");
        
        PrismLibSaving.SetupForSaving(gordoType,referenceID);

        var gordo = baseType.prefab.CopyObject();
        gordo.GetComponent<GordoIdentifiable>().identType = gordoType;
        gordo.GetComponent<GordoEat>().SlimeDefinition = baseSlime;
        if(customMaxEatCount!=0)
            gordo.GetComponent<GordoEat>().TargetCount = customMaxEatCount;
        
        gordo.hideFlags = HideFlags.DontUnloadUnusedAsset;
        
        var faceComp = gordo.GetComponent<GordoFaceComponents>();
        
        var baseAppearance = baseSlime.GetSlimeAppearance();
        faceComp.BlinkEyes = baseAppearance.Face.GetExpressionFace(SlimeFace.SlimeExpression.BLINK).Eyes;
        faceComp.StrainEyes = baseAppearance.Face.GetExpressionFace(SlimeFace.SlimeExpression.SCARED).Eyes;
        
        if (customEyesBlink != null) faceComp.BlinkEyes = customEyesBlink;
        if (customEyesNormal != null) faceComp.StrainEyes = customEyesNormal;
        if (customMouthHappy != null) faceComp.HappyMouth = customMouthHappy;
        if (customMouthChompOpen != null) faceComp.ChompOpenMouth = customMouthChompOpen;
        if (customMouthEating != null) faceComp.StrainMouth = customMouthEating;
        
        var meshRenderer = gordo.GetObjectRecursively<SkinnedMeshRenderer>("slime_gordo");
        var i = 0;
        meshRenderer.material = Object.Instantiate(baseMaterial);
        meshRenderer.materials = new List<Material>() { 
            meshRenderer.material,
            Object.Instantiate(faceComp.BlinkEyes),
            Object.Instantiate(faceComp.HappyMouth) }.ToArray();

        gordoType.prefab = gordo;

        gameContext.LookupDirector._gordoDict.Add(gordoType, gordo);
        gameContext.LookupDirector._gordoEntries.items.Add(gordo);
        
        
        
        var gordoSlime = new PrismGordo(gordoType, false);
        
        
        _createdGordo = gordoSlime;
        PrismShortcuts._prismGordos.Add(gordoType.ReferenceId,gordoSlime);
        return gordoSlime;
    }
}