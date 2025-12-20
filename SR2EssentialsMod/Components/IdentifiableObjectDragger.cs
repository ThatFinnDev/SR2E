using SR2E.Enums;
using SR2E.Managers;
using SR2E.Storage;
using UnityEngine.InputSystem;

namespace SR2E.Components;

/// <summary>
/// For use with camera
/// 
/// Currently bugged...
/// </summary>
[InjectClass]
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
            Vector3 mouseWorldPosition = MiscEUtil.GetActiveCamera().ScreenToWorldPoint(new Vector3(mouseScreenPosition.x, mouseScreenPosition.y, MiscEUtil.GetActiveCamera().nearClipPlane));
            return mouseWorldPosition;
        }
    }
    public void Update()
    {
        if (LKey.Q.OnKey())
        {
            distanceFromCamera -= Time.deltaTime * distanceChangeSpeed;
        }
        if (LKey.E.OnKey())
        {
            distanceFromCamera += Time.deltaTime * distanceChangeSpeed;
        }


        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (Physics.Raycast(new Ray(MiscEUtil.GetActiveCamera().transform.position, MiscEUtil.GetActiveCamera().transform.forward), out var hit,Mathf.Infinity,MiscEUtil.defaultMask))
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
            if (Physics.Raycast(new Ray(mousePos, MiscEUtil.GetActiveCamera().transform.forward), out var hit,Mathf.Infinity,MiscEUtil.defaultMask))
            {
                draggedObject.transform.position = hit.point;
            }
        }
    }
}