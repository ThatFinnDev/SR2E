using Il2CppMonomiPark.SlimeRancher.UI.MainMenu.Definition;
using Il2CppMonomiPark.SlimeRancher.UI.MainMenu.Definition.ButtonBehavior;

namespace SR2E.Buttons;

[RegisterTypeInIl2Cpp(false)]
internal class CustomMainMenuSubItemDefinition : SubMenuItemDefinition
{
    internal System.Action customAction;
}
