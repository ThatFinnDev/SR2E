using Il2CppSystem.Reflection;
using System.Linq;

namespace SR2E.Library.SaveExplorer
{
    [RegisterTypeInIl2Cpp(false)]
    public class PropertyStealer : MonoBehaviour
    {
        public static PropertyStealer Instance;
        public Il2CppSystem.Type selectedType;
        public Il2CppSystem.Object selected;
        public Il2CppSystem.Type defaultType;
        public SaveExplorerRoot root;
        public List<FieldInfo> defaultProperties;
        public List<FieldInfo> selectedProperties;
        public void Awake()
        {
            defaultType = GameContext.Instance.AutoSaveDirector.SavedGame.gameState.GetIl2CppType();
            root = GetComponent<SaveExplorerRoot>();
            Instance = this;
        }

        public void Refresh()
        {
            if (selected != null)
            {
                selectedType = selected.GetIl2CppType();
                selectedProperties = defaultType.GetFields(Il2CppSystem.Reflection.BindingFlags.Public | Il2CppSystem.Reflection.BindingFlags.NonPublic | Il2CppSystem.Reflection.BindingFlags.Instance | BindingFlags.GetField | BindingFlags.SetField).ToList();
                foreach (var tab in root.inspectorTabs)
                {
                    Destroy(tab.gameObject);
                }
                root.inspectorTabs.Clear();

                var idx2 = 0;
                foreach (var Field1 in selectedProperties)
                {
                    var obj2 = Instantiate(root.saveRootEntry).GetComponent<SaveExplorerTabEntry>();

                    obj2.Awake();
                    obj2.value.m_text = Stringify.ToString(Field1.GetValue(selected));
                    obj2.gameObject.SetActive(true);
                    obj2.label.m_text = Field1.Name;
                    obj2.transform.parent = root.inspector.transform.GetChild(0);
                    root.inspectorTabs.Add(obj2);
                    obj2.transform.GetChild(2).GetComponent<InspectorEntryInteract>().IDX = idx2;
                    idx2++;
                }
            }

            defaultProperties = defaultType.GetFields(Il2CppSystem.Reflection.BindingFlags.Public | Il2CppSystem.Reflection.BindingFlags.NonPublic | Il2CppSystem.Reflection.BindingFlags.Instance | BindingFlags.GetField | BindingFlags.SetField).ToList();
            foreach (var tab in root.rootTabs)
            {
                Destroy(tab.gameObject);
            }
            root.rootTabs.Clear();

            var idx = 0;
            foreach (var Field in defaultProperties)
            {
                var obj = Instantiate(root.saveRootEntry).GetComponent<SaveExplorerTabEntry>();

                obj.Awake();
                obj.value.m_text = "";
                obj.gameObject.SetActive(true);
                obj.label.m_text = Field.Name;
                obj.transform.parent = root.saveRootExplorer.transform.GetChild(0);
                root.rootTabs.Add(obj);
                obj.transform.localScale = new Vector3(0.8766f, 0.0462f, 1f);
                obj.transform.GetChild(2).GetComponent<SaveExplorerEntryInteract>().IDX = idx;
                idx++;
            }
        }   
    }
}
