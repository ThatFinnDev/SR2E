using Il2CppMonomiPark.SlimeRancher;
using Il2CppMonomiPark.SlimeRancher.DataModel;
using Il2CppMonomiPark.SlimeRancher.Persist;

namespace Starlight.Storage;

public class LoadingGameSessionData
{
    public readonly ActorIdProvider ActorIdProvider;
    public readonly ISaveReferenceTranslation ISaveReferenceTranslation;
    public readonly SaveReferenceTranslation SaveReferenceTranslation;
    public readonly GameV10 GameV10;
    public readonly dynamic GameState;
    public readonly GameModel GameModel;

    internal LoadingGameSessionData(ActorIdProvider actorIdProvider, ISaveReferenceTranslation iSaveReferenceTranslation,
        SaveReferenceTranslation saveReferenceTranslation, dynamic gameState, GameModel gameModel)
    {
        this.ActorIdProvider = actorIdProvider;
        this.ISaveReferenceTranslation = iSaveReferenceTranslation;
        this.SaveReferenceTranslation = saveReferenceTranslation;
        this.GameState = gameState;
        if(gameState is GameV10 gameV10)
            this.GameV10 = gameV10;
        this.GameModel = gameModel;
    }
}