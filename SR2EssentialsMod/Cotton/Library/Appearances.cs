using Il2CppInterop.Runtime.InteropTypes.Arrays;

namespace SR2E.Cotton;

public static partial class CottonLibrary
{
    public static class Appearances
    {
        public static void AddStructure(SlimeAppearance appearance, SlimeAppearanceStructure structure)
        {
            appearance.Structures=appearance.Structures.AddToNew(structure);
        }
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
    }
}