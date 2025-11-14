using Il2CppMonomiPark.SlimeRancher;

namespace SR2E.Prism.Lib;
/// <summary>
/// A library of helper functions for dealing with saving and loading
/// </summary>
public class PrismLibSaving
{
    internal static Dictionary<string, IdentifiableType> savedIdents = new ();
    
    
    /// <summary>
    /// Setups an identifiable type for saving
    /// </summary>
    /// <param name="ident">The identifiable type to setup</param>
    /// <param name="RefID">The reference ID to use for saving</param>
    public static void SetupForSaving(IdentifiableType ident, string RefID = null)
    {
        if (ident == null) return;
        if (string.IsNullOrWhiteSpace(RefID)) RefID = ident.ReferenceId;
        savedIdents.TryAdd(RefID, ident);

        if(!autoSaveDirector._configuration._identifiableTypes.IsMember(ident))
            gameContext.LookupDirector.AddIdentifiableTypeToGroup(ident, autoSaveDirector._configuration._identifiableTypes);
        gameContext.LookupDirector._identifiableTypeByRefId.TryAdd(RefID, ident);
        SetupSaveForIdent(RefID, ident);
    }

    private static void SetupSaveForIdent(string RefID, IdentifiableType ident)
    {
        var t = autoSaveDirector._saveReferenceTranslation;
        t._identifiableTypeLookup.TryAdd(RefID, ident);

        if (t._identifiableTypeToPersistenceId._primaryIndex.Count > 0)
            if (!t._identifiableTypeToPersistenceId._primaryIndex.Contains(RefID))
                t._identifiableTypeToPersistenceId._primaryIndex =
                    t._identifiableTypeToPersistenceId._primaryIndex.AddToNew(RefID);

        t._identifiableTypeToPersistenceId._reverseIndex.TryAdd(RefID,
            t._identifiableTypeToPersistenceId._reverseIndex.Count);

        if (ident is SlimeDefinition)
        {
            gameContext.SlimeDefinitions._slimeDefinitionsByIdentifiable.TryAdd(ident, ident.Cast<SlimeDefinition>());
            if (!gameContext.SlimeDefinitions.Slimes.Contains(ident.Cast<SlimeDefinition>()))
                gameContext.SlimeDefinitions.Slimes = gameContext.SlimeDefinitions.Slimes.AddToNew(ident.Cast<SlimeDefinition>());
        }

        ident.referenceId = RefID;
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
                SetupSaveForIdent(refresh.Key, refresh.Value);
        }
    }
}