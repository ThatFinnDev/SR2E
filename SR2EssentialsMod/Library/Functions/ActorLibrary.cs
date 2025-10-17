using System;
using Il2Cpp;
using Il2CppMonomiPark.SlimeRancher;
using UnityEngine;
using UnityEngine.Localization;

namespace CottonLibrary;

public static partial class Library
{
    public interface IColliderData
    {
        public int Type => -1;
        
        public float GetRadius();
        
        public Mesh GetMesh();
        
        public float GetWidth();
        
        public float GetHeight();
        public float GetDepth();
    }

    public class DefaultColliderData : IColliderData
    {
        public int Type => 1;
        
        public float GetRadius() => 1.5f;
        
        public Mesh GetMesh() => throw new NotImplementedException();

        public float GetWidth() => 1.5f;
        
        public float GetHeight() => 1.5f;
        public float GetDepth() => 1.5f;
    }
    public class SphereColliderData : IColliderData
    {
        public int Type => 1;
        
        public float radius;
        
        public float GetRadius() => radius;
        
        public Mesh GetMesh() => throw new NotImplementedException();

        public float GetWidth() => radius;
        
        public float GetHeight() => radius;
        public float GetDepth() => radius;
    }
    public class CapsuleColliderData : IColliderData
    {
        public int Type => 2;
        
        public float radius;
        public float length;
        
        public float GetRadius() => radius;
        
        public Mesh GetMesh() => throw new NotImplementedException();

        public float GetWidth() => length;
        
        public float GetHeight() => radius;
        public float GetDepth() => radius;
    }
    public class MeshColliderData : IColliderData
    {
        public int Type => 3;
        
        public Mesh mesh;
        
        public float GetRadius() => throw new NotImplementedException();
        
        public Mesh GetMesh() => mesh;

        public float GetWidth() => throw new NotImplementedException();
        
        public float GetHeight() => throw new NotImplementedException();
        public float GetDepth() => throw new NotImplementedException();
    }

    public class CubeColliderData : IColliderData
    {
        public Vector3 size;
        
        public int Type => 0;   
        
        public Mesh GetMesh() => throw new NotImplementedException();

        public float GetRadius() => size.magnitude;
        public float GetWidth() => size.x;
        public float GetHeight() => size.y;
        public float GetDepth() => size.z;
    }
    
    /// <summary>
    /// Creates a game object for a veggie/fruit object. This uses the veggie shader ('SR/AMP/Actor/Resource/Veggie').
    /// </summary>
    /// <param name="ident">The identifiable type attached to the food object.</param>
    /// <param name="mesh">The mesh for the food. Use an AssetBundle to import this.</param>
    /// <param name="texture">The main texture for the food</param>
    /// <param name="masks">Masks texture, R/G/B - Occlusion/Smoothness/Emission</param>
    /// <param name="scale">The size of the food game object. The default for a carrot is 0.12f</param>
    /// <returns>The game object of the food.</returns>
    public static GameObject CreateFoodObject(IdentifiableType ident, Mesh mesh, Texture2D texture, Texture2D masks, ref GameObject baitObject, float scale = 0.12f) => CreateFoodObject(ident, mesh, texture, masks, scale, new DefaultColliderData(), out baitObject);
    public static GameObject CreateFoodObject(IdentifiableType ident, Mesh mesh, Texture2D texture, Texture2D masks, float scale, IColliderData colliderData, out GameObject baitObject)
    {
        var obj = GetVeggie("CarrotVeggie").prefab.CopyObject();
        baitObject = GetVeggie("CarrotVeggie").gordoSnareBaitPrefab.CopyObject();

        obj.name = $"customFood{ident.name}";
        baitObject.name = $"customFood{ident.name}_GordoSnareBait";
        
        obj.RemoveComponent<CapsuleCollider>();

        if (colliderData.Type == 3)
        {
            obj.RemoveComponent<SphereCollider>();
            obj.AddComponent<BoxCollider>().size = new Vector3(colliderData.GetWidth(), colliderData.GetHeight(), colliderData.GetDepth());
        }
        else if (colliderData.Type == 1)
            obj.GetComponent<SphereCollider>().radius = colliderData.GetRadius();
        else if (colliderData.Type == 2)
        {
            obj.RemoveComponent<SphereCollider>();
            var col = obj.AddComponent<CapsuleCollider>();
            col.radius = colliderData.GetRadius();
            col.height = colliderData.GetHeight();
            col.direction = 0;
        }
        else if (colliderData.Type == 3)
        {
            obj.RemoveComponent<SphereCollider>();
            obj.AddComponent<MeshCollider>().sharedMesh = colliderData.GetMesh();
        }
        
        //obj.RemoveComponent<ResourceCycle>();

        var model = obj.transform.FindChild("model_carrot");
        
        model.GetComponent<MeshFilter>().mesh = mesh;
        
        var mat = Object.Instantiate(model.GetComponent<MeshRenderer>().material);
        mat.SetTexture("_Albedo", texture);
        mat.SetTexture("_Masks", masks);
        model.GetComponent<MeshRenderer>().material = mat;
        
        obj.SetObjectIdent(ident);
        
        obj.transform.localScale = Vector3.one * scale;

        ident.gordoSnareBaitPrefab = baitObject;
        
        var baitModel = baitObject.transform.GetChild(0);
        baitModel.GetComponent<MeshFilter>().mesh = mesh;
        baitModel.GetComponent<MeshRenderer>().material = model.GetComponent<MeshRenderer>().material;
        
        baitObject.transform.localScale = Vector3.one * scale;
        
        return obj;
    }
    public static GameObject CreateHenObject(IdentifiableType ident, Texture2D texture, Texture2D masks, IdentifiableType babyIdent, Texture2D babyTexture, Texture2D babyMasks, out GameObject baitObject, out GameObject babyObject)
    {
        var obj = GetMeat("Hen").prefab.CopyObject();
        baitObject = GetMeat("Hen").gordoSnareBaitPrefab.CopyObject();

        obj.name = $"customFood{ident.name}";
        baitObject.name = $"customFood{ident.name}_GordoSnareBait";
        
        var renderer = obj.GetComponentInChildren<SkinnedMeshRenderer>();
        
        
        var mat = Object.Instantiate(renderer.material);
        mat.SetTexture("_MainTex", texture);
        mat.SetTexture("_MaskMap", masks);
        renderer.material = mat;
        foreach (var r in obj.GetComponentsInChildren<MeshRenderer>())
        {
            r.material = mat;
        }
        
        obj.SetObjectIdent(ident);

        ident.gordoSnareBaitPrefab = baitObject;
        
        var baitModel = baitObject.transform.GetChild(0).GetChild(0);
        var baitRenderer = baitModel.GetComponent<SkinnedMeshRenderer>();
        baitRenderer.material = renderer.material;
        
        foreach (var r in baitObject.GetComponentsInChildren<MeshRenderer>())
        {
            r.material = mat;
        }

        babyObject = GetChick("Chick").prefab.CopyObject();

        babyIdent.prefab = babyObject;
        babyIdent.color = ident.color;
        
        babyObject.name = $"customFood{ident.name}Chick";
        
        var renderer2 = babyObject.GetComponentInChildren<SkinnedMeshRenderer>();
        
        
        var mat2 = Object.Instantiate(renderer2.material);
        mat2.SetTexture("_MainTex", babyTexture);
        mat2.SetTexture("_MaskMap", babyMasks);
        renderer2.material = mat2;
        foreach (var r in babyObject.GetComponentsInChildren<MeshRenderer>())
        {
            r.material = mat2;
        }
        
        babyObject.SetObjectIdent(babyIdent);

        var reproduce = obj.GetComponent<Reproduce>();
        reproduce.ChildPrefab = babyObject;
        
        return obj;
    }
    
    public static IdentifiableType GetIdent(this GameObject obj)
    {
        try
        {
            return obj.GetComponent<IdentifiableActor>().identType;
        }
        catch
        {
            return null;
        }
    }

    public static IdentifiableType GetIdentifiableType(this GameObject obj)
    {
        var comp = obj.GetComponent<IdentifiableActor>();

        if (comp != null)
        {
            return comp.identType;
        }

        return null;
    }

    public static GameObject SpawnActor(this GameObject obj, Vector3 pos) =>
        SpawnActor(obj, pos, Quaternion.identity);

    public static GameObject SpawnActor(this GameObject obj, Vector3 pos, Vector3 rot) =>
        SpawnActor(obj, pos, Quaternion.Euler(rot));

    public static GameObject SpawnActor(this GameObject obj, Vector3 pos, Quaternion rot)
    {
        return InstantiationHelpers.InstantiateActor(obj,
            SRSingleton<SceneContext>.Instance.RegionRegistry.CurrentSceneGroup,
            pos,
            rot,
            false,
            SlimeAppearance.AppearanceSaveSet.NONE,
            SlimeAppearance.AppearanceSaveSet.NONE);
    }

    public static GameObject SpawnDynamic(this GameObject obj, Vector3 pos, Quaternion rot)
    {
        return InstantiationHelpers.InstantiateDynamic(obj, pos, rot);
    }

    public static void SetObjectPrefab(this IdentifiableType Object, GameObject prefab)
    {
        Object.prefab = prefab;
    }

    public static void SetObjectIdent(this GameObject prefab, IdentifiableType obj)
    {
        if (obj is SlimeDefinition)
        {
            prefab.GetComponent<SlimeEat>().SlimeDefinition = obj.TryCast<SlimeDefinition>();
            prefab.GetComponent<SlimeAppearanceApplicator>().SlimeDefinition = obj.TryCast<SlimeDefinition>();
        }

        prefab.GetComponent<IdentifiableActor>().identType = obj;
        obj.prefab = prefab;
    }

    public static IdentifiableType CreatePlortType(string Name, Color32 VacColor, Sprite Icon, string RefID,
        float marketValue, float marketSaturation)
    {
        var plort = ScriptableObject.CreateInstance<IdentifiableType>();
        Object.DontDestroyOnLoad(plort);
        plort.hideFlags = HideFlags.HideAndDontSave;
        plort.name = Name + "Plort";
        plort.color = VacColor;
        plort.icon = Icon;
        plort.IsPlort = true;
        if (marketValue > 0)
            MakeSellable(plort, marketValue, marketSaturation);
        plort.AddToGroup("VaccableNonLiquids");
        INTERNAL_SetupLoadForIdent(RefID, plort);
        return plort;
    }
    public static IdentifiableType CreateBlankType(string Name, Color32 VacColor, Sprite Icon, string RefID)
    {
        var type = Object.Instantiate(GetVeggie("CarrotVeggie"));
        Object.DontDestroyOnLoad(type);
        type.hideFlags = HideFlags.HideAndDontSave;
        type.name = Name;
        type.color = VacColor;
        type.icon = Icon;
        type.AddToGroup("VaccableNonLiquids");
        INTERNAL_SetupLoadForIdent(RefID, type);
        return type;
    }

    public static void MakeVaccable(this IdentifiableType ident)
    {
        if (!ident.prefab.GetComponent<Vacuumable>())
            throw new NullReferenceException(
                "This object cannot be made vaccable, it's missing a Vacuumable component, you need to add one.");

        ident.AddToGroup("VaccableNonLiquids");
    }

    public static void SetPlortColor(Color32 Top, Color32 Middle, Color32 Bottom, GameObject Prefab)
    {
        var material = Prefab.GetComponent<MeshRenderer>().material;
        material.SetColor("_TopColor", Top);
        material.SetColor("_MiddleColor", Middle);
        material.SetColor("_BottomColor", Bottom);
    }

    public static void SetPlortTwinColor(Color32 Top, Color32 Middle, Color32 Bottom, GameObject Prefab)
    {
        var material = Prefab.GetComponent<MeshRenderer>().material;
        material.SetColor("_TwinTopColor", Top);
        material.SetColor("_TwinMiddleColor", Middle);
        material.SetColor("_TwinBottomColor", Bottom);
    }

    public static IdentifiableType GetPlort(string name)
    {
        foreach (IdentifiableType type in plorts.GetAllMembersArray())
            if (type.name.ToUpper() == name.ToUpper())
                return type;

        return null;
    }
    
    public static IdentifiableType GetChick(string name)
    {
        foreach (IdentifiableType type in chicks.GetAllMembersArray())
            if (type.name.ToUpper() == name.ToUpper())
                return type;

        return null;
    }
    public static IdentifiableType GetNectar(string name)
    {
        foreach (IdentifiableType type in nectar.GetAllMembersArray())
            if (type.name.ToUpper() == name.ToUpper())
                return type;

        return null;
    }
    
    public static IdentifiableType GetVeggie(string name)
    {
        foreach (IdentifiableType type in veggies.GetAllMembersArray())
            if (type.name.ToUpper() == name.ToUpper())
                return type;

        return null;
    }
    public static IdentifiableType GetFruit(string name)
    {
        foreach (IdentifiableType type in fruits.GetAllMembersArray())
            if (type.name.ToUpper() == name.ToUpper())
                return type;

        return null;
    }
    public static IdentifiableType GetMeat(string name)
    {
        foreach (IdentifiableType type in meat.GetAllMembersArray())
            if (type.name.ToUpper() == name.ToUpper())
                return type;

        return null;
    }
    public static IdentifiableType GetFood(string name)
    {
        foreach (IdentifiableType type in food.GetAllMembersArray())
            if (type.name.ToUpper() == name.ToUpper())
                return type;

        return null;
    }

    public static IdentifiableType GetCraft(string name)
    {
        foreach (IdentifiableType type in crafts.GetAllMembersArray())
            if (type.name.ToUpper() == name.ToUpper())
                return type;

        return null;
    }
    
    public static IdentifiableTypeGroup MakeNewGroup(IdentifiableType[] types, string groupName,
        IdentifiableTypeGroup[] subGroups = null)
    {
        var group = new IdentifiableTypeGroup();
        var typesList = new Il2CppSystem.Collections.Generic.List<IdentifiableType>();
        foreach (var type in types)
        {
            try
            {
                typesList.Add(type);
            }
            catch
            {
            }
        }

        var subGroupsList = new Il2CppSystem.Collections.Generic.List<IdentifiableTypeGroup>();
        foreach (var subGroup in subGroups)
        {
            try
            {
                subGroupsList.Add(subGroup);
            }
            catch
            {
            }
        }

        group._memberTypes = typesList;
        group._memberGroups = subGroupsList;
        
        GameContext.Instance.LookupDirector.RegisterIdentifiableTypeGroup(group);

        return group;
    }

    public static IdentifiableTypeGroup CreateIdentifiableGroup(LocalizedString localizedName, string codeName,
        List<IdentifiableType> types, List<IdentifiableTypeGroup> subGroups, bool isFood = false)
    {
        var group = ScriptableObject.CreateInstance<IdentifiableTypeGroup>();

        group._memberTypes = new Il2CppSystem.Collections.Generic.List<IdentifiableType>();
        foreach (var type in types)
            group._memberTypes.Add(type);

        group._memberGroups = new Il2CppSystem.Collections.Generic.List<IdentifiableTypeGroup>();
        foreach (var subGroup in subGroups)
            group._memberGroups.Add(subGroup);
        
        group._isFood = isFood;

        group._localizedName = localizedName;

        group.name = codeName;
        
        group.AllowedCategories = new Il2CppSystem.Collections.Generic.List<IdentifiableCategory>();
        
        group._runtimeObject = new IdentifiableTypeGroupRuntimeObject(group);

        customGroups.Add(group);
        return group;
    }
}