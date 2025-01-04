namespace SR2E.Storage;

public struct MenuIdentifier
{
    public string translationKey { get; }
    public SR2EMenuTheme defaultTheme { get; }
    public string saveKey { get; }

    public MenuIdentifier(string translationKey, SR2EMenuTheme defaultTheme, string saveKey)
    {
        this.translationKey = translationKey;
        this.defaultTheme = defaultTheme;
        this.saveKey = saveKey;
    }
    public override string ToString() => $"MenuIdentifier {{ TranslationKey: \"{translationKey}\", DefaultTheme: {defaultTheme}, SaveKey: \"{saveKey}\" }}";
    
}