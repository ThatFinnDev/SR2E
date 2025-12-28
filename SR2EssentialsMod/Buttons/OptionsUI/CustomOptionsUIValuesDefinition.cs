using Il2CppMonomiPark.SlimeRancher.DataModel;
using Il2CppMonomiPark.SlimeRancher.Options;
using Il2CppMonomiPark.SlimeRancher.UI.Pause;
using SR2E.Enums;
using SR2E.Storage;

namespace SR2E.Buttons.OptionsUI;

[InjectClass]
internal class CustomOptionsUIValuesDefinition : ScriptedValuePresetOptionDefinition
{
    internal CustomOptionsUIButtonValues button;
    public override void ApplyPresetSelection(int index)
    {
        if(!string.IsNullOrWhiteSpace(button.saveid))
            SR2EOptionsButtonManager.SetValuesButton(button.saveid, index);
        try { button.onModify.Invoke(index); }catch (Exception e) { MelonLogger.Error(e); }
        
    }

    public override OptionsItemModel CreateOptionItemModel()
    {
        try
        {
            return base.CreateOptionItemModel();
        }
        catch (Exception e)
        {
        }
        MelonLogger.Msg("Uhmmmmm");
        return null;
    }

    private int askedForPreset = 0;
    public override int GetDefaultPresetIndex()
    {
        askedForPreset++;
        if (askedForPreset < 2)
        {
            if (!string.IsNullOrWhiteSpace(button.saveid))
                return SR2EOptionsButtonManager.GetValuesButton(button.saveid, _defaultValueIndex);
        }
        else askedForPreset--;
        return _defaultValueIndex;
    }

    public override bool ShouldDisplay()
    {
        switch (button.visibleState)
        {
            case OptionsUIVisibleState.All: return true;
            case OptionsUIVisibleState.InGameOnly: return inGame;
            case OptionsUIVisibleState.MainMenuOnly: return SR2EEntryPoint.mainMenuLoaded;
        }
        return false;
    }
    public override bool ShouldEnable() => ShouldDisplay();

    public override string ReferenceId
    {
        get
        {
            _referenceId = button.refID;
            return button.refID;
        }
    }
}