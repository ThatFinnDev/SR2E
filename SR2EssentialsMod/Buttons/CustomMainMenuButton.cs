using Il2CppMonomiPark.SlimeRancher.UI.ButtonBehavior;
using Il2CppMonomiPark.SlimeRancher.UI.MainMenu;
using UnityEngine.Localization;
using SR2E.Patches.MainMenu;
using System;
using Il2CppInterop.Runtime.Injection;
namespace SR2E.Buttons;

[RegisterTypeInIl2Cpp(false)]
public class ModsMenuButtonBehaviorModel : ButtonBehaviorModel
{
    public ModsMenuButtonBehaviorModel(IntPtr ptr) : base(ptr) { }
    public ModsMenuButtonBehaviorModel(ButtonBehaviorDefinition def) : base(def) { }
    public ModsMenuButtonBehaviorModel() : base(ClassInjector.DerivedConstructorPointer<ModsMenuButtonBehaviorModel>()) => ClassInjector.DerivedConstructorBody(this);


    public override void InvokeBehavior()
    {
        SR2EModMenu.Open();
    }
}

[RegisterTypeInIl2Cpp(false)]
public class ModsMenuButtonBehaviorDef : ButtonBehaviorDefinition
{
    public ModsMenuButtonBehaviorDef(IntPtr ptr) : base(ptr)
    {
        SR2MainMenuButtonPatch.buttons.Add(this);
    }
    public ModsMenuButtonBehaviorDef() : base(ClassInjector.DerivedConstructorPointer<ModsMenuButtonBehaviorDef>()) => ClassInjector.DerivedConstructorBody(this);

    public override ButtonBehaviorModel CreateButtonBehaviorModel()
    { 
        return new ModsMenuButtonBehaviorModel(this);
    }
}