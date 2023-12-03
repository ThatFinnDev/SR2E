namespace SR2E;

public abstract class SR2CCommand
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
    public abstract string Description { get; }


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
    public virtual bool executeWhenConsoleIsOpen { get; } = false;


    /// <summary>
    /// Executes the command silently (for keybinds)
    /// </summary>
    /// <param name="args">The arguments passed in the console (null if no arguments are provided)</param>
    /// <returns>Return True at the end, if the normal execute should not be executed</returns>
    public virtual bool SilentExecute(string[] args)
    { return false; }
}
