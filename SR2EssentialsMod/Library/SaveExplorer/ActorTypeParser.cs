using Il2CppMonomiPark.SlimeRancher.Persist;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SR2E.Library.SaveExplorer
{
    internal static class ActorTypeParser
    {
        public static (string, IdentifiableType) ConvertToLocalized(int id)
        {
            var ASD = GameContext.Instance.AutoSaveDirector;
            var SG = ASD.SavedGame;
            var str = SG.persistenceIdToIdentifiableType._indexTable[id];
            var ident = SG.identifiableTypeLookup[str];
            return (ident.localizedName.GetLocalizedString(), ident);
        }
        
        public static (string, IdentifiableType) ConvertToLocalized(this ActorDataV01 saved)
        {
            int id = saved.TypeId;
            var ASD = GameContext.Instance.AutoSaveDirector;
            var SG = ASD.SavedGame;
            var str = SG.persistenceIdToIdentifiableType._indexTable[id];
            var ident = SG.identifiableTypeLookup[str];
            return (ident.localizedName.GetLocalizedString(), ident);
        }

        public static string ConvertToLocalized_OnlyString(int id)
        {

            var ASD = GameContext.Instance.AutoSaveDirector;
            var SG = ASD.SavedGame;
            var str = SG.persistenceIdToIdentifiableType._indexTable[id];
            var ident = SG.identifiableTypeLookup[str];
            return ident.localizedName.GetLocalizedString();
        }
        
        public static string ConvertToLocalized_OnlyString(this ActorDataV01 saved)
        {
            int id = saved.TypeId;
            var ASD = GameContext.Instance.AutoSaveDirector;
            var SG = ASD.SavedGame;
            var str = SG.persistenceIdToIdentifiableType._indexTable[id];
            var ident = SG.identifiableTypeLookup[str];
            return ident.localizedName.GetLocalizedString();
        }

        public static IdentifiableType ConvertToType(int id)
        {
            var ASD = GameContext.Instance.AutoSaveDirector;
            var SG = ASD.SavedGame;
            var str = SG.persistenceIdToIdentifiableType._indexTable[id];
            return SG.identifiableTypeLookup[str];
        }
        public static IdentifiableType ConvertToType(this ActorDataV01 saved)
        {
            int id = saved.TypeId;
            var ASD = GameContext.Instance.AutoSaveDirector;
            var SG = ASD.SavedGame;
            var str = SG.persistenceIdToIdentifiableType._indexTable[id];
            return SG.identifiableTypeLookup[str];
        }
    }
}
