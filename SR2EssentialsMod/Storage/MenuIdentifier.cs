namespace SR2E.Storage;

public struct MenuIdentifier
{
    public bool hasThemes { get; }
    public string translationKey { get; }
    public SR2EMenuTheme defaultTheme { get; }
    public string saveKey { get; }

    public MenuIdentifier(bool hasThemes, string translationKey, SR2EMenuTheme defaultTheme, string saveKey)
    {
        this.hasThemes = hasThemes;
        this.translationKey = translationKey;
        this.defaultTheme = defaultTheme;
        this.saveKey = saveKey;
    }
}