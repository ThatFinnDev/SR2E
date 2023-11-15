namespace SR2E
{
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
    }
}