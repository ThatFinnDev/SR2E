namespace SR2E.Enums;
// Make it public on release
internal enum OptionsButtonType
{
    /// <summary>
    /// If you use this, saveID will be used to store the state outside a save file
    /// </summary>
    OptionsUI=0, 
    /// <summary>
    /// If you use this, saveID will be used to store the state inside a save file
    /// </summary>
    InGameOptionsUIOnly=1
}