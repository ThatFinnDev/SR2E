using System;
using System.Globalization;
using Il2CppMonomiPark.SlimeRancher.DebugTool;
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
    public bool IsValid()
    {
        foreach (var g in systemContext.SceneLoader.SceneGroupList.items)
            if (g.ReferenceId == sceneGroup)
                return g.IsGameplay;
        return false;
    }

    bool IsInCorrectSceneGroup(string refIDCurrent, string refIDNext)
    {
        if (refIDCurrent == refIDNext) return true;
        if (refIDCurrent == "SceneGroup.AllZones")
        {
            if (refIDNext == "SceneGroup.ConservatoryFields") return true;
            if (refIDNext == "SceneGroup.PowderfallBluffs") return true;
            if (refIDNext == "SceneGroup.RumblingGorge") return true;
        }
        return false;
    }
    public SR2EError WarpPlayerThere()
    {
        if (!inGame) return SR2EError.NotInGame;
        if(!IsValid()) return SR2EError.Invalid;
        if (sceneContext.Player == null) return SR2EError.PlayerNull;
        TeleportablePlayer p = sceneContext.Player.GetComponent<TeleportablePlayer>();
        if (p == null) return SR2EError.TeleportablePlayerNull;
        SRCharacterController cc = sceneContext.Player.GetComponent<SRCharacterController>();
        if (cc == null) return SR2EError.SRCharacterControllerNull;
        MenuEUtil.CloseOpenMenu();
        if (IsInCorrectSceneGroup(p.SceneGroup.ReferenceId,sceneGroup))
        {
            cc.Position = position;
            cc.Rotation = rotation;
            cc.BaseVelocity = Vector3.zero;
            cc.ResetVelocity(false);
        }
        else
        {
            try
            {
                SceneGroup sc = null;
                foreach (var g in systemContext.SceneLoader.SceneGroupList.items)
                    if (g.ReferenceId == sceneGroup)
                    {
                        if(!g.IsGameplay) return SR2EError.SceneGroupNotSupported;
                        sc = g;
                        break;
                    }
                if(sc==null) return SR2EError.SceneGroupNotSupported;
                SR2EWarpManager.warpTo = this;
                SceneLoaderLoadSceneGroupPatch.isTeleportingPlayer = true;
                LocationBookmarksUtil.GoToLocationPlayer(sc,position+new Vector3(0,LocationBookmarksUtil.PLAYER_HEIGHT/2,0),rotation.eulerAngles);

            }
            catch (Exception e)
            {
                MelonLogger.Error(e); 
            }
        }

        return SR2EError.NoError;
    }

    public string sceneGroup = "None";
    public float x;
    public float y;
    public float z;

    internal Vector3 position => new Vector3(x, y, z);

    public float rotX;
    public float rotY;
    public float rotZ;
    public float rotW;

    internal Quaternion rotation => new Quaternion(rotX, rotY, rotZ, rotW);

    internal LocationBookmarksUtil.LocationBookmark ToNative()
    {
        var mark = new LocationBookmarksUtil.LocationBookmark();
        mark.position = position;
        mark.rotationEuler = rotation.eulerAngles;
        mark.sceneGroupName = sceneGroup;
        if (mark.sceneGroupName.StartsWith("SceneGroup."))
            mark.sceneGroupName = sceneGroup.Substring("SceneGroup.".Length);
        return mark;
    }
    internal static Warp FromNative(LocationBookmarksUtil.LocationBookmark mark)
    {
        if (mark == null) return null;
        return new Warp("SceneGroup." + mark.sceneGroupName, mark.position, mark.rotationEuler);
        
    }

    public static Warp CurrentLocation()
    {
        try
        {
            return FromNative(LocationBookmarksUtil.GetNewPlayerLocationBookmark());
        }
        catch 
        { }
        return new Warp();
    }
    public static Warp FromString(string stringed)
    {
        if (!LocationBookmarksUtil.ValidLocationString(stringed)) return null;
        string[] parts = stringed.Split('|');

        string[] posParts = parts[1].Split(',');
        Vector3 position = new Vector3(
            float.Parse(posParts[0], CultureInfo.InvariantCulture),
            float.Parse(posParts[1], CultureInfo.InvariantCulture),
            float.Parse(posParts[2], CultureInfo.InvariantCulture)
        );

        string[] rotParts = parts[2].Split(',');
        Vector3 rotation = new Vector3(
            float.Parse(rotParts[0], CultureInfo.InvariantCulture),
            float.Parse(rotParts[1], CultureInfo.InvariantCulture),
            float.Parse(rotParts[2], CultureInfo.InvariantCulture)
        );
        return new Warp(parts[0], position, rotation);
    }

    public override string ToString() => LocationBookmarksUtil.GetLocationStringFromBookmark(ToNative());
    
    private Warp() {}
    [JsonConstructor]
    public Warp(string sceneGroup, Vector3 position, Quaternion rotation)
    {
        this.sceneGroup = sceneGroup;
        x = position.x; y = position.y; z = position.z;

        rotX = rotation.x; rotY = rotation.y; rotZ = rotation.z; rotW = rotation.w;
    }
    public Warp(string sceneGroup, Vector3 position, Vector3 rotationEuler)
    {
        this.sceneGroup = sceneGroup;
        x = position.x; y = position.y; z = position.z;

        var rotation = Quaternion.Euler(rotationEuler);
        rotX = rotation.x; rotY = rotation.y; rotZ = rotation.z; rotW = rotation.w;
    }
    public Warp(SceneGroup sceneGroup, Vector3 position, Quaternion rotation)
    {
        this.sceneGroup = sceneGroup.ReferenceId;
        x = position.x;
        y = position.y;
        z = position.z;

        rotX = rotation.x; rotY = rotation.y; rotZ = rotation.z; rotW = rotation.w;
    }
    public Warp(SceneGroup sceneGroup, Vector3 position, Vector3 rotationEuler)
    {
        this.sceneGroup = sceneGroup.ReferenceId;
        x = position.x; y = position.y; z = position.z;

        var rotation = Quaternion.Euler(rotationEuler);
        rotX = rotation.x; rotY = rotation.y; rotZ = rotation.z; rotW = rotation.w;
    }
}