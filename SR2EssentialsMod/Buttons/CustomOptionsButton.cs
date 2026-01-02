using Il2CppMonomiPark.SlimeRancher.Options;
using SR2E.Enums;

namespace SR2E.Buttons;
// Make it public on release
internal abstract class CustomOptionsButton
{
    public OptionsButtonType type = OptionsButtonType.OptionsUI;
    internal static List<string> usedIds = new List<string>();
    public int insertIndex;
    private OptionsItemDefinition _definition;
    protected virtual OptionsItemDefinition GenerateOptionsItemDefinition()
    {
        return null;
    }

    internal CustomOptionsButton()
    {
        
    }
    internal OptionsItemDefinition GetOptionsItemDef()
    {
        if (_definition != null) return _definition;
        try
        {
            _definition = GenerateOptionsItemDefinition();
            return _definition;
        } catch (Exception e) {MelonLogger.Error(e);}

        return null;
    }
}