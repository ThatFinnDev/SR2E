using System;
using System.Linq;
using Il2CppSystem.Reflection;
using Il2CppTMPro;
using SR2E.Library.SaveExplorer;
using UnityEngine.UI;

namespace SR2E.SaveEditor;
[RegisterTypeInIl2Cpp(false)]
public class SR2ESaveEditor : MonoBehaviour
{
    public static SR2ESaveEditor instance;
    public Transform CEntryContent;
    public Transform CategoryContent;
    
    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
        gameObject.SetActive(false);
        CEntryContent = transform.getObjRec<Transform>("CEntryContent");
        CategoryContent = transform.getObjRec<Transform>("CategoryContent");
    }

    void Update()
    {
    }

    public static void Open()
    {
        instance.gameObject.SetActive(true);
        List<FieldInfo> selectedProperties = GameContext.Instance.AutoSaveDirector.SavedGame.gameState.GetIl2CppType().GetFields(Il2CppSystem.Reflection.BindingFlags.Public | Il2CppSystem.Reflection.BindingFlags.NonPublic | Il2CppSystem.Reflection.BindingFlags.Instance | BindingFlags.GetField | BindingFlags.SetField).ToList();
        GameObject categoryPrefab = instance.CategoryContent.GetChild(0).gameObject;
        foreach (var property in selectedProperties)
        {
            GameObject categoryInstance = Instantiate(categoryPrefab, instance.CategoryContent);
            categoryInstance.SetActive(true);
            categoryInstance.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = property.Name;
            categoryPrefab.GetComponent<Button>().onClick.AddListener((Action)(() =>
            {
                List<FieldInfo> typeProperties = property.FieldType.GetFields(Il2CppSystem.Reflection.BindingFlags.Public | Il2CppSystem.Reflection.BindingFlags.NonPublic | Il2CppSystem.Reflection.BindingFlags.Instance | BindingFlags.GetField | BindingFlags.SetField).ToList();
                GameObject entryPrefab = instance.CEntryContent.GetChild(0).gameObject;
                foreach (var typeProperty in typeProperties)
                {
                    GameObject entryInstance = Instantiate(entryPrefab, instance.CEntryContent);
                    entryInstance.SetActive(true);
                    entryInstance.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = typeProperty.Name;
                }
            }));
        }
    }
}