using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppInterop.Runtime.InteropTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SR2E.Library
{
    public static class LibraryUtils
    {

        public static IdentifiableType GetIdent(this GameObject obj)
        {
            var comp = obj.GetComponent<IdentifiableActor>();

            if (comp != null)
            {
                return comp.identType;
            }
            else { return null; }
        }

        public static void Add<T>(this Il2CppReferenceArray<T> array, T obj) where T : Il2CppObjectBase
        {
            var s = new T[0];
            array = HarmonyLib.CollectionExtensions.AddToArray(s, obj);
        }
        public static void AddRange<T>(this Il2CppReferenceArray<T> array, Il2CppReferenceArray<T> obj) where T : Il2CppObjectBase
        {
            var s = new T[0];
            array = HarmonyLib.CollectionExtensions.AddRangeToArray(s, obj);
        }
        public static void AddListRange<T>(this Il2CppSystem.Collections.Generic.List<T> list, Il2CppSystem.Collections.Generic.List<T> obj) where T : Il2CppObjectBase
        {
            foreach (var iobj in obj)
            {
                list.Add(iobj);
            }
        }
        public static void AddListRangeNoMultiple<T>(this Il2CppSystem.Collections.Generic.List<T> list, Il2CppSystem.Collections.Generic.List<T> obj) where T : Il2CppObjectBase
        {
            foreach (var iobj in obj)
            {
                if (!list.Contains(iobj))
                {
                    list.Add(iobj);
                }
            }
        }

        public static void AddString(this Il2CppStringArray array, string obj)
        {
            var s = new string[0];
            array = HarmonyLib.CollectionExtensions.AddToArray<string>(s, obj);
        }

        public static void MakePrefab(this GameObject obj)
        {
            UnityEngine.Object.DontDestroyOnLoad(obj);
            obj.transform.parent = SR2EMod.rootOBJ.transform;
        }
    }
}
