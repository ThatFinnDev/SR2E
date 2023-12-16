using MelonLoader;
using SR2E;
using System.Collections;
using UnityEngine;

namespace VirtualSlime
{
    [RegisterTypeInIl2Cpp]
    public class DisableConsoleBehaviour : MonoBehaviour
    {
        public void OnCollisionEnter(Collision c)
        {
            if (c.gameObject == player)
            {
                MelonCoroutines.Start(Main());  
            }
        }
        IEnumerator Main()
        {
            GetConsoleObject().getObjRec<GameObject>("commandInput").SetActive(false);
            yield return new WaitForSecondsRealtime(12.5f);
            GetConsoleObject().getObjRec<GameObject>("commandInput").SetActive(true);
        }
    }

    
}