using Il2CppMonomiPark.SlimeRancher.UI.Pause;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SR2E.Storage;

namespace SR2E.Buttons;

[InjectClass]
internal class CustomPauseItemModel : ResumePauseItemModel
{
    internal System.Action action;
    public override void InvokeBehavior()
    {
        action.Invoke();
        return;
    }
}
