using System;
using System.Collections.Generic;
using Il2Cpp;
using Il2CppMonomiPark.SlimeRancher.Regions;
using Il2CppMonomiPark.SlimeRancher.UI.Gadget;
using UnityEngine;

namespace SR2E.Commands
{
    public class SpawnCommand: SR2CCommand
    {
    
        public override string ID => "spawn";
        public override string Usage => "spawn <object> <amount>";
        public override string Description => "Spawns something in front of your face";
        
        public override List<string> GetAutoComplete(int argIndex, string[] args)
        {
            if (argIndex == 0)
            {
                string firstArg = "";
                if (args != null)
                    firstArg = args[0];
                List<string> list = new List<string>();
                int i = -1;
                foreach (IdentifiableType type in SR2EMain.identifiableTypes)
                {
                    if (i > 20)
                        break;
                    try
                    {
                        if (type.LocalizedName != null)
                        {
                            string localizedString = type.LocalizedName.GetLocalizedString();
                            if (localizedString.ToLower().Replace(" ", "").StartsWith(firstArg.ToLower()))
                            {
                                i++;
                                list.Add(localizedString.Replace(" ", ""));
                            }
                        }
                    }
                    catch (Exception ignored) { }

                }

                return list;
            }
            if (argIndex == 1)
            {
                
                List<string> list = new List<string>();
                list.Add("1");
                list.Add("5");
                list.Add("10");
                list.Add("20");
                list.Add("30");
                list.Add("50");
                return list;
            }

            return null;
        }
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
            IdentifiableType type = SR2EMain.getIdentifiableByName(identifierTypeName);

            if (type == null)
            {
                type = SR2EMain.getIdentifiableByLocalizedName(identifierTypeName.Replace("_", ""));
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
                
            //if(type.ReferenceId.StartsWith("GadgetDefinition"))
            //{ SR2Console.SendError(args[0] + " is a gadget, not an item!"); return false; }
            
            for (int i = 0; i < amount; i++)
            {
                if (Physics.Raycast(new Ray(Camera.main.transform.position, Camera.main.transform.forward), out var hit))
                {
                    try
                    {
                        var spawned = SRBehaviour.InstantiateActor(type.prefab, SceneContext.Instance.Player.GetComponent<RegionMember>().SceneGroup, hit.point,Quaternion.identity,true, SlimeAppearance.AppearanceSaveSet.NONE,SlimeAppearance.AppearanceSaveSet.NONE);
                        //var spawned = SRBehaviour.Instantiate(type.prefab, hit.point,Quaternion.identity);
                        spawned.transform.position = hit.point+hit.normal*PhysicsUtil.CalcRad(spawned.GetComponent<Collider>());
                        var delta = -(hit.point - Camera.main.transform.position).normalized;
                        spawned.transform.rotation = Quaternion.LookRotation(delta, hit.normal);
                        if(type is GadgetDefinition)
                         SceneContext.Instance.ActorRegistry.Register(spawned.GetComponent<Gadget>());
                    }
                    catch (Exception ignored)
                    { }
                }
            }
            
            
            SR2Console.SendMessage($"Successfully spawned {amount} {itemName}");
            
            return true;
        }
    }

    
}