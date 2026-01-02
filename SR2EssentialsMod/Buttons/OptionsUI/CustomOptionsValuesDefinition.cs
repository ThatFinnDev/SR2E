using Il2CppMonomiPark.SlimeRancher.DataModel;
using Il2CppMonomiPark.SlimeRancher.Options;
using Il2CppMonomiPark.SlimeRancher.UI.Pause;
using SR2E.Enums;
using SR2E.Storage;

namespace SR2E.Buttons.OptionsUI;

[InjectClass]
internal class CustomOptionsValuesDefinition : ScriptedValuePresetOptionDefinition
{
    internal CustomOptionsButtonValues button;
    public override void ApplyPresetSelection(int index)
    {
        if(!string.IsNullOrWhiteSpace(button.saveid))
            SR2EOptionsButtonManager.SetValuesButton(button.type,button.saveid, index);
        try { button.onModify.Invoke(index); }catch (Exception e) { MelonLogger.Error(e); }
    }


    private int askedForPreset = 0;
    public override int GetDefaultPresetIndex()
    {
        if (!string.IsNullOrWhiteSpace(button.saveid))
            return SR2EOptionsButtonManager.GetValuesButton(button.type,button.saveid, _defaultValueIndex);
        return _defaultValueIndex;
    }

    public override bool ShouldDisplay()
    {
        switch (button.type)
        {
            case OptionsButtonType.OptionsUI: return true;
            case OptionsButtonType.InGameOptionsUIOnly: return inGame;
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