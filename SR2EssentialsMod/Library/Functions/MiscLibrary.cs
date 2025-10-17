using System.Linq;
using System.Reflection;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppMonomiPark.SlimeRancher.Script.Util;
using MelonLoader;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Tables;

namespace CottonLibrary;

public static partial class Library
{

    public static GameObject CreatePrefab(string Name, GameObject baseObject)
    {
        var obj = baseObject.CopyObject();
        Object.DontDestroyOnLoad(obj);
        
        obj.name = Name;
        obj.transform.parent = rootOBJ.transform;

        var components = obj.GetComponents<Behaviour>();
        foreach (var component in components)
            component.enabled = true;
        
        return obj;
    }
    public static AssetBundle LoadBundle(string embeddedFile)
    {
        Assembly executingAssembly = Assembly.GetCallingAssembly();
        System.IO.Stream manifestResourceStream =
            executingAssembly.GetManifestResourceStream(executingAssembly.GetName().Name + "." + embeddedFile);
        byte[] array = new byte[manifestResourceStream.Length];
        manifestResourceStream.Read(array, 0, array.Length);
        
        return AssetBundle.LoadFromMemory(new Il2CppStructArray<byte>(array));
    }
    
    // Debugging
    
    /// <summary>
    /// Take all base slimes and organize them in a string. You can print this to log.
    /// </summary>
    /// <returns>The list of base slimes.</returns>
    public static string BaseSlimesListAsString()
    {
        string value = "Base slimes:\n";
        foreach (var slime in baseSlimes.GetAllMembersArray())
            value += $"Localized Name: {slime.localizedName.GetLocalizedString()}  |  Actual name: {slime.name}\n";
        return value;
    }

    public static void ExecuteInFrames(System.Action action, int frames)
    {
        MelonCoroutines.Start(Wait(action, frames));
    }

    private static System.Collections.IEnumerator Wait(System.Action action, int frames)
    {
        for (int i = 0; i < frames; i++){
            yield return null;
        }
  
        action();
    } 
}