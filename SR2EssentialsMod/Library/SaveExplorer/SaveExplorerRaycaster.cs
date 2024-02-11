using UnityEngine.InputSystem;

namespace SR2E.Library.SaveExplorer
{
    [RegisterTypeInIl2Cpp(false)]
    public class SaveExplorerRaycaster : MonoBehaviour
    {
        public void Update()
        {
            return;
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                var ray = Camera.main.ScreenPointToRay(new Vector2(Mouse.current.position.x.magnitude, Mouse.current.position.y.magnitude));
                var hit = Physics2D.Raycast(ray.origin, ray.direction);
                if (hit)
                {
                    if (hit.collider.GetComponent<SaveExplorerEntryInteract>())
                    {
                        hit.collider.GetComponent<SaveExplorerEntryInteract>().OnMouseDown();
                    }
                }
            }
        }
    }
}
