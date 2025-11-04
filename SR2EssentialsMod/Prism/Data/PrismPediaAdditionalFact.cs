using UnityEngine.Localization;

namespace SR2E.Prism.Data;

public struct PrismPediaAdditionalFact
{
    public LocalizedString title;
    public LocalizedString description;
    public Sprite icon;

    public static PrismPediaAdditionalFact Create(LocalizedString title, LocalizedString description, Sprite icon)
    {
        return new PrismPediaAdditionalFact { title = title, description = description, icon = icon };
    }

    public static PrismPediaAdditionalFact[] From(params PrismPediaAdditionalFact[] array) => array;
}