using Il2CppMonomiPark.SlimeRancher;
using Il2CppMonomiPark.SlimeRancher.DataModel;
using Il2CppMonomiPark.SlimeRancher.Persist;

namespace SR2E.Storage;

public class LoadingGameSessionData
{
    public readonly ActorIdProvider actorIdProvider;
    public readonly ISaveReferenceTranslation iSaveReferenceTranslation;
    public readonly SaveReferenceTranslation saveReferenceTranslation;
    public readonly GameV09 gameV09;
    public readonly GameModel gameModel;

    internal LoadingGameSessionData(ActorIdProvider actorIdProvider, ISaveReferenceTranslation iSaveReferenceTranslation,
        SaveReferenceTranslation saveReferenceTranslation, GameV09 gameV09, GameModel gameModel)
    {
        this.actorIdProvider = actorIdProvider;
        this.iSaveReferenceTranslation = iSaveReferenceTranslation;
        this.saveReferenceTranslation = saveReferenceTranslation;
        this.gameV09 = gameV09;
        this.gameModel = gameModel;
    }
}