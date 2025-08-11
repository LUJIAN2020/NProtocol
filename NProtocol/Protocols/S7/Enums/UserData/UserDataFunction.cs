namespace NProtocol.Protocols.S7.Enums.UserData
{
    /// <summary>
    /// Function codes for user data
    /// </summary>
    public enum UserDataFunction : byte
    {
        /// <summary>
        /// Mode transition
        /// </summary>
        ModeTransition = 0x00,

        /// <summary>
        /// Engineer debugging commands
        /// </summary>
        ProgrammerCommands = 0x01,

        /// <summary>
        /// Cyclic read
        /// </summary>
        CyclicData = 0x02,

        /// <summary>
        /// Block functions
        /// </summary>
        BlockFunctions = 0x03,

        /// <summary>
        /// CPU functions
        /// </summary>
        CPUFunctions = 0x04,

        /// <summary>
        /// Safety functions
        /// </summary>
        Security = 0x05,

        /// <summary>
        /// Programmable block function send/receive
        /// </summary>
        PBC_BSEND_BRECV = 0x06,

        /// <summary>
        /// Time function codes
        /// </summary>
        TimeFunctions = 0x07,

        /// <summary>
        /// NC programming
        /// </summary>
        NcProgramming = 0x0F,
    }
}
