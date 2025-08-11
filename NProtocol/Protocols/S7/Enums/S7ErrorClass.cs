namespace NProtocol.Protocols.S7.Enums
{
    /// <summary>
    /// Error types
    /// </summary>
    public enum S7ErrorClass : byte
    {
        /// <summary>
        /// No error
        /// </summary>
        NoError = 0x00,

        /// <summary>
        /// Application relationship
        /// </summary>
        ApplicationRelationship = 0x81,

        /// <summary>
        /// Object definition
        /// </summary>
        ObjectDefinition = 0x82,

        /// <summary>
        /// No available resources
        /// </summary>
        NoResourcesAvailable = 0x83,

        /// <summary>
        /// Error during service processing
        /// </summary>
        ErrorOnServiceProcessing = 0x84,

        /// <summary>
        /// Request error
        /// </summary>
        ErrorOnSupplies = 0x85,

        /// <summary>
        /// Access error
        /// </summary>
        AccessError = 0x87,
    }
}
