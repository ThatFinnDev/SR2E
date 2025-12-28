using Il2CppMonomiPark.SlimeRancher.Options;

namespace SR2E.Buttons;

public abstract class CustomOptionsUIButton
{
    internal static List<string> usedIds = new List<string>();
    public int insertIndex;
    private OptionsItemDefinition _definition;
    protected virtual OptionsItemDefinition GenerateOptionsItemDefinition()
    {
        return null;
    }

    internal CustomOptionsUIButton()
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