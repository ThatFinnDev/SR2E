using UnityEngine.InputSystem;

namespace SR2E.Commands;

public class FXPlayCommand : SR2Command
{
    public override string ID => "fxplayer";
    public override string Usage => "fxplayer <FX> [speed] [playandpause]";

    public override List<string> GetAutoComplete(int argIndex, string[] args)
    {
        if (argIndex == 0)
        {
            List<string> list = new List<string>();
            foreach (var p in FXLibrary)
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
           
        

        Camera cam = Camera.main;
        if (cam == null) return SendError(translation("cmd.error.nocamera"));
            

        float playbackSpeed = 0;
        bool playAndPause = false;
        if (args.Length >= 2)
        {
            try { playbackSpeed = float.Parse(args[1]); }
            catch { return SendError(translation("cmd.error.notvalidfloat",args[1])); }

            if (playbackSpeed <= 0) return SendError(translation("cmd.error.notfloatabove",args[1],0));
            


            if (args.Length == 3)
            {
                string boolToParse = args[2].ToLower();
                if (boolToParse != "true" && boolToParse != "false") return SendError(translation("cmd.error.notvalidbool",args[2]));
                
                if (boolToParse == "true") playAndPause = true;
            }
        }


        if (Physics.Raycast(new Ray(cam.transform.position, cam.transform.forward), out var hit))
        {
            GameObject fxobj;
            try
            {
                fxobj = FXLibraryReversable[args[0]].Item2;
            }
            catch { return SendError(translation("cmd.fxplayer.invalidfxname")); }

            fxobj.SpawnFX(cam.transform.position + hit.transform.position);

            if (args.Length >= 2)
            {
                fxobj.GetComponent<ParticleSystem>().playbackSpeed = playbackSpeed;
                if (args.Length == 3)
                {
                    if (playAndPause)
                        fxobj.AddComponent<FXPlayPauseFunction>();
                }
            }

            currFX = fxobj.GetComponent<ParticleSystem>();
            SendMessage(translation("cmd.fxplayer.success"));
        }

        return SendError(translation( "cmd.error.notlookingatanything"));
    }
}

[RegisterTypeInIl2Cpp(false)]
public class FXPlayPauseFunction : MonoBehaviour
{
    public ParticleSystem sys;

    public void Awake()
    {
        sys = GetComponent<ParticleSystem>();
    }

    public void Update()
    {
        if (Key.P.kc().wasPressedThisFrame)
        {
            if (sys.isPlaying) sys.Pause();
            else sys.Play();
        }
    }
}