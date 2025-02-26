using Il2CppMonomiPark.SlimeRancher.Player.CharacterController;
using Il2CppMonomiPark.SlimeRancher.Regions;
using Il2CppMonomiPark.SlimeRancher.SceneManagement;
using Il2CppMonomiPark.SlimeRancher.World.Teleportation;
using Newtonsoft.Json;
using SR2E.Components;
using SR2E.Enums;
using SR2E.Managers;

namespace SR2E.Storage;

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
        if (cc == null) return SR2EError.SRCharacterControllerNull;
        CloseOpenMenu();
        if (sceneGroup == p.SceneGroup.ReferenceId)
        {
            cc.Position = position;
            cc.Rotation = rotation;
            cc.BaseVelocity = Vector3.zero;
        }
        else
        {
            try
            {
                if (!SR2EWarpManager.teleporters.ContainsKey(sceneGroup)) return SR2EError.SceneGroupNotSupported;

                StaticTeleporterNode node = SR2EWarpManager.teleporters[sceneGroup];
                SR2EWarpManager.warpTo = this;

                StaticTeleporterNode obj = GameObject.Instantiate(node, SceneContext.Instance.Player.transform.position,
                    Quaternion.identity);
                obj.gameObject.SetActive(true);
                obj.UpdateFX();
                SceneContext.Instance.Camera.RemoveComponent<NoClipComponent>();
                TryHideMenus();
                TryUnPauseGame();
                //SR2ESavableDataV2.Instance.playerSavedData.noclipState = false;

            }
            catch
            {
            }
        }

        return SR2EError.NoError;
    }


    public string sceneGroup = "Empty";
    public float x;
    public float y;
    public float z;

    internal Vector3 position => new Vector3(x, y, z);

    public float rotX;
    public float rotY;
    public float rotZ;
    public float rotW;

    internal Quaternion rotation => new Quaternion(rotX, rotY, rotZ, rotW);

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