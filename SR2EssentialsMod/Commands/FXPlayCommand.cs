﻿using SR2E.Enums;
using SR2E.Managers;
using UnityEngine.InputSystem;

namespace SR2E.Commands;

internal class FXPlayCommand : SR2ECommand
{
    public override string ID => "fxplayer";
    public override string Usage => "fxplayer <FX> [speed] [playandpause]";
    public override CommandType type => CommandType.Fun;

    public override List<string> GetAutoComplete(int argIndex, string[] args)
    {
        if (argIndex == 0)
        {
            List<string> list = new List<string>();
            foreach (var p in LookupEUtil.FXLibrary)
                list.Add(p.Value.Item2);
            return list;
        }

        if (argIndex == 1)
            return new List<string>() { "0.25", "0.5", "0.75", "1", "1.25", "1.5", "2", };
        if (argIndex == 2)
            return new List<string>() { "true", "false", };
        return null;
    }

    public ParticleSystem currFX;

    public override bool Execute(string[] args)
    {
        if (!args.IsBetween(1,3)) return SendUsage();
        if (!inGame) SendLoadASaveFirst();

        if (currFX != null && !currFX.isStopped) return SendError(translation("cmd.fxplayer.waitforstop"));
        
        Camera cam = Camera.main; if (cam == null) return SendNoCamera();
        
        float playbackSpeed = 0;
        bool playAndPause = false;
        if (args.Length >= 2)
        {
            if (!TryParseFloat(args[1], out playbackSpeed, 0, false)) return false;
            if (args.Length == 3) if (!TryParseBool(args[2], out playAndPause)) return false;
        }


        if (Physics.Raycast(new Ray(cam.transform.position, cam.transform.forward), out var hit,Mathf.Infinity,MiscEUtil.defaultMask))
        {
            GameObject fxobj;
            try { fxobj = LookupEUtil.FXLibraryReversable[args[0]].Item2; }
            catch { return SendError(translation("cmd.fxplayer.invalidfxname")); }

            fxobj.SpawnFX(cam.transform.position + hit.transform.position);

            if (args.Length >= 2)
            {
                fxobj.GetComponent<ParticleSystem>().playbackSpeed = playbackSpeed;
                if (args.Length == 3) if (playAndPause) fxobj.AddComponent<FXPlayPauseFunction>();
            }

            currFX = fxobj.GetComponent<ParticleSystem>();
            SendMessage(translation("cmd.fxplayer.success"));
            return true;
        }
        return SendNotLookingAtAnything();
    }
}

[RegisterTypeInIl2Cpp(false)]
internal class FXPlayPauseFunction : MonoBehaviour
{
    public ParticleSystem sys;

    public void Awake()
    {
        sys = GetComponent<ParticleSystem>();
    }

    public void Update()
    {
        if (LKey.P.OnKeyDown())
        {
            if (sys.isPlaying) sys.Pause();
            else sys.Play();
        }
    }
}