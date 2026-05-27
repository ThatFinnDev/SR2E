using System;
using Il2CppMonomiPark.SlimeRancher;
using Il2CppMonomiPark.SlimeRancher.DataModel;
using Il2CppMonomiPark.SlimeRancher.Persist;

namespace Starlight.Storage;

public class SavingGameSessionData
{
    public readonly ISaveReferenceTranslation ISaveReferenceTranslation;
    public readonly SaveReferenceTranslation SaveReferenceTranslation;
    public readonly GameV10 GameV10;
    public readonly dynamic GameState;
    public readonly GameModel GameModel;
    public readonly GameMetadata GameMetadata;
    public readonly SavedGameInfoProvider SavedGameInfoProvider;

    internal SavingGameSessionData(ISaveReferenceTranslation iSaveReferenceTranslation, SaveReferenceTranslation saveReferenceTranslation, 
        dynamic gameState, GameModel gameModel, GameMetadata gameMetadata, SavedGameInfoProvider savedGameInfoProvider)
    {
        this.GameMetadata = gameMetadata;
        this.ISaveReferenceTranslation = iSaveReferenceTranslation;
        this.SaveReferenceTranslation = saveReferenceTranslation;
        this.GameState = gameState;
        if(gameState is GameV10 gameV10)
            this.GameV10 = gameV10;
        this.GameModel = gameModel;
        this.SavedGameInfoProvider = savedGameInfoProvider;
    }
}
