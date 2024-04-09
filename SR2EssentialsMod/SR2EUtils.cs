using System;
using Il2CppSystem.IO;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppInterop.Runtime.InteropTypes;
using System.Linq;
using System.Reflection;
using Il2CppMonomiPark.SlimeRancher.Damage;
using Il2CppMonomiPark.SlimeRancher.DataModel;
using Il2CppMonomiPark.SlimeRancher.Persist;
using Il2CppMonomiPark.SlimeRancher.Script.Util;
using Il2CppMonomiPark.SlimeRancher.UI;
using UnityEngine.Localization;
using UnityEngine.Localization.Tables;
using Il2CppMonomiPark.SlimeRancher.Weather;
using Il2CppMonomiPark.SlimeRancher.World;
using SR2E.Patches;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.Playables;
using SR2E.Library.Storage;

namespace SR2E.Library
{
    public static class SR2EUtils
    {
        public static WeatherStateDefinition[] weatherStateDefinitions => Resources.FindObjectsOfTypeAll<WeatherStateDefinition>();

        internal static GameObject rootOBJ;

        public static GameObject? player;

        // public enum VanillaPediaEntryCategories { TUTORIAL, SLIMES, RESOURCES, WORLD, RANCH, SCIENCE, WEATHER }
        public static SystemContext systemContext => SystemContext.Instance;
        public static GameContext gameContext => GameContext.Instance;
        public static SceneContext sceneContext => SceneContext.Instance;

        public static WeatherStateDefinition getWeatherStateByName(string name)
        {
            foreach (WeatherStateDefinition state in weatherStateDefinitions)
                try
                {
                    if (state.name.ToUpper().Replace(" ", "") == name.ToUpper())
                        return state;
                }
                catch (System.Exception ignored)
                { }
            return null;
        }

        public static Il2CppArrayBase<WeatherStateDefinition> weatherStates => GameContext.Instance.AutoSaveDirector.weatherStates.items.ToArray();
        public static WeatherStateDefinition WeatherState(string name) => weatherStates.FirstOrDefault((WeatherStateDefinition x) => x.name == name);


        public static Dictionary<string, Dictionary<string, string>> addedTranslations = new Dictionary<string, System.Collections.Generic.Dictionary<string, string>>();

        public static LocalizedString AddTranslation(string localized, string key = "l.SR2ELibraryTest", string table = "Actor")
        {
            StringTable table2 = LocalizationUtil.GetTable(table);

            System.Collections.Generic.Dictionary<string, string> dictionary;
            if (!addedTranslations.TryGetValue(table, out dictionary))
            {
                dictionary = new System.Collections.Generic.Dictionary<string, string>();

                addedTranslations.Add(table, dictionary);
            }

            string? key0 = null;

            if (key == "l.SR2E.LibraryTest")
            {
                key0 = $"{key}.{UnityEngine.Random.RandomRange(10000, 99999)}.{UnityEngine.Random.RandomRange(10, 99)}";
                while (table2.GetEntry(key0) != null)
                {
                    key0 = $"{key}.{UnityEngine.Random.RandomRange(10000, 99999)}.{UnityEngine.Random.RandomRange(10, 99)}";
                }

            }
            else
                key0 = key;

            dictionary.Add(key0, localized);
            StringTableEntry stringTableEntry = table2.AddEntry(key, localized);
            return new LocalizedString(table2.SharedData.TableCollectionName, stringTableEntry.SharedEntry.Id);
        }
        
        public static GameObject SpawnActor(this GameObject obj, Vector3 pos) => SpawnActor(obj, pos, Quaternion.identity);
        public static GameObject SpawnActor(this GameObject obj, Vector3 pos, Vector3 rot)=> SpawnActor(obj, pos, Quaternion.Euler(rot));
        public static GameObject SpawnActor(this GameObject obj, Vector3 pos, Quaternion rot)
        {
            return InstantiationHelpers.InstantiateActor(obj,
                SRSingleton<SceneContext>.Instance.RegionRegistry.CurrentSceneGroup, pos, rot,
                false, SlimeAppearance.AppearanceSaveSet.NONE, SlimeAppearance.AppearanceSaveSet.NONE);
        }
        public static GameObject SpawnDynamic(this GameObject obj, Vector3 pos, Quaternion rot)
        {
            return InstantiationHelpers.InstantiateDynamic(obj, pos, rot);
        }
        
        public static GameObject SpawnFX(this GameObject fx, Vector3 pos) => SpawnFX(fx, pos, Quaternion.identity);
        
        public static GameObject SpawnFX(this GameObject fx, Vector3 pos, Quaternion rot)
        {
            return FXHelpers.SpawnFX(fx, pos, rot);
        }
        public static T? Get<T>(string name) where T : Object => Resources.FindObjectsOfTypeAll<T>().FirstOrDefault((T x) => x.name == name);
  
        public static Sprite ConvertToSprite(this Texture2D texture)
        {
            return Sprite.Create(texture, new Rect(0f, 0f, (float)texture.width, (float)texture.height), new Vector2(0.5f, 0.5f), 1f);
        }
        public static GameObject CopyObject(this GameObject obj) => Object.Instantiate(obj, rootOBJ.transform);


        public static GameObject? Get(string name) => Get<GameObject>(name);

        public static void MakePrefab(this GameObject obj)
        {
            UnityEngine.Object.DontDestroyOnLoad(obj);
            obj.transform.parent = rootOBJ.transform;
        }
        

        internal static Sprite LoadSprite(string fileName) => ConvertToSprite(LoadImage(fileName));

        internal static Texture2D LoadImage(string filename)
        {
            Assembly executingAssembly = Assembly.GetExecutingAssembly();
            System.IO.Stream manifestResourceStream = executingAssembly.GetManifestResourceStream(executingAssembly.GetName().Name + "." + filename + ".png");
            byte[] array = new byte[manifestResourceStream.Length];
            manifestResourceStream.Read(array, 0, array.Length);
            Texture2D texture2D = new Texture2D(1, 1);
            ImageConversion.LoadImage(texture2D, array);
            texture2D.filterMode = FilterMode.Bilinear;
            return texture2D;
        }


        internal static Dictionary<MelonPreferences_Entry, System.Action> entriesWithoutWarning = new Dictionary<MelonPreferences_Entry, Action>();
        public static void disableWarning(this MelonPreferences_Entry entry) => entriesWithoutWarning.Add(entry, null);
        public static void disableWarning(this MelonPreferences_Entry entry, System.Action action) => entriesWithoutWarning.Add(entry, action);

        public static bool RemoveComponent<T>(this GameObject obj) where T : Component
        {
            try
            {
                T comp = obj.GetComponent<T>();
                var check = comp.gameObject;
                Object.Destroy(comp);
                return true;
            }
            catch { return false; }
        }
        public static bool RemoveComponent<T>(this Transform obj) where T : Component
        {
            try
            {
                T comp = obj.GetComponent<T>();
                var check = comp.gameObject;
                Object.Destroy(comp);
                return true;
            }
            catch { return false; }
        }

        internal static KeyCode KeyToKeyCode(Key key)
        {
            switch (key)
            {
                case Key.Space:
                    return KeyCode.Space;
                case Key.None:
                    return KeyCode.None;
                case Key.Enter:
                    return KeyCode.Return;
                case Key.Tab:
                    return KeyCode.Tab;
                case Key.Backquote:
                    return KeyCode.BackQuote;
                case Key.Quote:
                    return KeyCode.Quote;
                case Key.Semicolon:
                    return KeyCode.Semicolon;
                case Key.Comma:
                    return KeyCode.Comma;
                case Key.Period:
                    return KeyCode.Period;
                case Key.Slash:
                    return KeyCode.Slash;
                case Key.Backslash:
                    return KeyCode.Backslash;
                case Key.LeftBracket:
                    return KeyCode.LeftBracket;
                case Key.RightBracket:
                    return KeyCode.RightBracket;
                case Key.Minus:
                    return KeyCode.Minus;
                case Key.Equals:
                    return KeyCode.Equals;
                case Key.A:
                    return KeyCode.A;
                case Key.B:
                    return KeyCode.B;
                case Key.C:
                    return KeyCode.C;
                case Key.D:
                    return KeyCode.D;
                case Key.E:
                    return KeyCode.E;
                case Key.F:
                    return KeyCode.F;
                case Key.G:
                    return KeyCode.G;
                case Key.H:
                    return KeyCode.H;
                case Key.I:
                    return KeyCode.I;
                case Key.J:
                    return KeyCode.J;
                case Key.K:
                    return KeyCode.K;
                case Key.L:
                    return KeyCode.L;
                case Key.M:
                    return KeyCode.M;
                case Key.N:
                    return KeyCode.N;
                case Key.O:
                    return KeyCode.O;
                case Key.P:
                    return KeyCode.P;
                case Key.Q:
                    return KeyCode.Q;
                case Key.R:
                    return KeyCode.R;
                case Key.S:
                    return KeyCode.S;
                case Key.T:
                    return KeyCode.T;
                case Key.U:
                    return KeyCode.U;
                case Key.V:
                    return KeyCode.V;
                case Key.W:
                    return KeyCode.W;
                case Key.X:
                    return KeyCode.X;
                case Key.Y:
                    return KeyCode.Y;
                case Key.Z:
                    return KeyCode.Z;
                case Key.Digit1:
                    return KeyCode.Alpha1;
                case Key.Digit2:
                    return KeyCode.Alpha2;
                case Key.Digit3:
                    return KeyCode.Alpha3;
                case Key.Digit4:
                    return KeyCode.Alpha4;
                case Key.Digit5:
                    return KeyCode.Alpha5;
                case Key.Digit6:
                    return KeyCode.Alpha6;
                case Key.Digit7:
                    return KeyCode.Alpha7;
                case Key.Digit8:
                    return KeyCode.Alpha8;
                case Key.Digit9:
                    return KeyCode.Alpha9;
                case Key.Digit0:
                    return KeyCode.Alpha0;
                case Key.LeftShift:
                    return KeyCode.LeftShift;
                case Key.RightShift:
                    return KeyCode.RightShift;
                case Key.LeftAlt:
                    return KeyCode.LeftAlt;
                case Key.RightAlt:
                    return KeyCode.RightAlt;
                case Key.LeftCtrl:
                    return KeyCode.LeftControl;
                case Key.RightCtrl:
                    return KeyCode.RightControl;
                case Key.LeftWindows:
                    return KeyCode.LeftWindows;
                case Key.RightWindows:
                    return KeyCode.RightWindows;
                //Disabled to allow exiting key choose
                //case Key.Escape:
                //    return KeyCode.Escape;
                case Key.LeftArrow:
                    return KeyCode.LeftArrow;
                case Key.RightArrow:
                    return KeyCode.RightArrow;
                case Key.UpArrow:
                    return KeyCode.UpArrow;
                case Key.DownArrow:
                    return KeyCode.DownArrow;
                case Key.Backspace:
                    return KeyCode.Backspace;
                case Key.PageDown:
                    return KeyCode.PageDown;
                case Key.PageUp:
                    return KeyCode.PageUp;
                case Key.Home:
                    return KeyCode.Home;
                case Key.End:
                    return KeyCode.End;
                case Key.Insert:
                    return KeyCode.Insert;
                case Key.Delete:
                    return KeyCode.Delete;
                case Key.CapsLock:
                    return KeyCode.CapsLock;
                case Key.NumLock:
                    return KeyCode.Numlock;
                case Key.PrintScreen:
                    return KeyCode.Print;
                case Key.Pause:
                    return KeyCode.Pause;
                case Key.NumpadEnter:
                    return KeyCode.KeypadEnter;
                case Key.NumpadDivide:
                    return KeyCode.KeypadDivide;
                case Key.NumpadMultiply:
                    return KeyCode.KeypadMultiply;
                case Key.NumpadPlus:
                    return KeyCode.KeypadPlus;
                case Key.NumpadMinus:
                    return KeyCode.KeypadMinus;
                case Key.NumpadPeriod:
                    return KeyCode.KeypadPeriod;
                case Key.NumpadEquals:
                    return KeyCode.KeypadEquals;
                case Key.Numpad0:
                    return KeyCode.Keypad0;
                case Key.Numpad1:
                    return KeyCode.Keypad1;
                case Key.Numpad2:
                    return KeyCode.Keypad2;
                case Key.Numpad3:
                    return KeyCode.Keypad3;
                case Key.Numpad4:
                    return KeyCode.Keypad4;
                case Key.Numpad5:
                    return KeyCode.Keypad5;
                case Key.Numpad6:
                    return KeyCode.Keypad6;
                case Key.Numpad7:
                    return KeyCode.Keypad7;
                case Key.Numpad8:
                    return KeyCode.Keypad8;
                case Key.Numpad9:
                    return KeyCode.Keypad9;
                case Key.F1:
                    return KeyCode.F1;
                case Key.F2:
                    return KeyCode.F2;
                case Key.F3:
                    return KeyCode.F3;
                case Key.F4:
                    return KeyCode.F4;
                case Key.F5:
                    return KeyCode.F5;
                case Key.F6:
                    return KeyCode.F6;
                case Key.F7:
                    return KeyCode.F7;
                case Key.F8:
                    return KeyCode.F8;
                case Key.F9:
                    return KeyCode.F9;
                case Key.F10:
                    return KeyCode.F10;
                case Key.F11:
                    return KeyCode.F11;
                case Key.F12:
                    return KeyCode.F12;
                default:
                    return KeyCode.None;
            }
        }
        public static Il2CppSystem.Type il2cppTypeof(this Type type)
        {
            string typeName = type.AssemblyQualifiedName;

            if (typeName.ToLower().StartsWith("il2cpp"))
            {
                typeName = typeName.Substring("il2cpp".Length);
            }

            Il2CppSystem.Type il2cppType = Il2CppSystem.Type.GetType(typeName);

            return il2cppType;
        }

        public static T getObjRec<T>(this GameObject obj, string name) where T : class
        {
            var transform = obj.transform;

            List<GameObject> totalChildren = getAllChildren(transform);
            for (int i = 0; i < totalChildren.Count; i++)
                if (totalChildren[i].name == name)
                {
                    if (typeof(T) == typeof(GameObject))
                        return totalChildren[i] as T;
                    if (typeof(T) == typeof(Transform))
                        return totalChildren[i].transform as T;
                    if (totalChildren[i].GetComponent<T>() != null)
                        return totalChildren[i].GetComponent<T>();
                }
            return null;
        }

        public static List<GameObject> getAllChildren(this GameObject obj)
        {
            var container = obj.transform;
            List<GameObject> allChildren = new List<GameObject>();
            for (int i = 0; i < container.childCount; i++)
            {
                var child = container.GetChild(i);
                allChildren.Add(child.gameObject);
                allChildren.AddRange(getAllChildren(child));
            }
            return allChildren;
        }

        public static T[] getAllChildrenOfType<T>(this GameObject obj) where T : Component
        {
            List<T> children = new List<T>();
            foreach (var child in obj.getAllChildren())
            {
                if (child.GetComponent<T>() != null)
                {
                    children.Add(child.GetComponent<T>());
                }
            }
            return children.ToArray();
        }

        public static T[] getAllChildrenOfType<T>(this Transform obj) where T : Component
        {
            List<T> children = new List<T>();
            foreach (var child in obj.getAllChildren())
            {
                if (child.GetComponent<T>() != null)
                {
                    children.Add(child.GetComponent<T>());
                }
            }
            return children.ToArray();
        }

        public static T getObjRec<T>(this Transform transform, string name) where T : class
        {
            List<GameObject> totalChildren = getAllChildren(transform);
            for (int i = 0; i < totalChildren.Count; i++)
                if (totalChildren[i].name == name)
                {
                    if (typeof(T) == typeof(GameObject))
                        return totalChildren[i] as T;
                    if (typeof(T) == typeof(Transform))
                        return totalChildren[i].transform as T;
                    if (totalChildren[i].GetComponent<T>() != null)
                        return totalChildren[i].GetComponent<T>();
                }
            return null;
        }

        public static List<GameObject> getAllChildren(this Transform container)
        {
            List<GameObject> allChildren = new List<GameObject>();
            for (int i = 0; i < container.childCount; i++)
            {
                var child = container.GetChild(i);
                allChildren.Add(child.gameObject);
                allChildren.AddRange(getAllChildren(child));
            }
            return allChildren;
        }
        public static bool inGame
        {
            get
            {
                try
                {
                    if (SceneContext.Instance == null) return false;
                    if (SceneContext.Instance.PlayerState == null) return false;
                }
                catch
                { return false; }
                return true;
            }
        }


        public static TripleDictionary<GameObject, ParticleSystemRenderer, string> FXLibrary = new TripleDictionary<GameObject, ParticleSystemRenderer, string>();
        public static TripleDictionary<string, ParticleSystemRenderer, GameObject> FXLibraryReversable = new TripleDictionary<string, ParticleSystemRenderer, GameObject>();

    }
}
