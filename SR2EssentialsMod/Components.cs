using System;
using Il2CppKinematicCharacterController;
using Il2CppMonomiPark.SlimeRancher.Player.CharacterController;
using SR2E.Library.Buttons;
using SR2E.Patches;
using UnityEngine.InputSystem;


namespace SR2E;

[RegisterTypeInIl2Cpp]
public class ObjectBlocker : MonoBehaviour
{
    public void Start()
    {
        Destroy(gameObject);
    }
}

[RegisterTypeInIl2Cpp]
public class FlingMode : MonoBehaviour
{
    public void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            SR2Console.ExecuteByString("fling 100");
        }
        else if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            SR2Console.ExecuteByString("fling -100");
        }
    }
}

[RegisterTypeInIl2Cpp]
public class CustomMainMenuButtonPressHandler : MonoBehaviour
{
    public void OnEnable()
    {
        foreach (CustomMainMenuButton button in SR2MainMenuButtonPatch.buttons)
            if (button.label.GetLocalizedString()+"ButtonStarter(Clone)" == gameObject.name)
            {
                button.action.Invoke();
                break;
            }
        Destroy(gameObject);
    }
}
/// <summary>
/// For use with camera
/// 
/// Currently bugged...
/// </summary>
[RegisterTypeInIl2Cpp]
public class IdentifiableObjectDragger : MonoBehaviour
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
        if (Keyboard.current.qKey.isPressed)
        {
            distanceFromCamera -= Time.deltaTime * distanceChangeSpeed;
        }
        if (Keyboard.current.eKey.isPressed)
        {
            distanceFromCamera += Time.deltaTime * distanceChangeSpeed;
        }


        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (Physics.Raycast(new Ray(Camera.main.transform.position, Camera.main.transform.forward), out var hit))
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
            if (Physics.Raycast(new Ray(mousePos, Camera.main.transform.forward), out var hit))
            {
                draggedObject.transform.position = hit.point;
            }
        }
    }
}
[RegisterTypeInIl2Cpp]
public class NoclipComponent : MonoBehaviour
{
    public float baseSpeed = 15f;
    public static float speedAdjust => SR2EEntryPoint.noclipAdjustSpeed;
    public float speed
    {
        get
        {
            if (isSprint)
                return baseSpeed * SR2EEntryPoint.noclipSpeedMultiplier;
            return baseSpeed;
        }
    }
    bool isSprint => Keyboard.current.shiftKey.isPressed;
    public static Transform player;
    public static SRCharacterController playerController;
    public static KinematicCharacterMotor playerMotor;
    public static KCCSettings playerSettings;

    public void OnDestroy()
    {
        try
        {
            playerController.Velocity = Vector3.zero;
            playerMotor.enabled = true;
            playerSettings.AutoSimulation = true;
            playerController.Position = player.position;
            playerMotor.Capsule.enabled = true;
            playerMotor.SetCapsuleCollisionsActivation(true);
        }
        catch
        {
            // ignore error
        }
    }

    public void Awake()
    {
        playerMotor.enabled = false;
        playerSettings.AutoSimulation = false;
        playerMotor.SetCapsuleCollisionsActivation(false);
        playerMotor.Capsule.enabled = false;
    }

    public void Update()
    {
        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
        {
            player.position += -transform.right * (speed * Time.deltaTime);
        }

        if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
        {
            player.position += transform.right * (speed * Time.deltaTime);
        }

        if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed)
        {
            player.position += transform.forward * (speed * Time.deltaTime);
        }

        if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed)
        {
            player.position += -transform.forward * (speed * Time.deltaTime);
        }

        if (Mouse.current.scroll.ReadValue().y > 0)
        {
            baseSpeed += (speedAdjust * Time.deltaTime);
        }
        if (Mouse.current.scroll.ReadValue().y < 0)
        {
            baseSpeed -= (speedAdjust * Time.deltaTime);
        }
        if (baseSpeed < 1)
        {
            baseSpeed = 1.01f;
        }
    }
}
