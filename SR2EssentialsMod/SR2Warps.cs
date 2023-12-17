using Il2CppMonomiPark.SlimeRancher.Player.CharacterController;
using Il2CppMonomiPark.SlimeRancher.Regions;
using Il2CppMonomiPark.SlimeRancher.SceneManagement;
using Il2CppSystem;

namespace SR2E
{
    public static class SR2Warps
    {
        internal static Dictionary<string, Warp> warps = new Dictionary<string, Warp>();
        internal static Warp warpTo = null;
        private static string path = SystemContext.Instance.GetStorageProvider().Cast<FileStorageProvider>().savePath + "SR2EssentialsWarps.warps";

        internal static void OnSceneLoaded(string sceneName)
        {
            if (SceneContext.Instance == null) { warpTo = null; return; }
            if (SceneContext.Instance.PlayerState == null) { warpTo = null; return; }

            if (warpTo != null)
            {
                foreach (SceneGroup group in SystemContext.Instance.SceneLoader.SceneGroupList.items)
                    if (group.IsGameplay)
                        if (group.ReferenceId == warpTo.sceneGroup)
                            if (warpTo.sceneGroup == SceneContext.Instance.Player.GetComponent<RegionMember>()
                                    .SceneGroup.ReferenceId)
                            {
                                SRCharacterController cc = SceneContext.Instance.Player
                                    .GetComponent<SRCharacterController>();
                                cc.Position = warpTo.position;
                                cc.Rotation = warpTo.rotation;
                                cc.Velocity = Vector3.zero;
                                warpTo = null;
                                return;
                            }



            }
        }

        internal static void Start()
        {
            if (File.Exists(path))
            {
                LoadWarps();
            }
        }
        //Make saving system for warps
        internal static void SaveWarps()
        {
            string safe = "";
            foreach (KeyValuePair<string, Warp> keyValuePair in warps)
            {
                safe += keyValuePair.Key + "\n";
                safe += keyValuePair.Value.sceneGroup + "|" +
                        keyValuePair.Value.x + "|" + keyValuePair.Value.y + "|" + keyValuePair.Value.z + "|" +
                        keyValuePair.Value.rotX + "|" + keyValuePair.Value.rotY + "|" + keyValuePair.Value.rotZ + "|" +
                        keyValuePair.Value.rotW
                        + "\n";
            }

            File.WriteAllText(path, safe);
        }
        internal static void LoadWarps()
        {
            string name = "";
            foreach (string line in File.ReadAllLines(path))
            {
                if(String.IsNullOrEmpty(line))
                    continue;
                if (name=="")
                {
                    name = line;
                }
                else
                {
                    string[] split = line.Split('|');
                    string sceneGroup = split[0];
                    float x = float.Parse(split[1]);
                    float y = float.Parse(split[2]);
                    float z = float.Parse(split[3]);
                    float xRot = float.Parse(split[4]);
                    float yRot = float.Parse(split[5]);
                    float zRot = float.Parse(split[6]);
                    float wRot = float.Parse(split[7]);
                    Warp warp = new Warp(sceneGroup, new Vector3(x, y, z), new Quaternion(xRot,yRot,zRot,wRot));
                    warps.Add(name,warp);
                    name = "";
                }
            }
        }
    }

    internal class Warp
    {
        internal string sceneGroup;
        internal float x;
        internal float y;
        internal float z;

        internal Vector3 position
        {
            get { return new Vector3(x, y, z); }
        }

        internal float rotX;
        internal float rotY;
        internal float rotZ;
        internal float rotW;

        internal Quaternion rotation
        {
            get { return new Quaternion(rotX, rotY, rotZ, rotW); }
        }

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
}