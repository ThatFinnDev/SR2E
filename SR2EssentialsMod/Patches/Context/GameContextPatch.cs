using Il2CppMonomiPark.SlimeRancher.Damage;
using Il2CppMonomiPark.SlimeRancher.World.Teleportation;

namespace SR2E.Patches.Context;

[HarmonyPatch(typeof(GameContext), nameof(GameContext.Start))]
internal class GameContextPatch
{
    internal static void Postfix(GameContext __instance)
    {
        SR2EUtils._killDamage = new Damage
        {
            Amount = 99999999, DamageSource = ScriptableObject.CreateInstance<DamageSourceDefinition>(),
        };
        SR2EUtils._killDamage.DamageSource.hideFlags |= HideFlags.HideAndDontSave;
        AutoSaveDirector autoSaveDirector = GameContext.Instance.AutoSaveDirector;
        autoSaveDirector.saveSlotCount = SAVESLOT_COUNT.Get();

        foreach (ParticleSystemRenderer particle in
                 Resources.FindObjectsOfTypeAll<ParticleSystemRenderer>())
        {
            var pname = particle.gameObject.name.Replace(' ', '_');
            if (!FXLibrary.ContainsKey(particle.gameObject))
                FXLibrary.AddItems(particle.gameObject, particle, pname);
            if (!FXLibraryReversable.ContainsKey(pname))
                FXLibraryReversable.AddItems(pname, particle, particle.gameObject);
        }

        vaccableGroup = Get<IdentifiableTypeGroup>("VaccableNonLiquids");


        foreach (KeyValuePair<string, string> pair in teleportersToAdd)
            AddTeleporter(pair.Key, pair.Value);
        
        foreach (var expansion in SR2EEntryPoint.expansions)
            try
            {
                expansion.OnGameContext(__instance);
            } 
            catch (Exception e) { MelonLogger.Error(e); }
    }
    
    internal static Dictionary<string,string> teleportersToAdd = new Dictionary<string, string>()
    {
        {"SceneGroup.ConservatoryFields", "TeleporterHomeBlue"},
        {"SceneGroup.RumblingGorge", "TeleporterZoneGorge"},
        {"SceneGroup.LuminousStrand", "TeleporterZoneStrand"},
        {"SceneGroup.PowderfallBluffs", "TeleporterZoneBluffs"},
        {"SceneGroup.Labyrinth", "TeleporterZoneLabyrinth"},
    };
    internal static void AddTeleporter(string sceneGroup, string gadgetName)
    {
        StaticTeleporterNode teleporter = GameObject.Instantiate(getGadgetDefByName(gadgetName).prefab.transform.getObjRec<GadgetTeleporterNode>("Teleport Collider").gameObject.GetComponent<StaticTeleporterNode>());
        teleporter.gameObject.SetActive(false); teleporter.name = "TP-"+sceneGroup; teleporter.gameObject.MakePrefab(); teleporter.gameObject.MakePrefab(); teleporter._hasDestination = true;
        SR2ESaveManager.WarpManager.teleporters.TryAdd(sceneGroup, teleporter);
    }

}