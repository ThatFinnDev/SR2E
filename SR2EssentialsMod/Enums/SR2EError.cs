namespace SR2E.Enums;

public enum SR2EError
{
    NoError=0, 
    NotInGame=1, 
    PlayerNull=2, 
    TeleportablePlayerNull=3, 
    SRCharacterControllerNull=4,
    SceneGroupNotSupported=5, 
    AlreadyExists=6, 
    DoesntExist=7,
    SaveInvalidGeneral=8,
    NotInMainMenu=9,
    GameNotLoadedYet=10,
    FilePathNull=11,
    SomeSaveIDFailed=12,
    MainSaveIDFailed=13,
    LatestSaveInvalid=14,
    NeedExperiment=15,
    InvalidGameName=16,
    NoValidSaves=17,
    NoValidSummaries=18,
    NeedFlag=19,
}