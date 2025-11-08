/*
 
   
   
   TODO:
   MIGRATE TO PRISM
   IMPROVE PREFAB MECHANIC
   
   
   
   
   
 using System;
using Il2CppMonomiPark.SlimeRancher;
using SR2E.Prism.Lib;
namespace SR2E.Cotton;

public static partial class CottonLibrary
{
    public static class Actors
    {


        /// <summary>
        /// Creates a game object for a veggie/fruit object. This uses the veggie shader ('SR/AMP/Actor/Resource/Veggie').
        /// </summary>
        /// <param name="ident">The identifiable type attached to the food object.</param>
        /// <param name="mesh">The mesh for the food. Use an AssetBundle to import this.</param>
        /// <param name="texture">The main texture for the food</param>
        /// <param name="masks">Masks texture, R/G/B - Occlusion/Smoothness/Emission</param>
        /// <param name="scale">The size of the food game object. The default for a carrot is 0.12f</param>
        /// <returns>The game object of the food.</returns>
        public static GameObject CreateFoodObject(IdentifiableType ident, Mesh mesh, Texture2D texture, Texture2D masks,
            ref GameObject baitObject, float scale = 0.12f) => CreateFoodObject(ident, mesh, texture, masks, scale,
            new DefaultColliderData(), out baitObject);

        public static GameObject CreateFoodObject(IdentifiableType ident, Mesh mesh, Texture2D texture, Texture2D masks,
            float scale, IColliderData colliderData, out GameObject baitObject)
        {
            var obj = LookupEUtil.veggieFoodTypes.GetEntryByName("CarrotVeggie").prefab.CopyObject();
            baitObject = LookupEUtil.veggieFoodTypes.GetEntryByName("CarrotVeggie").gordoSnareBaitPrefab.CopyObject();

            obj.name = $"customFood{ident.name}";
            baitObject.name = $"customFood{ident.name}_GordoSnareBait";

            obj.RemoveComponent<CapsuleCollider>();

            if (colliderData.Type == 3)
            {
                obj.RemoveComponent<SphereCollider>();
                obj.AddComponent<BoxCollider>().size = new Vector3(colliderData.GetWidth(), colliderData.GetHeight(),
                    colliderData.GetDepth());
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
            
            obj.GetComponent<IdentifiableActor>().identType = ident;
            ident.prefab = obj;

            obj.transform.localScale = Vector3.one * scale;

            ident.gordoSnareBaitPrefab = baitObject;

            var baitModel = baitObject.transform.GetChild(0);
            baitModel.GetComponent<MeshFilter>().mesh = mesh;
            baitModel.GetComponent<MeshRenderer>().material = model.GetComponent<MeshRenderer>().material;

            baitObject.transform.localScale = Vector3.one * scale;

            return obj;
        }

        public static GameObject CreateHenObject(IdentifiableType ident, Texture2D texture, Texture2D masks,
            IdentifiableType babyIdent, Texture2D babyTexture, Texture2D babyMasks, out GameObject baitObject,
            out GameObject babyObject)
        {
            var obj = LookupEUtil.meatFoodTypes.GetEntryByName("Hen").prefab.CopyObject();
            baitObject = LookupEUtil.meatFoodTypes.GetEntryByName("Hen").gordoSnareBaitPrefab.CopyObject();

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

            obj.GetComponent<IdentifiableActor>().identType = ident;
            ident.prefab = obj;

            ident.gordoSnareBaitPrefab = baitObject;

            var baitModel = baitObject.transform.GetChild(0).GetChild(0);
            var baitRenderer = baitModel.GetComponent<SkinnedMeshRenderer>();
            baitRenderer.material = renderer.material;

            foreach (var r in baitObject.GetComponentsInChildren<MeshRenderer>())
            {
                r.material = mat;
            }

            babyObject = LookupEUtil.chickFoodTypes.GetEntryByName("Chick").prefab.CopyObject();

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

            obj.GetComponent<IdentifiableActor>().identType = ident;
            ident.prefab = obj;

            var reproduce = obj.GetComponent<Reproduce>();
            reproduce.ChildPrefab = babyObject;

            return obj;
        }
        

        public static void MakeVaccable(IdentifiableType ident)
        {
            if (!ident.prefab.GetComponent<Vacuumable>())
                throw new NullReferenceException(
                    "This object cannot be made vaccable, it's missing a Vacuumable component, you need to add one.");

            ident.Prism_AddToGroup("VaccableNonLiquids");
        }
        


    }
}*/