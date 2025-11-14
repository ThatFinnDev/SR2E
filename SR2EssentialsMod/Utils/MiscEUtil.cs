using System;
using System.Linq;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.InteropTypes;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppSystem.Linq;
using SR2E.Menus;
using Unity.Mathematics;

namespace SR2E.Utils;

public static class MiscEUtil
{
    public static float4 changeValue(this float4 float4, int index, float value)
    {
        return new float4(index == 0 ? value : float4[0],
            index == 1 ? value : float4[1],
            index == 2 ? value : float4[2],
            index == 3 ? value : float4[3]
        );
    }
    public static LayerMask defaultMask
    {
        get
        {
            LayerMask mask = ~0;
            mask &= ~(1 << Layers.GadgetPlacement);
            return mask;
        }
    }
    public static readonly Dictionary<Branch, string> BRANCHES = new()
    {
        { Branch.Release, "release" },
        { Branch.Beta, "beta" },
        { Branch.Alpha, "alpha" },
        { Branch.Developer, "dev" },
    };
    
    private const string AllowedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
    private static readonly int AllowedCharCount = AllowedChars.Length;
    internal static string GetRandomString(int length)
    {
        Span<char> chars = stackalloc char[length];
        var random = System.Random.Shared;
        for (int i = 0; i < length; i++)
            chars[i] = AllowedChars[random.Next(AllowedCharCount)];
        return new string(chars);
    }
    
    public static Camera GetActiveCamera()
    {
        Camera[] cams = Camera.allCameras;
        Camera active = null;

        foreach (var c in cams)
        {
            if (active == null || c.depth > active.depth)
                active = c;
        }

        return active;
    }

    internal static SlimeAppearance.AppearanceSaveSet GetAppearanceSet(this IdentifiableType type)
    {
        if (type.TryCast<SlimeDefinition>() != null) return SlimeAppearance.AppearanceSaveSet.CLASSIC;
        return SlimeAppearance.AppearanceSaveSet.NONE;
;    }
    public static void AddNullAction(this MelonPreferences_Entry entry) => SR2EModMenu.entriesWithActions.Add(entry, null);
    public static void AddAction(this MelonPreferences_Entry entry, System.Action action) => SR2EModMenu.entriesWithActions.Add(entry, action);

    public static Il2CppSystem.Type il2cppTypeof(this Type type) => Il2CppType.From(type);
    
    
    public static Il2CppArrayBase GetAllMembersArray(this IdentifiableTypeGroup group) => Il2CppSystem.Linq.Enumerable.ToArray(group.GetAllMembers());
    public static List<IdentifiableType> GetAllMembersList(this IdentifiableTypeGroup group) => group.GetAllMembers().ToArray().ToList();



    /// <summary>
    /// Use this for copying components, please make sure you are copying the same types!
    /// </summary>
    public static void CopyFields(this Object target, Object source)
    {
        foreach (var field in source.GetIl2CppType().GetFields(Il2CppSystem.Reflection.BindingFlags.Instance | Il2CppSystem.Reflection.BindingFlags.Public | Il2CppSystem.Reflection.BindingFlags.NonPublic))
        {
            if (field.IsLiteral && !field.IsInitOnly)
                continue;
            try
            {
                target.GetIl2CppType().GetField(field.Name, (Il2CppSystem.Reflection.BindingFlags)60).SetValue(target, field.GetValue(source));
            }
            // Errors when encountering `const` or `readonly`!
            catch { }
        }
    }
    
    public static Il2CppReferenceArray<T> RemoveToNew<T>(this Il2CppReferenceArray<T> array, T obj) where T : Il2CppObjectBase
    {
        var list = new Il2CppSystem.Collections.Generic.List<T>();
        foreach (var item in array)
        {
            list.Add(item);
        }

        if(list.Contains(obj))
            list.Remove(obj);

        array = list.ToArray().Cast<Il2CppReferenceArray<T>>();
        return array;
    }
    public static Il2CppReferenceArray<T> AddToNew<T>(this Il2CppReferenceArray<T> array, T obj) where T : Il2CppObjectBase
    {
        var list = new Il2CppSystem.Collections.Generic.List<T>();
        foreach (var item in array)
        {
            list.Add(item);
        }

        list.Add(obj);

        array = list.ToArray().Cast<Il2CppReferenceArray<T>>();
        return array;
    }
    public static Il2CppReferenceArray<T> ReplaceToNew<T>(this Il2CppReferenceArray<T> array, T obj, int index) where T : Il2CppObjectBase
    {
        var list = new Il2CppSystem.Collections.Generic.List<T>();
        foreach (var item in array)
        {
            list.Add(item);
        }

        list.RemoveAt(index);
        list.Insert(index,obj);

        array = list.ToArray().Cast<Il2CppReferenceArray<T>>();
        return array;
    }
    public static Il2CppReferenceArray<T> RemoveAtToNew<T>(this Il2CppReferenceArray<T> array, int index) where T : Il2CppObjectBase
    {
        var list = new Il2CppSystem.Collections.Generic.List<T>();
        foreach (var item in array)
        {
            list.Add(item);
        }

        list.RemoveAt(index);

        array = list.ToArray().Cast<Il2CppReferenceArray<T>>();
        return array;
    }
    public static Il2CppReferenceArray<T> InsertToNew<T>(this Il2CppReferenceArray<T> array, T obj, int index) where T : Il2CppObjectBase
    {
        var list = new Il2CppSystem.Collections.Generic.List<T>();
        foreach (var item in array)
        {
            list.Add(item);
        }

        list.Insert(index,obj);

        array = list.ToArray().Cast<Il2CppReferenceArray<T>>();
        return array;
    }
    public static Il2CppReferenceArray<T> AddRangeToNew<T>(this Il2CppReferenceArray<T> array, Il2CppReferenceArray<T> obj) where T : Il2CppObjectBase
    {
        var list = new Il2CppSystem.Collections.Generic.List<T>();
        foreach (var item in array) list.Add(item);

        foreach (var item in obj) list.Add(item);
        
        array = list.ToArray().Cast<Il2CppReferenceArray<T>>();
        return array;
    }

    public static Il2CppReferenceArray<T> AddRangeNoMultipleToNew<T>(this Il2CppReferenceArray<T> array, Il2CppReferenceArray<T> obj) where T : Il2CppObjectBase
    {
        var list = new Il2CppSystem.Collections.Generic.List<T>();
        foreach (var item in array)
            if (!list.Contains(item))
                list.Add(item);
        

        foreach (var item in obj)
            if (!list.Contains(item))
                list.Add(item);
        

        array = list.ToArray().Cast<Il2CppReferenceArray<T>>();
        return array;
    }

    public static Il2CppSystem.Collections.Generic.List<T> AddRangeToNew<T>(this Il2CppSystem.Collections.Generic.List<T> list, Il2CppSystem.Collections.Generic.List<T> obj) where T : Il2CppObjectBase
    {
        foreach (var iobj in obj)
            list.Add(iobj);
        return list;
    }

    public static Il2CppSystem.Collections.Generic.List<T>  AddRangeNoMultipleToNew<T>(this Il2CppSystem.Collections.Generic.List<T> list, Il2CppSystem.Collections.Generic.List<T> obj) where T : Il2CppObjectBase
    {
        foreach (var iobj in obj)
            if (!list.Contains(iobj))
                list.Add(iobj);
        return list;
    }

    public static Il2CppStringArray AddToNew(this Il2CppStringArray array, string str)
    {
        string[] strArray = new string[array.Count + 1];

        int i = 0;
        foreach (var item in array)
        {
            strArray[i] = item;
            i++;
        }

        strArray[i] = str;

        return new Il2CppStringArray(strArray);
    }

    
    public static List<T> ToList<T>(this HashSet<T> hashSet)
    {
        if (hashSet == null) return null;
        var list = new List<T>(hashSet.Count);
        foreach (T item in hashSet) list.Add(item);
        return list;
    }
    
    public static List<T> ToList<T>(this Il2CppSystem.Collections.Generic.HashSet<T> hashSet)
    {
        if (hashSet == null) return null;
        var list = new List<T>(hashSet.Count);
        foreach (T item in hashSet) list.Add(item);
        return list;
    }
    public static HashSet<T> ToHashSet<T>(this List<T> list)
    {
        if (list == null) return null;
        var hashSet = new HashSet<T>();
        foreach (T item in list) hashSet.Add(item);
        return hashSet;
    }
    public static Il2CppSystem.Collections.Generic.HashSet<T> ToIl2CppHashSet<T>(this List<T> list)
    {
        if (list == null) return null;
        var hashSet = new Il2CppSystem.Collections.Generic.HashSet<T>();
        foreach (T item in list) hashSet.Add(item);
        return hashSet;
    }
    
    public static bool IsInsideRange(this int number, int rangeMin, int rangeMax) => number >= rangeMin && number <= rangeMax;

    public static bool ContainsAny(this string str, params string[] check) => check.Any(str.Contains);

    public static Il2CppReferenceArray<T> ToCppArray<T>(this IEnumerable<T> collection) where T : Il2CppObjectBase => new(collection.ToArray());
}