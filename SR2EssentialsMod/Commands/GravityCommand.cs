using System;
using System.Text;
using UnityEngine.InputSystem;

namespace SR2E.Commands
{
    public class GravityCommand : SR2CCommand
    {
        public override string ID => "gravity";
        public override string Usage => "gravity <x> <y> <z>";
        public override string Description => "Sets the gravity";

        public override bool Execute(string[] args)
        {
            if (args == null)
            { SR2Console.SendMessage($"Usage: {Usage}"); return false; }

            if (args.Length != 3)
            { SR2Console.SendMessage($"Usage: {Usage}"); return false; }

            Vector3 gravBase;
            try
            {
                gravBase = new Vector3(-float.Parse(args[0]), -float.Parse(args[1]), -float.Parse(args[2]));
                Physics.gravity = gravBase * 9.81f;
                return true;
            }
            catch
            {
                SR2Console.SendError($"The vector {args[0]} {args[1]} {args[2]} is invalid!");
                return false;
            }
            return false;
        }
    }

}
