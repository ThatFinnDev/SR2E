using SR2E.Managers;
using UnityEngine.InputSystem;

namespace SR2E.Components;

/// <summary>
/// For use with camera
/// 
/// Currently bugged...
/// </summary>
[RegisterTypeInIl2Cpp(false)]
internal class IdentifiableObjectDragger : MonoBehaviour
{
    public GameObject draggedObject;
    public bool isDragging;
    public float distanceFromCamera = 2f;
    private float distanceChangeSpeed = 1f;
    public Vector3 mousePos
    {
        get
        {
            Vector2 mouseScreenPosition = Mouse.current.position.ReadValue();
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(new Vector3(mouseScreenPosition.x, mouseScreenPosition.y, Camera.main.nearClipPlane));
            return mouseWorldPosition;
        }
    }
    public void Update()
    {
        if (Key.Q.OnKey())
        {
            distanceFromCamera -= Time.deltaTime * distanceChangeSpeed;
        }
        if (Key.E.OnKey())
        {
            distanceFromCamera += Time.deltaTime * distanceChangeSpeed;
        }


        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (Physics.Raycast(new Ray(Camera.main.transform.position, Camera.main.transform.forward), out var hit,Mathf.Infinity,MiscEUtil.defaultMask))
            {
                if (hit.transform.GetComponent<Rigidbody>())
                {
                    isDragging = true;
                    draggedObject = hit.transform.gameObject;
                    draggedObject.GetComponent<Collider>().isTrigger = true;
                }
            }
        }

        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            draggedObject.GetComponent<Rigidbody>().velocity = Vector3.up * 2f;
            isDragging = false;
            draggedObject.GetComponent<Collider>().isTrigger = false;
            draggedObject = null;
        }

        if (isDragging && draggedObject)
        {
            draggedObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            if (Physics.Raycast(new Ray(mousePos, Camera.main.transform.forward), out var hit,Mathf.Infinity,MiscEUtil.defaultMask))
            {
                draggedObject.transform.position = hit.point;
            }
        }
    }
}