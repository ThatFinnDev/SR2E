/*using Il2CppMonomiPark.SlimeRancher.World.Teleportation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SR2E.Saving.Patches;

[HarmonyPatch(typeof(TeleporterNode), nameof(TeleporterNode.OnTriggerEnter))]
internal class TeleporterUsePatch
{
    internal static void Prefix(Collider collider)
    {
        if (collider.gameObject == SceneContext.Instance.player)
            GameContext.Instance.AutoSaveDirector.SaveGame();
    }
}*/
//Broken as of SR2 0.6.0