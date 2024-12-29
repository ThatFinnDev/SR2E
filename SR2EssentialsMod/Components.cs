using System;
using Il2CppKinematicCharacterController;
using Il2CppMonomiPark.SlimeRancher.Player.CharacterController;
using Il2CppMonomiPark.SlimeRancher.UI;
using Il2CppTMPro;
using SR2E.Buttons;
using SR2E.Commands;
using SR2E.Patches.MainMenu;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace SR2E;

[RegisterTypeInIl2Cpp(false)]
internal class ObjectBlocker : MonoBehaviour
{
    public void Start()
    {
        Destroy(gameObject);
    }
}

[RegisterTypeInIl2Cpp(false)]
internal class FlingMode : MonoBehaviour
{
    public void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            SR2EConsole.ExecuteByString("fling 100");
        }
        else if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            SR2EConsole.ExecuteByString("fling -100");
        }
    }
}

[RegisterTypeInIl2Cpp(false)]
internal class CustomMainMenuButtonPressHandler : MonoBehaviour
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
[RegisterTypeInIl2Cpp(false)]
internal class CheatMenuRefineryEntry : MonoBehaviour
{
    public IdentifiableType item;
    public Image icon;
    private bool dontChange = false;
    public Slider amountSlider;
    public TextMeshProUGUI handleText;
    public TextMeshProUGUI itemName;
    private bool didStartRan = false;
    private void Start()
    {
        didStartRan = true;
        amountSlider = gameObject.getObjRec<Slider>("Slider");
        amountSlider.maxValue = GadgetDirector.REFINERY_MAX;
        handleText = amountSlider.gameObject.getObjRec<TextMeshProUGUI>("Text");
        itemName = gameObject.getObjRec<TextMeshProUGUI>("Name");
        icon = gameObject.getObjRec<Image>("Icon");
        icon.sprite = item.icon;
        itemName.text = item.getName().Replace("'", "");
        amountSlider.onValueChanged.AddListener((Action<float>)((valueFloat) =>
        {
            if (dontChange)
            {
                dontChange = false; return;
            }

            int newValue = int.Parse(valueFloat.ToString().Split(".")[0]);
            int oldValue = SceneContext.Instance.GadgetDirector.GetItemCount(item);
            int difference = newValue - oldValue;
            if (difference == 0) return;
            if (difference > 0)
                SceneContext.Instance.GadgetDirector.AddItem(item,difference);
            if (difference < 0)
            {
                IdentCostEntry costEntry = new IdentCostEntry();
                costEntry.amount = -difference;
                costEntry.identType = item;
                Il2CppSystem.Collections.Generic.List<IdentCostEntry> entries =
                    new Il2CppSystem.Collections.Generic.List<IdentCostEntry>();
                entries.Add(costEntry);
                SceneContext.Instance.GadgetDirector.TryToSpendItems(entries);
            }
            handleText.SetText(newValue.ToString());
        }));}

    public void OnOpen()
    {
        if(!didStartRan) Start();

        dontChange = true;
        amountSlider.value = SceneContext.Instance.GadgetDirector.GetItemCount(item);
        handleText.SetText(SceneContext.Instance.GadgetDirector.GetItemCount(item).ToString());
    }
}


[RegisterTypeInIl2Cpp(false)]
internal class CheatMenuGadgetEntry : MonoBehaviour
{
    public IdentifiableType item;
    public Image icon;
    private bool dontChange = false;
    public Slider amountSlider;
    public TextMeshProUGUI handleText;
    public TextMeshProUGUI itemName;
    private bool didStartRan = false;
    private void Start()
    {
        didStartRan = true;
        amountSlider = gameObject.getObjRec<Slider>("Slider");
        amountSlider.maxValue = GadgetDirector.REFINERY_MAX;
        handleText = amountSlider.gameObject.getObjRec<TextMeshProUGUI>("Text");
        itemName = gameObject.getObjRec<TextMeshProUGUI>("Name");
        icon = gameObject.getObjRec<Image>("Icon");
        icon.sprite = item.icon;
        
        itemName.text = item.getName().Replace("'", "");
        amountSlider.onValueChanged.AddListener((Action<float>)((valueFloat) =>
        {
            if (dontChange)
            {
                dontChange = false; return;
            }

            int newValue = int.Parse(valueFloat.ToString().Split(".")[0]);
            if (newValue > 0 && !SceneContext.Instance.GadgetDirector.HasBlueprint(item.Cast<GadgetDefinition>()))
                SceneContext.Instance.GadgetDirector.AddBlueprint(item.Cast<GadgetDefinition>());
            int oldValue = SceneContext.Instance.GadgetDirector.GetItemCount(item);
            int difference = newValue - oldValue;
            if (difference == 0) return;
            if (difference > 0)
                SceneContext.Instance.GadgetDirector.AddItem(item,difference);
            if (difference < 0)
            {
                IdentCostEntry costEntry = new IdentCostEntry();
                costEntry.amount = -difference;
                costEntry.identType = item;
                Il2CppSystem.Collections.Generic.List<IdentCostEntry> entries =
                    new Il2CppSystem.Collections.Generic.List<IdentCostEntry>();
                entries.Add(costEntry);
                SceneContext.Instance.GadgetDirector.TryToSpendItems(entries);
            }
            handleText.SetText(newValue.ToString());
        }));}

    public void OnOpen()
    {
        if(!didStartRan) Start();

        dontChange = true;
        amountSlider.value = SceneContext.Instance.GadgetDirector.GetItemCount(item);
        handleText.SetText(SceneContext.Instance.GadgetDirector.GetItemCount(item).ToString());
    }
}


[RegisterTypeInIl2Cpp(false)]
internal class CheatMenuSlot : MonoBehaviour
{
    public int slotID;
    public Button applyButton;
    public Slider amountSlider;
    public TextMeshProUGUI handleText;
    public TMP_InputField entryInput;
    private bool didStartRan = false;
    private void Start()
    {
        didStartRan = true;
        slotID = int.Parse(gameObject.getObjRec<TextMeshProUGUI>("Text").text.Replace(" ", "").Replace(":", "").Replace("Slot", ""))-1;
        applyButton = gameObject.getObjRec<Button>("Apply");
        amountSlider = gameObject.getObjRec<Slider>("Slider");
        handleText = amountSlider.gameObject.getObjRec<TextMeshProUGUI>("Text");
        entryInput = gameObject.getObjRec<TMP_InputField>("EntryInput");
        applyButton.onClick.AddListener((Action)(() =>{Apply();}));
        amountSlider.onValueChanged.AddListener((Action<float>)((value) => { handleText.SetText(value.ToString().Split(".")[0]); }));
    }

    public void Apply()
    {
        if (amountSlider.value == 0) { entryInput.text = ""; slot.Clear(); return; }
        
        IdentifiableType type = getIdentByName(entryInput.text);
        if (type == null) { entryInput.text = ""; slot.Clear(); amountSlider.value = 0; return; }
        
        string itemName = type.getName().Replace("'","").Replace(" ","");
        entryInput.text = itemName;
        slot.Clear();
        SceneContext.Instance.PlayerState.Ammo.MaybeAddToSpecificSlot(type, null, slotID, 
            int.Parse(amountSlider.value.ToString().Split(".")[0]));
    }
    private Ammo.Slot slot {
        get
        {
            try { return SceneContext.Instance.PlayerState.Ammo.Slots[slotID]; }
            catch { return null; }
        }
    }
    public void OnOpen()
    {
        if(!didStartRan) Start();
        
        if (slot == null) return;

        amountSlider.maxValue = slot.MaxCount;
        amountSlider.value = slot.Count;
        string identName = "";
        if (slot.Id != null) identName = slot.Id.getName().Replace("'","").Replace(" ","");
        
        entryInput.text = identName;
    }
}
[RegisterTypeInIl2Cpp(false)]
internal class CheatMenuNewbucks : MonoBehaviour
{
    public Slider amountSlider;
    public TextMeshProUGUI handleText;
    private bool didStartRan = false;
    private bool dontChange = false;
    private void Start()
    {
        didStartRan = true;
        amountSlider = gameObject.getObjRec<Slider>("Slider");
        handleText = amountSlider.gameObject.getObjRec<TextMeshProUGUI>("Text");
        amountSlider.onValueChanged.AddListener((Action<float>)((value) =>
        {
            if (dontChange)
            {
                dontChange = false; return;
            }

            double newValue = Math.Pow(value, 4.5);
            if (newValue < 0) newValue = 0;
            if (newValue > 9999999999) newValue = 9999999999;
            handleText.SetText(newValue.ToString().Split(".")[0]);
            SceneContext.Instance.PlayerState._model.currency = int.Parse(newValue.ToString().Split(".")[0]);
        }));
    }

    
    public void OnEnable()
    {
        if(!didStartRan) Start();

        try
        {
            double newValue = Math.Pow(SceneContext.Instance.PlayerState._model.currency, (1.0 / 4.5));
            dontChange = true;
            amountSlider.value = int.Parse(newValue.ToString().Split(".")[0]);
            handleText.SetText(SceneContext.Instance.PlayerState._model.currency.ToString());
        }
        catch { }
    }
}

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
