using UnityEngine.InputSystem;

namespace SR2E.Commands;

public class FXPlayCommand : SR2Command
{
    public override string ID => "fxplayer";
    public override string Usage => "fxplayer <FX> [speed] [playandpause]";
    public override string Description => "Plays a effect in front of you.";

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
        if (args == null)
        {
            SendMessage($"Usage: {Usage}");
            return false;
        }

        if (!(args.Length <= 3))
        {
            SendMessage($"Usage: {Usage}");
            return false;
        }

        if (!inGame) SendLoadASaveFirst();

        if (currFX != null && !currFX.isStopped)
        {
            SendError("Please wait for the current FX to stop.");
            return false;
        }

        Camera cam = Camera.main;
        if (cam == null)
        {
            SendError("Couldn't find player camera!");
            return false;
        }

        float playbackSpeed = 0;
        bool playAndPause = false;
        if (args.Length >= 2)
        {
            try
            {
                playbackSpeed = float.Parse(args[1]);
            }
            catch
            {
                SendError(args[1] + " is not a valid float!");
                return false;
            }

            if (playbackSpeed <= 0)
            {
                SendError(args[1] + " is not a float above 0!");
                return false;
            }


            if (args.Length == 3)
            {
                string boolToParse = args[2].ToLower();
                if (boolToParse != "true" && boolToParse != "false")
                {
                    SendError(args[2] + " is not a valid bool!");
                    return false;
                }

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
            catch
            {
                SendError("Invalid FX Name!");
                return false;
            }

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
            SendMessage("Successfully playing FX!");
        }

        SendWarning("You're not looking at anything!");
        return false;

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
        if (Keyboard.current.pKey.wasPressedThisFrame)
        {
            if (sys.isPlaying) sys.Pause();
            else sys.Play();
        }
    }
}