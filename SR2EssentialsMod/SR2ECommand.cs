using System;
using SR2E.Managers;

namespace SR2E;

/// <summary>
/// Abstract console command class
/// </summary>
public abstract class SR2ECommand
{
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
        if(!silent) SR2ELogManager.SendMessage(translation("cmd.usage", Usage));
        return false;
    }
    /// <summary>
    /// Sends the no arguments message
    /// </summary>
    public bool SendNoArguments()
    {
        if(!silent) SR2ELogManager.SendError(translation("cmd.noarguments"));
        return false;
    }
    
    /// <summary>
    /// Sends the load a save first message
    /// </summary>
    public bool SendLoadASaveFirst()
    {
        if(!silent) SR2ELogManager.SendError(translation("cmd.loadasavefirst"));
        return false;
    }
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

    public bool silent = false;


}
[Flags]
public enum CommandType
{
    None = 0,
    DontLoad = 1 << 1,
    DevOnly = 1 << 2,
    Cheat = 1 << 3,
    Binding = 1 << 4,
    Warp = 1 << 5,
    Common = 1 << 6,
    Menu = 1 << 7,
    Miscellaneous = 1 << 8,
    Fun = 1 << 9,
    Experimental = 1 << 10
}