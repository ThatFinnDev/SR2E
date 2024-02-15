using System.Collections;
using UnityEngine;
using MelonLoader;
using Il2Cpp;
using SR2E.Library;
using SR2E.Library.Storage;
using Il2CppMonomiPark.SlimeRancher.Weather.Activity;

namespace VirtualSlime
{
    [RegisterTypeInIl2Cpp]
    public class MergeBehaviour : MonoBehaviour
    {
        public SlimeDefinition mergeWith;
        public SlimeDefinition mergeInto;

        public void Start()
        {
            var ident = gameObject.GetComponent<IdentifiableActor>().identType.prefab.GetComponent<MergeBehaviour>();
            mergeInto = ident.mergeInto;
            mergeWith = ident.mergeWith;
        }

        public void OnCollisionEnter(Collision collision)
        {
            var ident = collision.gameObject.GetIdent();
            if (ident)
            {
                if (ident == mergeWith)
                {
                    var pos = transform.position;
                    var rot = transform.rotation;

                    mergeInto.prefab.SpawnActor(pos, rot);
                    Destroyer.DestroyActor(collision.gameObject, "", true);
                    Destroyer.DestroyActor(gameObject, "", true);
                }
            }
        }
    }
}