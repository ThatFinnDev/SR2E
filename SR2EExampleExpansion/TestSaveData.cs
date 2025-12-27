using SR2E.Saving;

namespace SR2EExampleExpansion;

// This allows you to store lots of different types of data in a save file
// A bonus point, it won't destroy the save file if you uninstall all mods

// You can use all these primitives, Vector3, Quaternion,
// List, Dictionary, Array and Hashset from both System and Il2CppSystem

// If you want subclasses, they have to inherit SubSave and they are bound to the same types as RootSave

// If one thing fails to load, the entire save data won't load, meaning
// it will act like no save data was present. It even has a CRC32 checksum to check its validity
// So you don't have to worry about things loading halfway
// Fortunately, saving/loading errors are very unlikely!

// Add a [StoryInSave] in front of every variable that should be stored in the save file

// The entire save data is compressed inside the save in order to save precious storage space

public class ExampleSaveData : RootSave 
{
    // Primitives
    [StoreInSave] public bool b = true;
    [StoreInSave] public byte by = 255;
    [StoreInSave] public sbyte sb = -127;
    [StoreInSave] public char c = 'Z';
    [StoreInSave] public decimal dec = 99.99m;
    [StoreInSave] public double d = 1.23456789;
    [StoreInSave] public float f = 3.14f;
    [StoreInSave] public int i = -42;
    [StoreInSave] public uint ui = 42;
    [StoreInSave] public long l = 9000000000L;
    [StoreInSave] public ulong ul = 9000000000UL;
    [StoreInSave] public short s = -32000;
    [StoreInSave] public ushort us = 65000;
    [StoreInSave] public string str = "Hello SR2E!";

    // Unity Types
    [StoreInSave] public Vector3 pos = new Vector3(10, 20, 30);
    [StoreInSave] public Quaternion rot = Quaternion.identity;

    // .NET Collections
    [StoreInSave] public string[] netArray = { "A", "B", "C" };
    [StoreInSave] public List<int> netList = new List<int> { 1, 2, 3 };
    [StoreInSave] public Dictionary<string, float> netDict = new Dictionary<string, float> { { "key", 0.5f } };
    [StoreInSave] public HashSet<int> netSet = new HashSet<int> { 100, 200 };

    // IL2CPP Collections
    [StoreInSave] public Il2CppSystem.Collections.Generic.List<string> ilList;
    [StoreInSave] public Il2CppSystem.Collections.Generic.Dictionary<int, Vector3> ilDict;
    [StoreInSave] public Il2CppInterop.Runtime.InteropTypes.Arrays.Il2CppArrayBase<float> ilArray;
    [StoreInSave] public Il2CppSystem.Collections.Generic.HashSet<int> ilSet;

    // Recursive Object
    [StoreInSave] public SubData sub = new SubData();
}

public class SubData : SubSave 
{
    [StoreInSave] public string subName = "I am a child object";
    public override void OnSave() => MelonLogger.Msg("SubData: Saving...");
    public override void OnLoad() => MelonLogger.Msg("SubData: Loaded!");
}