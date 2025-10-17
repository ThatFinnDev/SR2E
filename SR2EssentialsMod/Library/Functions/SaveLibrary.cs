using Il2Cpp;

namespace CottonLibrary;

public static partial class Library
{
    internal static void INTERNAL_SetupSaveForIdent(string RefID, IdentifiableType ident)
    {
        gameContext.AutoSaveDirector._saveReferenceTranslation._identifiableTypeLookup.TryAdd(RefID, ident);

        if (gameContext.AutoSaveDirector._saveReferenceTranslation._identifiableTypeToPersistenceId._primaryIndex.Count > 0)
            if (!gameContext.AutoSaveDirector._saveReferenceTranslation._identifiableTypeToPersistenceId._primaryIndex.Contains(RefID))
                gameContext.AutoSaveDirector._saveReferenceTranslation._identifiableTypeToPersistenceId._primaryIndex =
                    gameContext.AutoSaveDirector._saveReferenceTranslation._identifiableTypeToPersistenceId._primaryIndex
                        .AddString(RefID);

        gameContext.AutoSaveDirector._saveReferenceTranslation._identifiableTypeToPersistenceId._reverseIndex.TryAdd(RefID,
            gameContext.AutoSaveDirector._saveReferenceTranslation._identifiableTypeToPersistenceId._reverseIndex.Count);


        if (ident is SlimeDefinition)
        {
            if (!gameContext.SlimeDefinitions.Slimes.Contains(ident.Cast<SlimeDefinition>()))
                gameContext.SlimeDefinitions.Slimes.Add(ident.Cast<SlimeDefinition>());

            gameContext.SlimeDefinitions._slimeDefinitionsByIdentifiable.TryAdd(ident,
                ident.Cast<SlimeDefinition>());
        }

        ident.referenceId = RefID;
    }

    internal static void INTERNAL_SetupLoadForIdent(string RefID, IdentifiableType ident)
    {
        ident.SetupForSaving(RefID);

        gameContext.LookupDirector.AddIdentifiableTypeToGroup(ident, gameContext.AutoSaveDirector._configuration._identifiableTypes);

        INTERNAL_SetupSaveForIdent(RefID, ident);
    }}