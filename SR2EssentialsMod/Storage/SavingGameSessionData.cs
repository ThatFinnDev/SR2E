using Il2CppMonomiPark.SlimeRancher;
using Il2CppMonomiPark.SlimeRancher.DataModel;
using Il2CppMonomiPark.SlimeRancher.Persist;

namespace SR2E.Storage;

public class SavingGameSessionData
{
    public readonly ISaveReferenceTranslation iSaveReferenceTranslation;
    public readonly SaveReferenceTranslation saveReferenceTranslation;
    public readonly GameV09 gameV09;
    public readonly GameModel gameModel;
    public readonly GameMetadata gameMetadata;
    public readonly SavedGameInfoProvider savedGameInfoProvider;

    internal SavingGameSessionData(ISaveReferenceTranslation iSaveReferenceTranslation, SaveReferenceTranslation saveReferenceTranslation, 
        GameV09 gameV09, GameModel gameModel, GameMetadata gameMetadata, SavedGameInfoProvider savedGameInfoProvider)
    {
        this.gameMetadata = gameMetadata;
        this.iSaveReferenceTranslation = iSaveReferenceTranslation;
        this.saveReferenceTranslation = saveReferenceTranslation;
        this.gameV09 = gameV09;
        this.gameModel = gameModel;
        this.savedGameInfoProvider = savedGameInfoProvider;
    }
}
