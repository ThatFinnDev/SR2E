using Il2CppMonomiPark.SlimeRancher.UI.Pause;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SR2E.Buttons;

[RegisterTypeInIl2Cpp(false)]
<<<<<<< HEAD
public class CustomPauseItemModel : ResumePauseItemModel
{
    public System.Action action;
=======
internal class CustomPauseItemModel : ResumePauseItemModel
{
    internal System.Action action;
>>>>>>> experimental
    public override void InvokeBehavior()
    {
        action.Invoke();
        return;
    }
}
