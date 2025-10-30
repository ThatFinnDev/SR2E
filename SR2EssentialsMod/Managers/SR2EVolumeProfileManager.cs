using System;
using System.IO;
using System.Xml.Serialization;
using UnityEngine.Rendering;

namespace SR2E.Managers;

public static class SR2EVolumeProfileManager
{
    internal static Dictionary<string,VolumeProfile> presets = new Dictionary<string, VolumeProfile>();
    static Volume volumeHolder = null;

    public static byte[] SaveProfile(VolumeProfile profile)
    {
        var data = new VolumeProfileData();

        foreach (var comp in profile.components)
        {
            var compData = new VolumeProfileData.ComponentData
            {
                typeName = comp.GetIl2CppType().AssemblyQualifiedName,
                jsonData = JsonUtility.ToJson(comp)
            };
            data.components.Add(compData);
        }

        var ms = new MemoryStream();
        var serializer = new XmlSerializer(typeof(VolumeProfileData));
        serializer.Serialize(ms, data);
        return ms.ToArray();
    }
    public static bool LoadProfile(string preset, byte[] bytes)
    {
        try
        {

            if (preset == "NORMAL") return false;
            if(presets.ContainsKey(preset)) return false;
            if (bytes == null || bytes.Length == 0) return false;
        
            VolumeProfileData data;
            var ms = new MemoryStream(bytes);
        
            var serializer = new XmlSerializer(typeof(VolumeProfileData));
            data = (VolumeProfileData)serializer.Deserialize(ms);
        
            var newProfile = ScriptableObject.CreateInstance<VolumeProfile>();

            foreach (var compData in data.components)
            {
                var type = Il2CppSystem.Type.GetType(compData.typeName);
                if (type == null) continue;
            

                var comp = ScriptableObject.CreateInstance(type).TryCast<VolumeComponent>();
                comp.hideFlags |= HideFlags.DontUnloadUnusedAsset;
                JsonUtility.FromJsonOverwrite(compData.jsonData, comp);

                newProfile.components.Add(comp);
            }
        
            newProfile.hideFlags |= HideFlags.DontUnloadUnusedAsset;
            presets[preset]=newProfile;
            return true;
        }
        catch (Exception e) { MelonLogger.Error("Error loading preset "+preset+"\n"+e); }
        return false;
    }

    public static bool UnloadProfile(string preset)
    {
        if(!presets.ContainsKey(preset)) return false;
        if (volumeHolder != null)
            if (volumeHolder.profile == presets[preset]) DisableProfile();
        presets.Remove(preset);
        return true;
    }
    public static bool EnableProfile(VolumeProfile profile, bool getVolumeHolderIfNull = true)
    {
        if (profile==null) return false;
        if (volumeHolder==null)
        {
            if (getVolumeHolderIfNull)
            {
                volumeHolder = Get<Volume>("SR2EVolumeHolder");
                return EnableProfile(profile,false);
            }
            return false;
        }

        volumeHolder.priority = 999999;
        volumeHolder.profile = profile;
        volumeHolder.m_InternalProfile = profile;
        volumeHolder.enabled = true;
        return true;
    }
    public static bool EnableProfile(string preset, bool getVolumeHolderIfNull = true)
    {
        if(!presets.ContainsKey(preset)) return false;
        return EnableProfile(presets[preset],getVolumeHolderIfNull);
    }

    public static void DisableProfile(bool getVolumeHolderIfNull = true)
    {
        if (volumeHolder==null)
        {
            if (getVolumeHolderIfNull)
            {
                volumeHolder = Get<Volume>("SR2EVolumeHolder");
                DisableProfile(false);
            }

            return;
        }
        volumeHolder.profile = null;
        volumeHolder.m_InternalProfile = null;
        volumeHolder.enabled = false;
    }
        
    internal static void OnMainMenuUILoad()
    {
        if (volumeHolder != null) return;
        if (volumeHolder == null)
        {
            volumeHolder = Get<Volume>("SR2EVolumeHolder");
            if (volumeHolder != null) return;
        }
        var obj = new GameObject();
        obj.name = "SR2EVolumeHolder";
        obj.AddComponent<Volume>().enabled = false;
        Object.DontDestroyOnLoad(obj);
        volumeHolder= obj.GetComponent<Volume>();
        foreach (var pair in EmbeddedResourceEUtil.LoadResources("Assets.VolumePresets"))
            LoadProfile(pair.Key, pair.Value);
        try
        {
            foreach (var path in Directory.GetFiles(SR2EEntryPoint.CustomVolumeProfilesPath))
            {
                try
                {
                    if (!path.EndsWith(".xml")) return;
                    var name = Path.GetFileNameWithoutExtension(path);
                    LoadProfile(name, File.ReadAllBytes(path));
                }
                catch (Exception e)
                {
                    MelonLogger.Error(e);
                    MelonLogger.Error("Error loading volume profile: "+path);
                }
            }
        } catch (Exception e) { }
        
    }
    
    [Serializable]
    public class VolumeProfileData
    {
        public List<ComponentData> components = new List<ComponentData>();

        [Serializable]
        public class ComponentData
        {
            public string typeName;
            public string jsonData;
        }
    }
}