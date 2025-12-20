using Il2CppMonomiPark.SlimeRancher.UI.MainMenu.Definition;
using SR2E.Storage;

namespace SR2E.Buttons;

[InjectClass]
internal class CustomMainMenuItemDefinition : LoadGameItemDefinition
{
    internal System.Action customAction;
}
