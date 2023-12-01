using Il2CppMonomiPark.SlimeRancher.Player.CharacterController;
using SR2E.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace SR2E
{
    public static class SavePatches
    {
        [HarmonyPatch(typeof(IdentifiableActor), nameof(IdentifiableActor.Awake))]
        public static class ExtraSlimeSavedDataPatch
        {
            public static void Postfix(IdentifiableActor __instance)
            {
                var dataSaver = __instance.gameObject.AddComponent<SR2ESavableData.SR2ESlimeDataSaver>();
            }
        }
        
        [HarmonyPatch(typeof(SRCharacterController), nameof(SRCharacterController.Start))]
        public static class NoclipStateLoadPatch
        {
            public static void Postfix(SRCharacterController __instance)
            {
                NoClipCommand.RemoteExc(SR2ESavableData.Instance.playerSavedData.noclipState);
            }
        }

        [HarmonyPatch(typeof(AutoSaveDirector), nameof(AutoSaveDirector.SaveGame))]
        public static class AutoSaveDirectorSavePatch
        {
            public static bool Prefix()
            {
                foreach (var savableSlime in Resources.FindObjectsOfTypeAll<SR2ESavableData.SR2ESlimeDataSaver>())
                {
                    savableSlime.SaveData();
                }

                SR2ESavableData.Instance.TrySave();
                return true;
            }
        }

        [HarmonyPatch(nameof(FileStorageProvider),"savePath", MethodType.Getter)]
        public static class StorageProviderPathGrabber
        {
            static void Postfix(ref string __result)
            {
                AutoSaveDirectorLoadPatch.loadPath = __result;
            }
        }

        [HarmonyPatch(typeof(AutoSaveDirector), nameof(AutoSaveDirector.BeginLoad))]
        public static class AutoSaveDirectorLoadPatch
        {
            public static string loadPath;
            public static void Postfix(AutoSaveDirector __instance, string gameName, string saveName, Il2CppSystem.Action onError)
            {
                if (File.Exists(Path.Combine(loadPath, $"{saveName}.sr2e")))
                {
                    try
                    {
                        SR2ESavableData.LoadFromStream(new FileStream(Path.Combine(loadPath, $"{saveName}.sr2e"), FileMode.Open));
                    }
                    catch (Exception ex)
                    {
                        SR2Console.SendWarning("Failed to load SR2E save data, creating new");
                        SR2Console.SendWarning($"Developer error: {ex}");
                        var stream = new FileStream(Path.Combine(loadPath, $"{saveName}.sr2e"), FileMode.OpenOrCreate);
                        new SR2ESavableData();
                        SR2ESavableData.currStream = stream;
                        SR2ESavableData.currPath = Path.Combine(loadPath, $"{saveName}.sr2e");
                    }
                }
                else
                {
                    var stream = new FileStream(Path.Combine(loadPath, $"{saveName}.sr2e"), FileMode.CreateNew);
                    new SR2ESavableData();
                    SR2ESavableData.currStream = stream;
                    SR2ESavableData.currPath = Path.Combine(loadPath, $"{saveName}.sr2e");
                }
                SR2ESavableData.SR2ESlimeDataSaver.LoadData();
                NoClipCommand.RemoteExc(SR2ESavableData.Instance.playerSavedData.noclipState);

            }
        }
    }

    [Serializable]
    internal class SR2ESavableData
    {
        public static Stream currStream;
        public static string currPath;
        public static SR2ESavableData Instance;
        public SR2ESavableData()
        {
            Instance = this;
            gordoSavedData = new Dictionary<string, SR2EGordoData>();
            slimeSavedData = new Dictionary<long, SR2ESlimeData>();
        }
        internal class SR2ESlimeDataSaver : MonoBehaviour
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
            
            public static void LoadData()
            {
                var model = SceneContext.Instance.GameModel.identifiables;

                foreach (var slime in SR2ESavableData.Instance.slimeSavedData)
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

        public void IncreaseSaveIndex()
        {

        }

        [Serializable]
        public struct SR2EGordoData
        {
            public float baseSize;
        }
        [Serializable]
        public struct SR2EPlayerData
        {
            public bool noclipState;
        }
        [Serializable]
        public struct SR2ESlimeData
        {
            public float scaleX;
            public float scaleY;
            public float scaleZ;
        }

        /// <summary>
        /// Gordo ID to Gordo Data
        /// </summary>
        public Dictionary<string, SR2EGordoData> gordoSavedData;

        /// <summary>
        /// Actor ID to Slime Data
        /// </summary>
        public Dictionary<long, SR2ESlimeData> slimeSavedData;

        public SR2EPlayerData playerSavedData = new SR2EPlayerData();

        public void SaveToStream(Stream stream)
        {
            var formatter = new BinaryFormatter();
            formatter.Serialize(stream, this);
        }

        public void DebugSaveToNewFile(string path)
        {
            var stream = new FileStream(path, FileMode.OpenOrCreate);
            var formatter = new BinaryFormatter();
            formatter.Serialize(stream, this);
        }
        public void TrySave()
        {
            var formatter = new BinaryFormatter();
            formatter.Serialize(currStream, this);
        }
        public static SR2ESavableData LoadFromStream(Stream stream)
        {
            if (currStream != null) currStream.Close();
            var formatter = new BinaryFormatter();
            SR2ESavableData save = (SR2ESavableData)formatter.Deserialize(stream);
            currStream = stream;
            return save;
        }
    }
}
