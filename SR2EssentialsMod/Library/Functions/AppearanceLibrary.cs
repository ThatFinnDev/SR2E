using Il2Cpp;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using UnityEngine;

namespace CottonLibrary;

public static partial class Library
{
    public static SlimeAppearanceStructure AddStructure(this SlimeAppearance app, Mesh mesh,
        SlimeAppearance.SlimeBone rootBone, SlimeAppearance.SlimeBone parentBone, string elementName)
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
        
        app._structures = app._structures.Add(structure);
        return structure;
    }
}