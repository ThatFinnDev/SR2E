using Il2CppMonomiPark.SlimeRancher.World.Teleportation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SR2E.Patches;

[HarmonyPatch(typeof(TeleporterNode), nameof(TeleporterNode.OnTriggerEnter))]
internal class TeleporterUsePatch
{
    public static void Prefix(Collider collider)
    {
        if (collider.gameObject == SceneContext.Instance.player)
            GameContext.Instance.AutoSaveDirector.SaveGame();
    }
}