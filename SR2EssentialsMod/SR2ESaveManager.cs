using Il2CppMonomiPark.SlimeRancher.Player.CharacterController;
using Il2CppMonomiPark.SlimeRancher.Regions;
using Il2CppMonomiPark.SlimeRancher.SceneManagement;
using Il2CppMonomiPark.SlimeRancher.World.Teleportation;
using Newtonsoft.Json;
using UnityEngine.InputSystem;

namespace SR2E;

public enum SR2EError
{
    NoError, NotInGame, PlayerNull, TeleportablePlayerNull, SRCharacterControllerNull, SceneGroupNotSupported, AlreadyExists, DoesntExist
}
public static class SR2ESaveManager
{
    
    internal static SR2ESaveData data = null;
    internal static void Start()
    {
        Load();
    }

    internal static void Update()
    {
        BindingManger.Update();
    }
    internal static void Load()
    {
            if (File.Exists(path)) data = JsonConvert.DeserializeObject<SR2ESaveData>(File.ReadAllText(path));
            else data = new SR2ESaveData();
        
    }
    internal static void Save() { File.WriteAllText(path,JsonConvert.SerializeObject(data, Formatting.Indented)); }
    static string path { get { FileStorageProvider provider = SystemContext.Instance.GetStorageProvider().TryCast<FileStorageProvider>(); if (provider==null) return Application.persistentDataPath + "/SR2E.data"; return provider.savePath + "/SR2E.data"; } }
   
    public class SR2ESaveData
    {
        public Dictionary<string, Warp> warps = new Dictionary<string, Warp>();
        public Dictionary<Key, string> keyBinds = new Dictionary<Key, string>();

    }
    
    public static class WarpManager
    {
        internal static Warp warpTo = null;

        internal static SR2EError AddWarp(string warpName, Warp warp)
        {
            if (data.warps.ContainsKey(warpName)) return SR2EError.AlreadyExists;
            data.warps.Add(warpName, warp);
            Save();
            return SR2EError.NoError;
        }

        internal static Warp GetWarp(string warpName)
        {
            if (!data.warps.ContainsKey(warpName)) return null;
            return data.warps[warpName];
        }

        internal static SR2EError RemoveWarp(string warpName)
        {
            if (!data.warps.ContainsKey(warpName)) return SR2EError.DoesntExist;
            data.warps.Remove(warpName);
            return SR2EError.NoError;
        }
        internal static void OnSceneLoaded()
        {
            if (SceneContext.Instance == null) { warpTo = null; return; }
            if (SceneContext.Instance.PlayerState == null) { warpTo = null; return; }

            if (warpTo != null)
            {
                foreach (SceneGroup group in SystemContext.Instance.SceneLoader.SceneGroupList.items)
                    if (group.IsGameplay) if (group.ReferenceId == warpTo.sceneGroup)
                        if (warpTo.sceneGroup == SceneContext.Instance.Player.GetComponent<RegionMember>().SceneGroup.ReferenceId)
                        {
                            SRCharacterController cc = SceneContext.Instance.Player.GetComponent<SRCharacterController>();
                            cc.Position = warpTo.position;
                            cc.Rotation = warpTo.rotation;
                            cc.Velocity = Vector3.zero;
                            warpTo = null;
                            return;
                        }
            }
        }

    }
    [System.Serializable]
    public class Warp
    {
        internal SR2EError WarpPlayerThere()
        {
            if (!inGame) return SR2EError.NotInGame;
            if (SceneContext.Instance.Player == null) return SR2EError.PlayerNull;
            TeleportablePlayer p = SceneContext.Instance.Player.GetComponent<TeleportablePlayer>();
            if (p == null) return SR2EError.TeleportablePlayerNull;
            SRCharacterController cc = SceneContext.Instance.Player.GetComponent<SRCharacterController>();
            if(cc == null) return SR2EError.SRCharacterControllerNull;
            if (SR2EConsole.isOpen) SR2EConsole.Close();
            if(sceneGroup==p.SceneGroup.ReferenceId)
            {
                cc.Position = position;
                cc.Rotation = rotation;
                cc.Velocity = Vector3.zero;
            }
            else
            {
                foreach (SceneGroup group in SystemContext.Instance.SceneLoader.SceneGroupList.items)
                    if (group.IsGameplay) if (group.ReferenceId == sceneGroup)
                    {
                        try
                        {
                            GameObject prefab = null;
                            switch (sceneGroup)
                            {
                                case "SceneGroup.ConservatoryFields": WarpManager.warpTo = this; prefab = getGadgetDefByName("TeleporterHomeBlue").prefab; break;
                                case "SceneGroup.RumblingGorge": WarpManager.warpTo = this; prefab = getGadgetDefByName("TeleporterZoneGorge").prefab; break;
                                case "SceneGroup.LuminousStrand": WarpManager.warpTo = this; prefab = getGadgetDefByName("TeleporterZoneStrand").prefab; break;
                                case "SceneGroup.PowderfallBluffs": WarpManager.warpTo = this; prefab = getGadgetDefByName("TeleporterZoneBluffs").prefab; break;
                                default: return SR2EError.SceneGroupNotSupported;
                            }

                            if (prefab != null)
                            {
                                GameObject teleporterCollider = prefab.transform.getObjRec<GadgetTeleporterNode>("Teleport Collider").gameObject;
                                GameObject obj = GameObject.Instantiate(teleporterCollider, SceneContext.Instance.Player.transform.position, Quaternion.identity);
                                obj.SetActive(true);
                                obj.GetComponent<StaticTeleporterNode>()._hasDestination = true;
                                obj.GetComponent<StaticTeleporterNode>().UpdateFX();
                            }
                        }catch { }
                    }
            }
            return SR2EError.NoError;
        }
        
        
        public string sceneGroup = "Empty";
        public float x;
        public float y;
        public float z;

        internal Vector3 position
        {
            get { return new Vector3(x, y, z); }
        }

        public float rotX;
        public float rotY;
        public float rotZ;
        public float rotW;

        internal Quaternion rotation
        {
            get { return new Quaternion(rotX, rotY, rotZ, rotW); }
        }

        [JsonConstructor]
        internal Warp(string sceneGroup, Vector3 position, Quaternion rotation)
        {
            this.sceneGroup = sceneGroup;
            x = position.x;
            y = position.y;
            z = position.z;

            rotX = rotation.x;
            rotY = rotation.y;
            rotZ = rotation.z;
            rotW = rotation.w;
        }
    }



    public static class BindingManger
    {
        public static void BindKey(Key key, string command)
        {
            if (data.keyBinds.ContainsKey(key)) data.keyBinds[key] += ";" + command;
            else data.keyBinds.Add(key, command);
            Save();
        }
        public static void UnbindKey(Key key)
        {
            if (data.keyBinds.ContainsKey(key)) data.keyBinds.Remove(key);
            Save();
        }
        public static string GetBind(Key key)
        {
            if (data.keyBinds.ContainsKey(key)) return data.keyBinds[key]; return null;
        }

        public static bool isKeyBound(Key key)
        {
            return data.keyBinds.ContainsKey(key);
        }
        
        internal static void Update()
        {
            try
            {
                foreach (KeyValuePair<Key,string> keyValuePair in data.keyBinds)
                    if (keyValuePair.Key.kc().wasPressedThisFrame)
                        if(WarpManager.warpTo==null)
                            SR2EConsole.ExecuteByString(keyValuePair.Value,true);
            }
            catch 
            {
            }
        }
    }
    
    
}