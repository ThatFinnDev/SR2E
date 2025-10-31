using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppMonomiPark.SlimeRancher.Pedia;
using UnityEngine.Localization;

namespace SR2E.Cotton;

public static partial class CottonLibrary
{
    public static class Pedia
    {
            public static IdentifiablePediaEntry CreatePediaEntryForSlime(SlimeDefinition slime, string persistID, LocalizedString intro, LocalizedString slimeology, LocalizedString risks, LocalizedString plort)
            {
                return CreateIdentPediaEntry(
                    slime,
                    persistID,
                    Get<IdentifiablePediaEntry>("Pink"),
                    intro,
                    PediaCategoryType.Slimes,
                    PediaDetail.Params(
                        PediaDetail.Create(0, slimeology, PediaDetailType.Slimeology),
                        PediaDetail.Create(1, risks, PediaDetailType.Risk),
                        PediaDetail.Create(2, plort, PediaDetailType.Plort)
                    )
                );
            }

    private static Dictionary<PediaDetailType, PediaDetailSection> pediaDetails = new Dictionary<PediaDetailType, PediaDetailSection>();

    internal static void PediaDetailInitialize()
    {
        pediaDetails.Add(PediaDetailType.HowToUse, Get<PediaDetailSection>("How To Use"));
        pediaDetails.Add(PediaDetailType.About, Get<PediaDetailSection>("About"));
        pediaDetails.Add(PediaDetailType.Plort, Get<PediaDetailSection>("Plortonomics"));
        pediaDetails.Add(PediaDetailType.Risk, Get<PediaDetailSection>("Rancher Risks"));
        pediaDetails.Add(PediaDetailType.Slimeology, Get<PediaDetailSection>("Slimeology"));
    }
    
    public enum PediaDetailType
    {
        Slimeology,
        Plort,
        Risk,
        About,
        HowToUse,
    }
    public enum PediaCategoryType
    {
        Slimes,
        Tutorial,
        Science,
        World,
        Ranch,
        Toys,
        Resources,
        Blueprints,
        Weather,
    }
    public struct PediaDetail
    {
        public int index;
        public LocalizedString text;
        public PediaDetailType type;

        public static PediaDetail Create(int index, LocalizedString text, PediaDetailType type)
        {
            return new PediaDetail { index = index, text = text, type = type };
        }

        public static PediaDetail[] Params(params PediaDetail[] array)
        {
            return array;
        } 
    }

    public static IdentifiablePediaEntry CreateIdentPediaEntry(IdentifiableType ident, string persistID, IdentifiablePediaEntry baseEntry, LocalizedString intro, PediaCategoryType category, PediaDetail[] details)
    {
        var newEntry = Object.Instantiate(baseEntry);

        newEntry._title = ident.localizedName;
        newEntry._identifiableType = ident;
        newEntry._description = intro;

        int c = newEntry._details.Count;
        LocalizedString gamepadText = null;
        foreach (var detail in details)
        {
            if (detail.index < c)
            {
                gamepadText = newEntry._details[detail.index].TextGamepad;
                newEntry._details[detail.index].Text = detail.text;
                newEntry._details[detail.index].Section = pediaDetails[detail.type];
            }
            else
            {
                var newDetail = new PediaEntryDetail()
                {
                    Contents = new Il2CppReferenceArray<PediaEntryDetailSubContent>(0),
                    Section = pediaDetails[detail.type],
                    Text = detail.text,
                    TextGamepad = gamepadText,
                    TextPS4 = gamepadText,
                };
            }
        }

        pediaEntries[category].Add(newEntry);
        
        newEntry.name = persistID;
        newEntry._identifiableType._pediaPersistenceSuffix = persistID;
        
        return newEntry;
    }

    internal static Dictionary<PediaCategoryType, List<PediaEntry>> pediaEntries = new Dictionary<PediaCategoryType, List<PediaEntry>>()
    {
        {PediaCategoryType.Slimes, new List<PediaEntry>()},
        {PediaCategoryType.Resources, new List<PediaEntry>()},
        {PediaCategoryType.Ranch, new List<PediaEntry>()},
        {PediaCategoryType.Weather, new List<PediaEntry>()},
        {PediaCategoryType.Toys, new List<PediaEntry>()},
        {PediaCategoryType.Blueprints, new List<PediaEntry>()},
        {PediaCategoryType.Science, new List<PediaEntry>()},
        {PediaCategoryType.Tutorial, new List<PediaEntry>()},
        {PediaCategoryType.World, new List<PediaEntry>()},
    };
    public static FixedPediaEntry CreateFixedPediaEntry(FixedPediaEntry baseEntry, string persistID, LocalizedString title, LocalizedString intro, Sprite icon, PediaCategoryType category,  PediaDetail[] details)
    {
        var newEntry = Object.Instantiate(baseEntry);

        newEntry._title = title;
        newEntry._description = intro;
        newEntry._icon = icon;
        
        int c = newEntry._details.Count;
        LocalizedString gamepadText = null;
        foreach (var detail in details)
        {
            if (detail.index < c)
            {
                gamepadText = newEntry._details[detail.index].TextGamepad;
                newEntry._details[detail.index].Text = detail.text;
                newEntry._details[detail.index].Section = pediaDetails[detail.type];
            }
            else
            {
                var newDetail = new PediaEntryDetail()
                {
                    Contents = new Il2CppReferenceArray<PediaEntryDetailSubContent>(0),
                    Section = pediaDetails[detail.type],
                    Text = detail.text,
                    TextGamepad = gamepadText,
                    TextPS4 = gamepadText,
                };
            }
        }
        
        pediaEntries[category].Add(newEntry);

        newEntry.name = persistID;
        newEntry._persistenceSuffix = persistID;
        
        
        return newEntry;
    }

    }
}