using System.Collections;
using System.Collections.Generic;
using Il2CppTMPro;
using UnityEngine;
namespace SR2E.Library.SaveExplorer;
[RegisterTypeInIl2Cpp(false)]
public class SaveExplorerTabEntry : MonoBehaviour
{
    public TMP_Text label;
    public TMP_Text value;

    public void Awake()
    {
        label = transform.GetChild(0).GetComponent<TMP_Text>();
        value = transform.GetChild(1).GetComponent<TMP_Text>();
    }
}
