using Il2CppMonomiPark.SlimeRancher.UI.Pause;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SR2E.Library.Buttons;

[RegisterTypeInIl2Cpp(false)]
public class CustomPauseItemModel : ResumePauseItemModel
{
    public System.Action action;
    public override void InvokeBehavior()
    {
        action.Invoke();
        return;
    }
}
