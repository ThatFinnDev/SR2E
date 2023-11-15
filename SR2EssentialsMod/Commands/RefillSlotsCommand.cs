using Il2Cpp;

namespace SR2E.Commands
{
    public class RefillSlotsCommand : SR2CCommand
    {
        public override string ID { get; } = "refillslots";
        public override string Usage { get; } = "refillslots";
        public override string Description { get; } = "Refills all of your slots";
        
        public override bool Execute(string[] args)
        {
            if (args != null)
            {
                SR2Console.SendError($"The '<color=white>{ID}</color>' command takes no arguments");
                return false;
            }
            if (SceneContext.Instance == null)
            { SR2Console.SendError("Load a save first!"); return false; }
                
            if (SceneContext.Instance.PlayerState == null) 
            { SR2Console.SendError("Load a save first!"); return false; }
            
            foreach (Ammo.Slot slot in SceneContext.Instance.PlayerState.Ammo.Slots)
            {
                if (slot.IsUnlocked)
                {
                    if (slot.Id != null)
                    {
                        slot.Count = slot.MaxCount;
                    }
                }
            }
            SR2Console.SendMessage("Successfully refilled your slots");

            return true;
        }
    }}