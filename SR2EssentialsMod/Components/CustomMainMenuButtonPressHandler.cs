using SR2E.Buttons;
using SR2E.Patches.MainMenu;

namespace SR2E.Components;

[RegisterTypeInIl2Cpp(false)]
internal class CustomMainMenuButtonPressHandler : MonoBehaviour
{
    public void OnEnable()
    {
        foreach (CustomMainMenuButton button in SR2MainMenuButtonPatch.buttons)
            if (button.label.TableEntryReference.Key+"ButtonStarter(Clone)" == gameObject.name)
            {
                button.action.Invoke();
                break;
            }
        Destroy(gameObject);
    }
}