using System;

namespace SR2E;

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
    public virtual CommandType type { get; } = CommandType.DontLoad;


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
    /// Allows the execution of the Command when the console is open
    /// </summary>
    public virtual bool execWhenIsOpenConsole { get; } = false;

    /// <summary>
    /// Allows the execution of the Command when the cheat menu is open
    /// </summary>
    public virtual bool execWhenIsOpenCheatMenu { get; } = false;

    /// <summary>
    /// Allows the execution of the Command when the mod menu is open
    /// </summary>
    public virtual bool execWhenIsOpenModMenu { get; } = false;


    /// <summary>
    /// Gets called every frame
    /// </summary>
    public virtual void Update()
    { }
    /// <summary>
    /// Gets called when the scene GameCore loads
    /// </summary>
    public virtual void OnGameCoreLoad()
    { }
    /// <summary>
    /// Gets called when the scene UICore loads
    /// </summary>
    public virtual void OnUICoreLoad()
    { }
    /// <summary>
    /// Gets called when the scene PlayerCore loads
    /// </summary>
    public virtual void OnPlayerCoreLoad()
    { }
    /// <summary>
    /// Gets called when the scene MainMenuUI loads
    /// </summary>
    public virtual void OnMainMenuUILoad()
    { }

    /// <summary>
    /// Sends the usage of the command to the in game console 
    /// </summary>
    public bool SendUsage()
    {
        if(!silent) SR2EConsole.SendMessage(translation("cmd.usage", Usage));
        return false;
    }
    /// <summary>
    /// Sends the no arguments message
    /// </summary>
    public bool SendNoArguments()
    {
        if(!silent) SR2EConsole.SendError(translation("cmd.noarguments"));
        return false;
    }
    
    /// <summary>
    /// Sends the load a save first message
    /// </summary>
    public bool SendLoadASaveFirst()
    {
        if(!silent) SR2EConsole.SendError(translation("cmd.loadasavefirst"));
        return false;
    }
    /// <summary>
    /// Display a message in the console
    /// </summary>

    public void SendMessage(string message)
    {
        if (!silent) SR2EConsole.SendMessage(message, SR2EEntryPoint.syncConsole);
    }
    
    /// <summary>
    /// Display an error in the console
    /// </summary>
    public bool SendError(string message)
    {
        if (!silent) SR2EConsole.SendError(message, SR2EEntryPoint.syncConsole);
        return false;
    }

    /// <summary>
    /// Display an error in the console
    /// </summary>
    public void SendWarning(string message)
    {
        if (!silent) SR2EConsole.SendWarning(message, SR2EEntryPoint.syncConsole);
    }

    public bool silent = false;


}
[Flags]
public enum CommandType
{
    DontLoad = 0,
    DevOnly = 1 << 1,
    Cheat = 1 << 2,
    Binding = 1 << 3,
    Warp = 1 << 4,
    Common = 1 << 5,
    Menu = 1 << 6,
    Miscellaneous = 1 << 7,
    Fun = 1 << 8,
    Experimental = 1 << 9
}