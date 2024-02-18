using Il2CppMonomiPark.SlimeRancher.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SR2E.Library.SaveExplorer
{
    [RegisterTypeInIl2Cpp(false)]
    public class SaveExplorerEntryInteract : MonoBehaviour
    {
        internal int IDX;
        public void OnMouseDown()
        {
            PropertyStealer.Instance.selected = PropertyStealer.Instance.defaultProperties[IDX].GetValue(gameContext.AutoSaveDirector.SavedGame.gameState);
            PropertyStealer.Instance.Refresh();
        }
    }

    [RegisterTypeInIl2Cpp(false)]
    public class InspectorEntryInteract : MonoBehaviour
    {
        internal int IDX;
        public void OnMouseDown()
        {
            PropertyStealer.Instance.selected = PropertyStealer.Instance.selectedProperties[IDX].GetValue(PropertyStealer.Instance.selected);
            PropertyStealer.Instance.Refresh();
        }
    }
}
