using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppMonomiPark.SlimeRancher.Pedia;
using UnityEngine.Localization;

namespace SR2E.Cotton;
/*
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
}*/