using Il2CppMonomiPark.SlimeRancher;
using Il2CppSystem.Diagnostics;
using Il2CppMonomiPark.SlimeRancher.Persist;
using Il2CppMonomiPark.SlimeRancher.Player;

namespace SR2E.Patches;
/*
[HarmonyPatch(typeof(GameModelPushHelpers), nameof(GameModelPushHelpers.PushGame))]
internal class TestPatch
{
    internal static void Prefix()
    {
      var stackTrace = new StackTrace(true); 
      MelonLogger.Msg("=== Call Stack ===");

      // Skip frame 0 (PrintCallStack itself)
      for (int i = 1; i < stackTrace.FrameCount; i++)
      {
          var frame = stackTrace.GetFrame(i);
          var method = frame.GetMethod();

          string declaringType = method.DeclaringType != null 
              ? method.DeclaringType.FullName
              : "(no type)";
            //Line numbers are broken because of il2cpp :(
          MelonLogger.Msg($"{i}: {declaringType}.{method.Name} " +
                          $"(Line {frame.GetFileLineNumber()})");
      }
    }
}*/
    
    
/*
   [20:46:49.686] === Call Stack ===
   [20:46:49.689] 1: MonomiPark.SlimeRancher.Persist.VersionedGamePersistedDataSet`1.LoadData (Line 0)
   [20:46:49.690] 2: MonomiPark.SlimeRancher.Persist.PersistedDataSet.Load (Line 0)
   [20:46:49.691] 3: MonomiPark.SlimeRancher.Persist.VersionedPersistedDataSet`1.Load (Line 0)
   [20:46:49.692] 4: MonomiPark.SlimeRancher.AutoSaveDirector.Load (Line 0)
   [20:46:49.693] 5: MonomiPark.SlimeRancher.AutoSaveDirector+<>c__DisplayClass49_0.<BeginLoad>b__0 (Line 0)
   [20:46:49.694] 6: MonomiPark.SlimeRancher.AutoSaveDirector+<LoadSave_Coroutine>d__50.MoveNext (Line 0)
   [20:46:49.695] 7: UnityEngine.SetupCoroutine.InvokeMoveNext (Line 0)
   [20:46:49.695] 8: MonomiPark.SlimeRancher.AutoSaveDirector.BeginLoad (Line 0)
   [20:46:49.696] 9: MonomiPark.SlimeRancher.UI.MainMenu.Model.LoadGameBehaviorModel.InvokeBehavior (Line 0)
   [20:46:49.697] 10: UnityEngine.EventSystems.ExecuteEvents.Execute (Line 0)
   [20:46:49.698] 11: MonomiPark.SlimeRancher.UI.Framework.SRExecuteEvents.ExecuteHierarchy (Line 0)
   [20:46:49.699] 12: UnityEngine.EventSystems.ExecuteEvents.Execute (Line 0)
   [20:46:49.700] 13: UnityEngine.InputSystem.UI.InputSystemUIInputModule.ProcessPointerButton (Line 0)
   [20:46:49.701] 14: UnityEngine.InputSystem.UI.InputSystemUIInputModule.ProcessPointer (Line 0)
   [20:46:49.701] 15: UnityEngine.InputSystem.UI.InputSystemUIInputModule.Process (Line 0)
   [20:46:49.702] 16: UnityEngine.EventSystems.EventSystem.Update (Line 0)
*/