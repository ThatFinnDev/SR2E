
﻿using Il2CppMonomiPark.SlimeRancher.Player.CharacterController;
using SR2E.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace SR2E
{
    public static class SavePatches
    {
        [HarmonyPatch(typeof(AutoSaveDirector), nameof(AutoSaveDirector.Awake))]
        public static class ExtraSlimeSavedDataPatch
        {
            public static void Postfix(IdentifiableActor __instance)
            {
                foreach (var ident in Resources.FindObjectsOfTypeAll<IdentifiableType>())
                {
                    if (ident.prefab != null)
                    {
                        var p = ident.prefab;
                        var dataSaver = p.AddComponent<SR2ESavableData.SR2ESlimeDataSaver>();
                    }
                }
            }
        }

        [HarmonyPatch(typeof(SRCharacterController), nameof(SRCharacterController.Start))]
        public static class NoclipStateLoadPatch
        {
            public static void Postfix(SRCharacterController __instance)
            {
                NoClipCommand.RemoteExc(SR2ESavableData.Instance.playerSavedData.noclipState);
                if (SR2ESavableData.Instance.playerSavedData.noclipState)
                {
                    SR2Console.SendMessage("Load noclip state debug");
                }
            }
        }

        [HarmonyPatch(typeof(AutoSaveDirector), nameof(AutoSaveDirector.SaveGame))]
        public static class AutoSaveDirectorSavePatch
        {
            public static void Prefix(AutoSaveDirector __instance)
            {
                MelonLogger.Msg("test");
                foreach (var savableSlime in Resources.FindObjectsOfTypeAll<SR2ESavableData.SR2ESlimeDataSaver>())
                {
                    try
                    {
                        savableSlime.SaveData();
                    }
                    catch
                    {

                    }
                }
                if (SR2ESavableData.Instance.idx != 5)
                {
                    SR2ESavableData.currPath = $"{Path.Combine(SR2ESavableData.Instance.dir, SR2ESavableData.Instance.gameName)}_{SR2ESavableData.Instance.idx + 1}.sr2e";
                    SR2ESavableData.Instance.idx++;
                }
                else
                {
                    SR2ESavableData.currPath = $"{Path.Combine(SR2ESavableData.Instance.dir, SR2ESavableData.Instance.gameName)}_{0}.sr2e";
                    SR2ESavableData.Instance.idx = 0;
                }
                MelonLogger.Warning(SR2ESavableData.currPath);
                SR2ESavableData.Instance.TrySave();
            }
        }

        [HarmonyPatch(nameof(FileStorageProvider), "savePath", MethodType.Getter)]
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
                        SR2ESavableData.currPath = Path.Combine(loadPath, $"{saveName}.sr2e");
                        SR2ESavableData.Instance.dir = $"{loadPath}\\";
                        SR2ESavableData.Instance.gameName = gameName;
                        SR2ESavableData.Instance.idx = int.Parse(saveName.Split('_')[2]);
                    }
                    catch (Exception ex)
                    {
                        SR2Console.SendWarning("Failed to load SR2E save data, creating new");
                        SR2Console.SendWarning($"Developer error: {ex}");
                        var stream = new FileStream(Path.Combine(loadPath, $"{saveName}.sr2e"), FileMode.OpenOrCreate);
                        new SR2ESavableData();
                        SR2ESavableData.currPath = Path.Combine(loadPath, $"{saveName}.sr2e");
                        SR2ESavableData.Instance.dir = $"{loadPath}\\";
                        SR2ESavableData.Instance.gameName = gameName;
                        SR2ESavableData.Instance.idx = int.Parse(saveName.Split('_')[2]);
                    }
                }
                else
                {
                    var stream = new FileStream(Path.Combine(loadPath, $"{saveName}.sr2e"), FileMode.CreateNew);
                    new SR2ESavableData();
                    SR2ESavableData.currPath = Path.Combine(loadPath, $"{saveName}.sr2e");
                    SR2ESavableData.Instance.dir = $"{loadPath}\\";
                    SR2ESavableData.Instance.gameName = gameName;
                    SR2ESavableData.Instance.idx = int.Parse(saveName.Split('_')[2]);
                }
                SR2ESavableData.SR2ESlimeDataSaver.LoadData();

            }
        }
    }

    [Serializable]
    internal class SR2ESavableData
    {

        public static string currPath;
        public string dir;
        public string gameName;
        public int idx;

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
            if (idx != 5)
            {
                currPath = $"{Path.Combine(dir, gameName)}_{idx + 1}.sr2e";
                idx++;
            }
            else
            {
                currPath = $"{Path.Combine(dir, gameName)}_{0}.sr2e";
                idx = 0;
            }
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
            var writer = new StreamWriter(stream);

            var json = JsonConvert.SerializeObject(this, Formatting.Indented);
            writer.Write(json);
        }

        public void DebugSaveToNewFile(string path)
        {
            try
            {
                var json = DebugPrint();
                MelonLogger.Msg(json);
                using (var writer = new StreamWriter(path))
                {
                    writer.Write(json);
                    writer.Flush();
                }
            }
            catch (Exception error)
            {
                SR2Console.SendError($"Saving error: {error}");
            }

        }

        public string DebugPrint() => JsonConvert.SerializeObject(this, Formatting.Indented);

        public void TrySave()
        {
            //var json = JsonConvert.SerializeObject(this, Formatting.Indented);
            //File.WriteAllText(currPath, json);
            DebugSaveToNewFile(currPath);
        }
        public static SR2ESavableData LoadFromStream(Stream stream)
        {
            using (var localStream = stream)
            {
                var reader = new StreamReader(localStream);
                var json = reader.ReadToEnd();
                SR2ESavableData save = JsonConvert.DeserializeObject<SR2ESavableData>(json);
                return save;
            }
        }
    }
}
