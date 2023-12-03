using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SR2E.Saving
{
    public class SR2EGordoDataSaver : MonoBehaviour
    {
        public void SaveData()
        {
            var model = GetComponent<GordoEat>();
            SR2Console.SendMessage("1");
            var data = new SR2EGordoData()
            {
                baseSize = model._initScale
            };
            SR2Console.SendMessage("2");

            if (SR2ESavableData.Instance.gordoSavedData.ContainsKey(model.Id))
            {
                SR2ESavableData.Instance.gordoSavedData[model.Id] = data;
            }
            else
            {
                SR2ESavableData.Instance.gordoSavedData.Add(model.Id, data);
            }
        }

        public void LoadData()
        {
            if (SR2EEntryPoint.debugLogging)
                SR2Console.SendMessage($"load ident debug start: {gameObject.name}");
            var id = GetComponent<GordoEat>().Id;
            GetComponent<GordoEat>()._initScale = SR2ESavableData.Instance.gordoSavedData[id].baseSize;
            if (SR2EEntryPoint.debugLogging)
                SR2Console.SendMessage("loaded ident");
        }
        public void Start()
        {
            try
            {
                LoadData();
            }
            catch { }
        }
    }
}
