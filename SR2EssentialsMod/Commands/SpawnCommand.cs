using Il2Cpp;
using Il2CppMonomiPark.SlimeRancher.Regions;
using UnityEngine;

namespace SR2E.Commands
{
    public class SpawnCommand: SR2CCommand
    {
    
        public override string ID => "spawn";
        public override string Usage => "spawn <object> <amount>";
        public override string Description => "Spawns something in front of your face";
        
        public override bool Execute(string[] args)
        {
            if (args == null)
            {
                SR2Console.SendMessage($"Usage: {Usage}");
                return false;
            }

            if (args.Length != 2)
            {
                SR2Console.SendMessage($"Usage: {Usage}");
                return false;
            }

            
            if (SceneContext.Instance == null) { SR2Console.SendError("Load a save first!"); return false; }
            if (SceneContext.Instance.PlayerState == null) { SR2Console.SendError("Load a save first!"); return false; }


            string itemName = "";
            string identifierTypeName = args[0];
            IdentifiableType type = SR2EMain.getVaccableByName(identifierTypeName);

            if (type == null)
            {
                type = SR2EMain.getVaccableByLocalizedName(identifierTypeName.Replace("_", ""));
                if (type == null)
                { SR2Console.SendError(args[0] + " is not a valid IdentifiableType!"); return false; }
                string name = type.LocalizedName.GetLocalizedString();
                if (name.Contains(" "))
                    itemName = "'" + name + "'";
                else
                    itemName = name;
            }
            else
                itemName=type.name;
            int amount = 0;
            if (!int.TryParse(args[1], out amount))
            { SR2Console.SendError(args[1] + " is not a valid integer!"); return false; }
            
            if (amount<=0)
            { SR2Console.SendError(args[1] + " is not an integer above 0!"); return false; }
                
            

            for (int i = 0; i < amount; i++)
            {
                if (Physics.Raycast(new Ray(Camera.main.transform.position, Camera.main.transform.forward), out var hit))
                {
                    var spawned = SRBehaviour.InstantiateActor(type.prefab, SceneContext.Instance.Player.GetComponent<RegionMember>().SceneGroup, hit.point,Quaternion.identity,true, SlimeAppearance.AppearanceSaveSet.NONE,SlimeAppearance.AppearanceSaveSet.NONE);
                    spawned.transform.position = hit.point+hit.normal*PhysicsUtil.CalcRad(spawned.GetComponent<Collider>());
                    var delta = -(hit.point - Camera.main.transform.position).normalized;
                    spawned.transform.rotation = Quaternion.LookRotation(delta, hit.normal);
                }
            }
            
            
            SR2Console.SendMessage($"Successfully spawned {amount} {itemName}");
            
            return true;
        }
    }

    
}