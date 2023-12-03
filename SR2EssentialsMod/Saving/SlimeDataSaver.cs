using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SR2E.Saving
{
    public class SR2ESlimeDataSaver : MonoBehaviour
    {
        public void SaveData()
        {
            var model = GetComponent<IdentifiableActor>();

            var data = new SR2ESlimeData()
            {
                scaleX = transform.localScale.x,
                scaleY = transform.localScale.y,
                scaleZ = transform.localScale.z,
            };

            if (SR2ESavableData.Instance.slimeSavedData.ContainsKey(model.GetActorId()))
            {
                SR2ESavableData.Instance.slimeSavedData[model.GetActorId()] = data;
            }
            else
            {
                SR2ESavableData.Instance.slimeSavedData.Add(model.GetActorId(), data);
            }
        }

        public void LoadData()
        {
            SR2Console.SendMessage($"load ident debug start: {gameObject.name}");
            var id = GetComponent<IdentifiableActor>().model.actorId;
            transform.localScale = new Vector3(SR2ESavableData.Instance.slimeSavedData[id].scaleX, SR2ESavableData.Instance.slimeSavedData[id].scaleY, SR2ESavableData.Instance.slimeSavedData[id].scaleZ);
            SR2Console.SendMessage("loaded ident");
        }
        public void Start()
        {
            LoadData();
        }
    }
}
