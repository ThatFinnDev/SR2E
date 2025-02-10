using SR2E.Enums;

namespace SR2E.Storage;

public struct MenuIdentifier
{
    public string translationKey { get; }
    public SR2EMenuTheme defaultTheme { get; }
    public string saveKey { get; }
    public SR2EMenuFont defaultFont { get; }

    public MenuIdentifier(string translationKey, SR2EMenuFont defaultFont, SR2EMenuTheme defaultTheme, string saveKey)
    {
        this.translationKey = translationKey;
        this.defaultFont = defaultFont;
        this.defaultTheme = defaultTheme;
        this.saveKey = saveKey;
    }
    public override string ToString() => $"MenuIdentifier {{ TranslationKey: \"{translationKey}\", DefaultFont: {defaultFont}, DefaultTheme: {defaultTheme}, SaveKey: \"{saveKey}\" }}";
    
}