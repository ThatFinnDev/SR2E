using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SR2E.Library.SaveExplorer
{
    [RegisterTypeInIl2Cpp(false)]
    public class SaveExplorerRoot : MonoBehaviour
    {
        public GameObject saveRootExplorer;
        public GameObject saveRootEntry;
        public GameObject inspector;
        public GameObject inspectorEntry;

        internal List<SaveExplorerTabEntry> rootTabs;
        internal List<SaveExplorerTabEntry> inspectorTabs;

        public void Awake()
        {
            saveRootExplorer = transform.GetChild(0).GetChild(0).gameObject;
            saveRootEntry = transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).gameObject;
            inspector = transform.GetChild(0).GetChild(1).gameObject;
            inspectorEntry = transform.GetChild(0).GetChild(1).GetChild(0).GetChild(0).gameObject;

            inspectorEntry.AddComponent<SaveExplorerTabEntry>();
            inspectorEntry.transform.GetChild(2).gameObject.AddComponent<InspectorEntryInteract>();

            saveRootEntry.AddComponent<SaveExplorerTabEntry>();
            saveRootEntry.transform.GetChild(2).gameObject.AddComponent<SaveExplorerEntryInteract>();

            gameObject.AddComponent<PropertyStealer>();
            gameObject.AddComponent<SaveExplorerRaycaster>();

            saveRootEntry.SetActive(false);
            inspectorEntry.SetActive(false);

            rootTabs = new List<SaveExplorerTabEntry>();
            inspectorTabs = new List<SaveExplorerTabEntry>();
        }

        public void Update()
        {
            return;
            if (Keyboard.current.f10Key.wasPressedThisFrame)
            {
                transform.GetChild(0).gameObject.SetActive(!transform.GetChild(0).gameObject.active);
            }
            if (Keyboard.current.f9Key.wasPressedThisFrame)
            {
                GetComponent<PropertyStealer>().Refresh();
            }
        }
    }
}