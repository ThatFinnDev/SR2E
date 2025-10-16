using System;
using Il2CppInterop.Runtime;
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
    
    
    public static void AddNullAction(this MelonPreferences_Entry entry) => SR2EModMenu.entriesWithActions.Add(entry, null);
    public static void AddAction(this MelonPreferences_Entry entry, System.Action action) => SR2EModMenu.entriesWithActions.Add(entry, action);

    public static Il2CppSystem.Type il2cppTypeof(this Type type) => Il2CppType.From(type);
    
}