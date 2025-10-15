using System;
using Il2CppMonomiPark.SlimeRancher.Player.CharacterController;
using Il2CppMonomiPark.SlimeRancher.SceneManagement;
using Il2CppMonomiPark.SlimeRancher.World.Teleportation;
using Newtonsoft.Json;
using SR2E.Components;
using SR2E.Enums;
using SR2E.Managers;
using SR2E.Patches.General;

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
        MenuUtil.CloseOpenMenu();
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
                var sg = SystemContext.Instance.SceneLoader.SceneGroupList.GetSceneGroupFromReferenceId(sceneGroup);
                if (sg == null) return SR2EError.SceneGroupNotSupported;
                if(!sg.IsGameplay) return SR2EError.SceneGroupNotSupported;
                SceneContext.Instance.Camera.RemoveComponent<NoClipComponent>();;
                TryHideMenus();
                TryUnPauseGame();
                SR2EWarpManager.warpTo = this;
                ExecuteInTicks((Action)(() =>
                {
                    try
                    {
                        
                        var e = SceneLoaderLoadSceneGroupPatch._loadingParameters;
                        //When you just set the position to your target position, the zones don't load properly
                        //By setting the player's pos really far away and then to the correct one after the loading screen, it works fine
                        e.TeleportPosition = new Il2CppSystem.Nullable<Vector3>(new Vector3(0,9999999,0));
                        SystemContext.Instance.SceneLoader.LoadSceneGroup(sg, e);
                    }
                    catch (Exception e)
                    {
                        MelonLogger.Error(e); 
                    }
                    //Anything lower than 5 ticks breaks it. I guess smth with Timescaling but idk
                }), 5);

            }
            catch (Exception e)
            {
                MelonLogger.Error(e); 
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