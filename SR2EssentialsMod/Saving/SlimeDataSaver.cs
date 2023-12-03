using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SR2E.Saving.SR2ESavableData;

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

            if (Instance.slimeSavedData.ContainsKey(model.GetActorId()))
            {
                Instance.slimeSavedData[model.GetActorId()] = data;
            }
            else
            {
                Instance.slimeSavedData.Add(model.GetActorId(), data);
            }
        }

        public static void LoadData()
        {
            var model = SceneContext.Instance.GameModel.identifiables;

            foreach (var slime in Instance.slimeSavedData)
            {
                if (model.ContainsKey(slime.Key))
                {
                    var slimeTransform = model[slime.Key].GetGameObject().transform;
                    var slimeData = slime.Value;

                    slimeTransform.localScale = new Vector3(slimeData.scaleX, slimeData.scaleY, slimeData.scaleZ);
                }
            }

        }
    }
}
