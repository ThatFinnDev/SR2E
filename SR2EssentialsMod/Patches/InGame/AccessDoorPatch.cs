using Il2CppMonomiPark.World;
using SR2E.Commands;

namespace SR2E.Patches.InGame;

[HarmonyPatch(typeof(AccessDoor), nameof(AccessDoor.Awake))]
internal class AccessDoorPatch
{
    internal static void Postfix(AccessDoor __instance)
    {
        RanchCommand.accessDoors.Add(__instance);
        RanchCommand.accessDoors.RemoveAll(item => item == null);
        __instance.CurrState = GameContext.Instance.AutoSaveDirector.SavedGame.GameState.Ranch.AccessDoorStates[__instance.Id];
    }
}
