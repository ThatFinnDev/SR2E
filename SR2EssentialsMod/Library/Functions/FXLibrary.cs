using Il2Cpp;
using UnityEngine;

namespace CottonLibrary;

public static partial class Library
{
    public static Dictionary<string, GameObject> allFXObjects = new Dictionary<string, GameObject>();
    internal static void InitializeFX()
    {
        foreach (var particle in Resources.FindObjectsOfTypeAll<ParticleSystem>())
        {
            allFXObjects.TryAdd(particle.gameObject.name, particle.gameObject);
        }
    }

    public static GameObject GetFXObject(string name) => allFXObjects[name];

    public static GameObject PlayFX(string name, Vector3 position, Quaternion rotation) => FXHelpers.SpawnAndPlayFX(GetFXObject(name), position, rotation);
}