using Il2CppKinematicCharacterController;
using Il2CppMonomiPark.SlimeRancher.Player.CharacterController;
using SR2E.Commands;
using SR2E.Managers;
using UnityEngine.InputSystem;

namespace SR2E.Components;

[RegisterTypeInIl2Cpp(false)]
internal class NoClipComponent : MonoBehaviour
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
    bool isSprint => Key.LeftShift.OnKey();
    public static Transform player;
    public static SRCharacterController playerController;
    public static KinematicCharacterMotor playerMotor;
    public static KCCSettings playerSettings;

    public void OnDestroy()
    {
        try
        {
            playerController.BaseVelocity = Vector3.zero;
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
        if(NoClipCommand.horizontal!=null)
        {
            float horizontal = NoClipCommand.horizontal.ReadValue<float>();
            float vertical = NoClipCommand.vertical.ReadValue<float>();
            if(horizontal>0.01f||horizontal<-0.01f)
                player.position += transform.right * (horizontal*speed * Time.deltaTime);
            if(vertical>0.01f||vertical<-0.01f)
                player.position += transform.forward * (vertical*speed * Time.deltaTime);
        }
        else
        {
            if (Key.A.OnKey() || Key.LeftArrow.OnKey())
                player.position += -transform.right * (speed * Time.deltaTime);
            if (Key.D.OnKey() || Key.RightArrow.OnKey())
                player.position += transform.right * (speed * Time.deltaTime);
            if (Key.W.OnKey() || Key.UpArrow.OnKey())
                player.position += transform.forward * (speed * Time.deltaTime);
            if (Key.S.OnKey() || Key.DownArrow.OnKey())
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
