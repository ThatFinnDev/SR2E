using System;
using System.Collections.Generic;
using UnityEngine.InputSystem;

namespace SR2E.Commands
{
    public class UnbindCommand : SR2CCommand
    {
        public override string ID => "unbind";
        public override string Usage => "unbind <key>";
        public override string Description => "Unbinds a key that was previously bound to a command";
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
            return null;
        }
        public override bool Execute(string[] args)
        {
            if (args == null)
            { SR2Console.SendMessage($"Usage: {Usage}"); return false; }

            if (args.Length != 1)
            { SR2Console.SendMessage($"Usage: {Usage}"); return false; }

            int e;
            string keyToParse = args[0];
            
            if(args[0].ToCharArray().Length==1)
                if (int.TryParse(args[0], out e))
                    keyToParse = "Digit"+args[0];
            
            Key key;
            if (Key.TryParse(keyToParse,true,out key))
            {
                if (!SR2CommandBindingManager.keyCodeCommands.ContainsKey(key))
                {
                    SR2Console.SendMessage($"{args[0]} is not bound to anything!");
                    return false;
                }
                SR2CommandBindingManager.keyCodeCommands.Remove(key);
                SR2CommandBindingManager.SaveKeyBinds();
                SR2Console.SendMessage($"Successfully unbound key {key}");
                return true;
            }
            
            SR2Console.SendMessage($"{args[0]} is not a valid KeyCode!");
            return false;
        }
    }
}