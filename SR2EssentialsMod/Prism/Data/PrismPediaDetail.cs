using UnityEngine.Localization;

namespace SR2E.Prism.Data;

public struct PrismPediaDetail
{
    public LocalizedString text;
    public PrismPediaDetailType type;

    public static PrismPediaDetail Create(PrismPediaDetailType type, LocalizedString text)
    {
        return new PrismPediaDetail { text = text, type = type };
    }

    public static PrismPediaDetail[] From(params PrismPediaDetail[] array) => array;
}