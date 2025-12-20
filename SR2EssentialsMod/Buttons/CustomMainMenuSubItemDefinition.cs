using Il2CppMonomiPark.SlimeRancher.UI.MainMenu.Definition;
using Il2CppMonomiPark.SlimeRancher.UI.MainMenu.Definition.ButtonBehavior;
using SR2E.Storage;

namespace SR2E.Buttons;

[InjectClass]
internal class CustomMainMenuSubItemDefinition : SubMenuItemDefinition
{
    internal System.Action customAction;
}
