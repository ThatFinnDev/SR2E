using Il2CppMonomiPark.SlimeRancher;
using Il2CppMonomiPark.SlimeRancher.DataModel;
using Il2CppMonomiPark.SlimeRancher.Persist;
using SR2E.Storage;

namespace SR2E.Patches.Dev;


[DevPatch()]
[HarmonyPatch(typeof(GameModelPushHelpers), nameof(GameModelPushHelpers.PushGame))]
public static class PushGamePatch
{
    public static ActorIdProvider tmpActorIdProvider;
    public static GameV08 tmpGameState;
    public static ISaveReferenceTranslation tmpSaveReferenceTranslation;
    public static GameModel tmpGameModel;
    internal static void Postfix(ActorIdProvider actorIdProvider, ISaveReferenceTranslation saveReferenceTranslation, GameV08 gameState, GameModel gameModel)
    {
        tmpActorIdProvider=actorIdProvider;
        tmpSaveReferenceTranslation=saveReferenceTranslation;
        tmpGameState=gameState;
        tmpGameModel=gameModel;
    }

}