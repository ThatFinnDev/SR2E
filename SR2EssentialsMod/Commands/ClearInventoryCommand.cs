﻿using Il2Cpp;

namespace SR2E.Commands
{
    public class ClearInventoryCommand : SR2CCommand
    {
        public override string ID => "clearinv";
        public override string Usage => "clearinv";
        public override string Description => "Clears your inventory";
        
        public override bool Execute(string[] args)
        {
            if (args != null)
            {
                SR2Console.SendError($"The '<color=white>{ID}</color>' command takes no arguments");
                return false;
            }

            
            if (SceneContext.Instance == null) { SR2Console.SendError("Load a save first!"); return false; }
            if (SceneContext.Instance.PlayerState == null) { SR2Console.SendError("Load a save first!"); return false; }


            foreach (Ammo.Slot slot in SceneContext.Instance.PlayerState.Ammo.Slots)
                slot.Clear();
            
            
            SR2Console.SendMessage("Successfully cleared your inventory");

            return true;
        }
    }
}