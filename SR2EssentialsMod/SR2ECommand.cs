using System;
using SR2E.Managers;

namespace SR2E;

/// <summary>
/// Abstract console command class
/// </summary>
public abstract class SR2ECommand
{
    public bool silent = false;
    /// <summary>
    /// The ID of this command (Always lowercase)
    /// </summary>
    public abstract string ID { get; }

    /// <summary>
    /// The usage info of this command
    /// </summary>
    public abstract string Usage { get; }

    /// <summary>
    /// The description of this command
    /// </summary>
    public virtual string Description => translation($"cmd.{ID.ToLower()}.description");

    /// <summary>
    /// The full description of this command
    /// </summary>
    public virtual string ExtendedDescription
    {
        get
        {
            string key = $"cmd.{ID.ToLower()}.extendeddescription";
            string translation = SR2ELanguageManger.translation(key);
            return key == translation ? Description : translation;
        }
    }

    /// <summary>
    /// The type of this command
    /// </summary>
    public virtual CommandType type { get; } = CommandType.None;


    public virtual bool Hidden { get; }


    /// <summary>
    /// Executes the command
    /// </summary>
    /// <param name="args">The arguments passed in the console (null if no arguments are provided)</param>
    /// <returns>True if it executed, false otherwise</returns>
    public abstract bool Execute(string[] args);

    /// <summary>
    /// Gets the auto complete list (word filter is done by the system)
    /// </summary>
    /// <param name="argIndex">The index of the argument in the command string</param>
    /// <param name="args">A list of inputted arguments</param>
    /// <returns>The list of auto complete options</returns>
    public virtual List<string> GetAutoComplete(int argIndex, string[] args)
    {
        return null;
    }

    /// <summary>
    /// Allows the execution of the Command when the respective menus are open
    /// use typeof(Menu) to add
    /// </summary>
    public virtual Type[] execWhileMenuOpen { get; } = Array.Empty<Type>();

    
    /// <summary>
    /// Gets called every frame
    /// </summary>
    public virtual void Update()
    { }
    /// <summary>
    /// Gets called when the scene GameCore loads
    /// </summary>
    
    /// <summary>
    /// Gets executed every time the scene "StandaloneEngagementPrompt" gets loaded.
    /// </summary>
    public virtual void OnStandaloneEngagementPromptLoad() { }
    /// <summary>
    /// Gets executed every time the scene "StandaloneEngagementPrompt" gets unloaded.
    /// </summary>
    public virtual void OnStandaloneEngagementPromptUnload() { }
    /// <summary>
    /// Gets executed every time the scene "StandaloneEngagementPrompt" gets initialized.
    /// </summary>
    public virtual void OnStandaloneEngagementPromptInitialize() { }
    
    /// <summary>
    /// Gets executed once GameContext loads. In Postfix of the Start method
    /// </summary>
    public virtual void OnGameContext(GameContext gameContext) { }
    
    /// <summary>
    /// Gets executed every time the scene "PlayerCore" gets loaded.
    /// </summary>
    public virtual void OnPlayerCoreLoad() { }
    /// <summary>
    /// Gets executed every time the scene "PlayerCore" gets unloaded.
    /// </summary>
    public virtual void OnPlayerCoreUnload() { }
    /// <summary>
    /// Gets executed every time the scene "PlayerCore" gets initialized.
    /// </summary>
    public virtual void OnPlayerCoreInitialize() { }
    
    /// <summary>
    /// Gets executed every time the scene "UICore" gets loaded.
    /// </summary>
    public virtual void OnUICoreLoad() { }
    /// <summary>
    /// Gets executed every time the scene "UICore" gets unloaded.
    /// </summary>
    public virtual void OnUICoreUnload() { }
    /// <summary>
    /// Gets executed every time the scene "UICore" gets initialized.
    /// </summary>
    public virtual void OnUICoreInitialize() { }
    
    /// <summary>
    /// Gets executed every time the scene "MainMenuUI" gets initialized.
    /// </summary>
    public virtual void OnMainMenuUILoad() { }
    /// <summary>
    /// Gets executed every time the scene "MainMenuUI" gets loaded.
    /// </summary>
    public virtual void OnMainMenuUIUnload() { }
    /// <summary>
    /// Gets executed every time the scene "MainMenuUI" gets unloaded.
    /// </summary>
    public virtual void OnMainMenuUIInitialize() { }
    
    /// <summary>
    /// Gets executed every time the scene "LoadScene" gets loaded.
    /// </summary>
    public virtual void OnLoadSceneLoad() { }
    /// <summary>
    /// Gets executed every time the scene "LoadScene" gets unloaded .
    /// </summary>
    public virtual void OnLoadSceneUnload() { }
    /// <summary>
    /// Gets executed every time the scene "LoadScene" gets initialized.
    /// </summary>
    public virtual void OnLoadSceneInitialize() { }

    /// <summary>
    /// Sends the usage of the command to the in game console 
    /// </summary>
    public bool SendUsage()
    {
        SendMessage(translation("cmd.usage", Usage));
        return false;
    }
    /// <summary>
    /// Sends the no arguments message
    /// </summary>
    public bool SendNoArguments() => SendError(translation("cmd.noarguments"));
    
    
    /// <summary>
    /// Sends the load a save first message
    /// </summary>
    public bool SendLoadASaveFirst() => SendError(translation("cmd.loadasavefirst"));
    /// <summary>
    /// Sends the cheats disabled message
    /// </summary>
    public bool SendCheatsDisabled() => SendError(translation("cmd.cheatsdisabled"));

    /// <summary>
    /// Display a message in the console
    /// </summary>

    public void SendMessage(string message)
    {
        if (!silent) SR2ELogManager.SendMessage(message, SR2EEntryPoint.SR2ELogToMLLog);
    }
    
    /// <summary>
    /// Display an error in the console
    /// </summary>
    public bool SendError(string message)
    {
        if (!silent) SR2ELogManager.SendError(message, SR2EEntryPoint.SR2ELogToMLLog);
        return false;
    }

    /// <summary>
    /// Display an error in the console
    /// </summary>
    public void SendWarning(string message)
    {
        if (!silent) SR2ELogManager.SendWarning(message, SR2EEntryPoint.SR2ELogToMLLog);
    }

    public bool SendCommandMaintenance() => SendError(translation("cmd.error.maintenance"));
    public bool SendNotValidPedia(object obj) => SendError(translation("cmd.error.notvalidpedia",obj));
    public bool SendNotValidVacMode(object obj) => SendError(translation("cmd.error.notvalidvacmode",obj));
    public bool SendNotValidWeather(object obj) => SendError(translation("cmd.error.notvalidweather",obj));
    public bool SendNotValidKeyCode(object obj) => SendError(translation("cmd.error.notvalidkeycode",obj));
    public bool SendNotValidInt(object obj) => SendError(translation("cmd.error.notvalidint",obj));
    public bool SendNotValidFloat(object obj) => SendError(translation("cmd.error.notvalidfloat",obj));
    public bool SendNotValidDouble(object obj) => SendError(translation("cmd.error.notvaliddouble",obj));
    public bool SendNotValidBool(object obj) => SendError(translation("cmd.error.notvalidbool",obj));
    public bool SendNotValidTrool(object obj) => SendError(translation("cmd.error.notvalidtrool",obj));
    public bool SendNotValidVector3(object objX,object objY,object objZ) => SendError(translation("cmd.error.notvalidvector3",objX,objY,objZ));
    public bool SendNotValidIdentType(object obj) => SendError(translation("cmd.error.notvalididenttype",obj));
    public bool SendNotValidGadget(object obj) => SendError(translation("cmd.error.notvalidgadget",obj));
    public bool SendNotValidUpgrade(object obj) => SendError(translation("cmd.error.notvalidupgrade",obj));
    public bool SendNotIntAtLeast(object currValue, object obj) => SendError(translation("cmd.error.notintatleast",currValue,obj));
    public bool SendNotIntAbove(object currValue, object obj) => SendError(translation("cmd.error.notintabove",currValue,obj));
    public bool SendNotIntUnder(object currValue, object obj) => SendError(translation("cmd.error.notintbelow",currValue,obj));
    public bool SendNotFloatAtLeast(object currValue, object obj) => SendError(translation("cmd.error.notfloatatleast",currValue,obj));
    public bool SendNotFloatAbove(object currValue, object obj) => SendError(translation("cmd.error.notfloatabove",currValue,obj));
    public bool SendNotFloatUnder(object currValue, object obj) => SendError(translation("cmd.error.notfloatbelow",currValue,obj));
    public bool SendNotDoubleAtLeast(object currValue, object obj) => SendError(translation("cmd.error.notdoubleatleast",currValue,obj));
    public bool SendNotDoubleAbove(object currValue, object obj) => SendError(translation("cmd.error.notdoubleabove",currValue,obj));
    public bool SendNotDoubleUnder(object currValue, object obj) => SendError(translation("cmd.error.notdoublebelow",currValue,obj));
    public bool SendNullSRCharacterController() => SendError(translation("cmd.error.srccnull"));
    public bool SendNullTeleportablePlayer() => SendError(translation("cmd.error.teleportableplayernull"));
    public bool SendNullKinematicCharacterMotor() => SendError(translation("cmd.error.kinematiccharactermotornull"));
    public bool SendUnsupportedSceneGroup(object obj) => SendError(translation("cmd.error.scenegroupnotsupported",obj));
    public bool SendNoCamera() => SendError(translation("cmd.error.nocamera"));
    public bool SendNotLookingAtValidObject() => SendError(translation("cmd.error.notlookingatvalidobject"));
    public bool SendNotLookingAtAnything() => SendError(translation("cmd.error.notlookingatanything"));
    public bool SendUnknown() => SendError(translation("cmd.error.unknown"));
    public bool SendIsGadgetNotItem(object obj) => SendError(translation("cmd.error.isgadgetnotitem",obj));
    public bool SendErrorToManyArgs(object obj) => SendError(translation("cmd.error.errortoomanyargs",obj));
    public bool SendNotValidOption(object obj) => SendError(translation("cmd.error.notvalidoption",obj));
}
