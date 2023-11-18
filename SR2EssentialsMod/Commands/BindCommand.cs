using System.Text;
using UnityEngine.InputSystem;

namespace SR2E.Commands
{
    public class BindCommand : SR2CCommand
    {
        public override string ID => "bind";
        public override string Usage => "bind <key> <command>";
        public override string Description => "Binds a key to a specific command";

        public override bool Execute(string[] args)
        {
            if (args == null)
            { SR2Console.SendMessage($"Usage: {Usage}"); return false; }

            if (args.Length < 1)
            { SR2Console.SendMessage($"Usage: {Usage}"); return false; }

            int e;
            string keyToParse = args[0];
            
            if(args[0].ToCharArray().Length==1)
                if (int.TryParse(args[0], out e))
                    keyToParse = "Digit"+args[0];
            
            Key key;
            if (Key.TryParse(keyToParse,true,out key))
            {
                StringBuilder builder = new StringBuilder();
                for (int i = 1; i < args.Length; i++)
                { builder.Append(args[i]+" "); }
                string executeString = builder.ToString();
                
                if (SR2CommandBindingManager.keyCodeCommands.ContainsKey(key))
                    SR2CommandBindingManager.keyCodeCommands[key] += ";" + executeString;
                else
                    SR2CommandBindingManager.keyCodeCommands.Add(key, executeString);
                
                SR2CommandBindingManager.SaveKeyBinds();
                SR2Console.SendMessage($"Successfully bound command '{executeString}' to key {key}");
                return true;
            }
            
            SR2Console.SendMessage($"{args[0]} is not a valid KeyCode!");
            return false;
        }
    }

}