using System.Collections.Generic;
using System.Linq;
using NProtocol.Exceptions;

namespace NProtocol.Protocols.Modbus
{
    public class ModbusErrorCodeException : ReceivedException
    {
        public ModbusErrorCodeException(
            byte abnormalCode,
            byte errorCode,
            byte[] sendData,
            byte[] receiveData,
            string driverId
        )
            : base(GetErrorMessage(abnormalCode, errorCode), sendData, receiveData, driverId) { }

        public static string GetErrorMessage(byte abnormalCode, byte errorCode)
        {
            string msg = string.Empty;
            if (AbnormalFuncCodeValues.TryGetValue(abnormalCode, out var abnormalMsg))
            {
                msg.Concat($"AbnormalCode:{abnormalCode:X2},AbnormalMessage：{abnormalMsg}.");
            }
            if (ErrorCodeValues.TryGetValue(errorCode, out var errorMsg))
            {
                msg.Concat($"ErrorCode：{errorCode},ErrorMessage:{errorMsg}.");
            }
            return msg;
        }

        public static IReadOnlyDictionary<byte, string> AbnormalFuncCodeValues =>
            new Dictionary<byte, string>()
            {
                { 0x82, "Read input discrete quantities" },
                { 0x81, "Read the coil" },
                { 0x85, "Write a single coil" },
                { 0x8F, "Write multiple coils" },
                { 0x84, "Read input registers" },
                { 0x83, "Read multiple registers" },
                { 0x86, "Write a single register" },
                { 0x90, "Write multiple registers" },
                { 0x97, "Read/write multiple registers" },
                { 0x96, "Mask write registers" },
            };
        public static IReadOnlyDictionary<byte, string> ErrorCodeValues =>
            new Dictionary<byte, string>()
            {
                { 0x01, "Illegal function" },
                { 0x02, "Illegal data address" },
                { 0x03, "Illegal data value" },
                { 0x04, "Slave device failure" },
                { 0x05, "Acknowledge" },
                { 0x06, "Slave device busy" },
                { 0x08, "Memory parity error" },
                { 0x0A, "Gateway path unavailable" },
                { 0x0B, "Gateway target device dailed to respond" },
            };
    }
}
