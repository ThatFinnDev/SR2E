using UnityEngine.InputSystem;

namespace SR2E.Commands
{
    public class FXPlayCommand: SR2CCommand
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
                SR2Console.SendMessage($"Usage: {Usage}");
                return false;
            }

            if (!(args.Length <= 3))
            {
                SR2Console.SendMessage($"Usage: {Usage}");
                return false;
            }

            if (currFX != null && !currFX.isStopped)
            {
                SR2Console.SendError("Please wait for the current FX to stop.");
                return false;
            }
            
            if (Physics.Raycast(new Ray(Camera.main.transform.position, Camera.main.transform.forward), out var hit))
            {
                GameObject fxobj;
                try
                { fxobj = FXLibraryReversable[args[0]].Item2; }
                catch
                { SR2EConsole.SendError("Invalid FX Name!"); return false; }
                fxobj.SpawnFX(Camera.main.transform.position + (Camera.main.transform.forward * 3));
                if (args.Length >= 2)
                {
                    fxobj.GetComponent<ParticleSystem>().playbackSpeed = float.Parse(args[1]);
                    if (args.Length == 3)
                    {
                        if (bool.Parse(args[2]))
                            fxobj.AddComponent<FXPlayPauseFunction>();
                    }
                }
                currFX = fxobj.GetComponent<ParticleSystem>();
            }
            return true;
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
}