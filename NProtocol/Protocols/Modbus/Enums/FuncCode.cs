namespace NProtocol.Protocols.Modbus
{
    public enum FuncCode : byte
    {
        /// <summary>
        /// Read coil FC01 operation data type: bit
        /// </summary>
        ReadCoils = 0x01,

        /// <summary>
        /// Read discrete input FC02 is read only. Operation data type: bit
        /// </summary>
        ReadDiscreteInputs = 0x02,

        /// <summary>
        /// Read the holding register FC03
        /// </summary>
        ReadHoldingRegisters = 0x03,

        /// <summary>
        /// Read input register FC04 is read only
        /// </summary>
        ReadInputRegisters = 0x04,

        /// <summary>
        /// Write single coil FC05 operation data type: bit
        /// </summary>
        WriteSingleCoil = 0x05,

        /// <summary>
        /// Write a single register FC06
        /// </summary>
        WriteSingleRegister = 0x06,

        /// <summary>
        /// Write multiple coils FC15
        /// </summary>
        WriteMultipleCoils = 0x0F,

        /// <summary>
        /// Write multiple registers FC16
        /// </summary>
        WriteMultipleRegisters = 0x10,

        /// <summary>
        /// Read and write multiple registers FC23
        /// </summary>
        ReadWriteMultipleRegisters = 0x17,
    }
}
