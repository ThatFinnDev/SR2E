using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.InputSystem;

namespace SR2E.Library.SaveExplorer
{
    [RegisterTypeInIl2Cpp(false)]
    public class SaveExplorerRaycaster : MonoBehaviour
    {
        public void Update()
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                Ray ray = Camera.main.ScreenPointToRay(new Vector2(Mouse.current.position.x.magnitude, Mouse.current.position.y.magnitude));
                RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity);
                if (hit)
                {
                    if (hit.collider.GetComponent<SaveExplorerEntryInteract>())
                        hit.collider.GetComponent<SaveExplorerEntryInteract>().OnMouseDown();
                    if (hit.collider.GetComponent<InspectorEntryInteract>())
                        hit.collider.GetComponent<InspectorEntryInteract>().OnMouseDown();
                }
            }
        }
    }
}
