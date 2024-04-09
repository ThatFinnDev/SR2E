using System;
using System.Text;
using UnityEngine.InputSystem;

namespace SR2E.Commands
{
    public class BindCommand : SR2CCommand
    {
        public override string ID => "bind";
        public override string Usage => "bind <key> <command>";
        public override string Description => "Binds a key to a specific command";

        public override List<string> GetAutoComplete(int argIndex, string[] args)
        {
            if (argIndex == 0)
            {
                string firstArg = "";
                if (args != null)
                    firstArg = args[0];
                List<string> list = new List<string>();
                foreach (string key in System.Enum.GetNames(typeof(Key)))
                    if (!String.IsNullOrEmpty(key))
                        if(key!="None")
                            if (key.ToLower().Replace(" ", "").StartsWith(firstArg.ToLower())) 
                                list.Add(key.Replace(" ", ""));
                
                return list;
            }
            if (argIndex == 1)
            {
                List<string> list = new List<string>();
                foreach (KeyValuePair<string, SR2CCommand> entry in SR2EConsole.commands) list.Add(entry.Key);
                return list;
            }
            string secondArg = args[1];
            foreach (KeyValuePair<string, SR2CCommand> entry in SR2EConsole.commands)
            {
                if (entry.Key == secondArg) return entry.Value.GetAutoComplete(argIndex-2,args);
            }
            return null;
        }
        public override bool Execute(string[] args)
        {
            if (args == null || args.Length < 1) return SendUsage();

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
                
                SR2ESaveManager.BindingManger.BindKey(key,executeString);
                SR2EConsole.SendMessage($"Successfully bound command '{executeString}' to key {key}");
                return true;
            }
            
            SR2EConsole.SendMessage($"{args[0]} is not a valid KeyCode!");
            return false;
        }
    }

}