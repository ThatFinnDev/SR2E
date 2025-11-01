using Il2CppMonomiPark.SlimeRancher;

namespace SR2E.Cotton;

public static partial class CottonLibrary
{
    public static class Saving
    {
        internal static void INTERNAL_SetupSaveForIdent(string RefID, IdentifiableType ident)
        {
            gameContext.AutoSaveDirector._saveReferenceTranslation._identifiableTypeLookup.TryAdd(RefID, ident);

            if (gameContext.AutoSaveDirector._saveReferenceTranslation._identifiableTypeToPersistenceId._primaryIndex.Count > 0)
                if (!gameContext.AutoSaveDirector._saveReferenceTranslation._identifiableTypeToPersistenceId._primaryIndex.Contains(RefID))
                    gameContext.AutoSaveDirector._saveReferenceTranslation._identifiableTypeToPersistenceId._primaryIndex =
                        gameContext.AutoSaveDirector._saveReferenceTranslation._identifiableTypeToPersistenceId._primaryIndex.AddToNew(RefID);

            gameContext.AutoSaveDirector._saveReferenceTranslation._identifiableTypeToPersistenceId._reverseIndex
                .TryAdd(RefID,
                    gameContext.AutoSaveDirector._saveReferenceTranslation._identifiableTypeToPersistenceId
                        ._reverseIndex.Count);


            
            
            if (ident is SlimeDefinition)
            {
                if (!gameContext.SlimeDefinitions.Slimes.Contains(ident.Cast<SlimeDefinition>()))
                    gameContext.SlimeDefinitions.Slimes.AddToNew(ident.Cast<SlimeDefinition>());

                gameContext.SlimeDefinitions._slimeDefinitionsByIdentifiable.TryAdd(ident,
                    ident.Cast<SlimeDefinition>());
            }

            ident.referenceId = RefID;
        }

        internal static void INTERNAL_SetupLoadForIdent(string RefID, IdentifiableType ident)
        {
            SetupForSaving(ident,RefID);

            gameContext.LookupDirector.AddIdentifiableTypeToGroup(ident,
                gameContext.AutoSaveDirector._configuration._identifiableTypes);
            if(gameContext.LookupDirector._identifiableTypeByRefId.ContainsKey(RefID))
                gameContext.LookupDirector._identifiableTypeByRefId.Add(RefID,ident);
            INTERNAL_SetupSaveForIdent(RefID, ident);
        }

        public static void SetupForSaving(IdentifiableType ident, string RefID)
        {
            savedIdents.TryAdd(RefID, ident);
        }

        internal static void RefreshIfNotFound(SaveReferenceTranslation table, IdentifiableType ident)
        {
            try
            {
                table.GetPersistenceId(ident);
            }
            catch
            {
                foreach (var refresh in savedIdents)
                    INTERNAL_SetupSaveForIdent(refresh.Key, refresh.Value);
            }
        }
    }
}